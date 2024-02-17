using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public static string strWeaponSaveData = "WeaponSaveData", strResourceSaveData = "ResourceSaveData";
    public Item_Database itemDatabase;
    public List<WeaponSaveData> weaponSaveDatas = new List<WeaponSaveData>();
    public List<ResourceSaveData> resourceSaveDatas = new List<ResourceSaveData>();

    public Dictionary<Item_Resource, ResourceSaveData> resourceDataLinker =
        new Dictionary<Item_Resource, ResourceSaveData>();
    public Dictionary<Item_Weapon, WeaponSaveData> weaponDataLinker =
        new Dictionary<Item_Weapon, WeaponSaveData>();
    
    private AsyncOperationHandle handle;

    public void Awake()
    {
        instance = this;
        Load();
        resourceDataLinker = new Dictionary<Item_Resource, ResourceSaveData>();
        weaponDataLinker = new Dictionary<Item_Weapon, WeaponSaveData>();
        foreach (var rsd in resourceSaveDatas) resourceDataLinker.Add(rsd.resource,rsd);
        foreach (var wsd in weaponSaveDatas) weaponDataLinker.Add(wsd.weapon,wsd);
    }
    private void Load()
    {
        if (ES3.KeyExists(strResourceSaveData)) resourceSaveDatas = ES3.Load<List<ResourceSaveData>>(strResourceSaveData);
        else
        {
            resourceSaveDatas = new List<ResourceSaveData>();
            for(int i=0;i<itemDatabase.resources.Count; i++)
            {
                var itemResource = itemDatabase.resources[i];
                int count = 0;
                var state = InventorySlot.SlotState.Locked;
                resourceSaveDatas.Add(new ResourceSaveData(itemResource, count,state));
            }
        }
        
        if (ES3.KeyExists(strWeaponSaveData)) weaponSaveDatas = ES3.Load<List<WeaponSaveData>>(strWeaponSaveData);
        else
        {
            weaponSaveDatas = new List<WeaponSaveData>();
            for(int i=0;i<itemDatabase.weapons.Count; i++)
            {
                var itemWeapon = itemDatabase.weapons[i];
                int count = i < 3 ? 1 : 0;
                var state = i < 3 ? InventorySlot.SlotState.Opened : InventorySlot.SlotState.Locked;
                weaponSaveDatas.Add(new WeaponSaveData(itemWeapon, count,state));
            }
        }
    }
    private void Save()
    {
        ES3.Save(strResourceSaveData,resourceSaveDatas);
        ES3.Save(strWeaponSaveData,weaponSaveDatas);
    }
    [Button]
    public void ClearData()
    {
        if(ES3.KeyExists(strWeaponSaveData)) ES3.DeleteKey(strWeaponSaveData);
        if(ES3.KeyExists(strResourceSaveData)) ES3.DeleteKey(strResourceSaveData);
    }
    [Button]
    public void FindResource(Item_Resource resource)
    {
        var saveData = resourceDataLinker[resource];
        if (saveData.slotState != InventorySlot.SlotState.Locked) return;
        saveData.slotState = InventorySlot.SlotState.Found;
        Save();
    }
    [Button]
    public void AddResource(Item_Resource resource,int count)
    {
        if (count <= 0) return;
        var saveData = resourceDataLinker[resource];
        saveData.slotState = InventorySlot.SlotState.Opened;
        saveData.count += count;
        Save();
    }
    [Button]
    public bool RemoveResource(Item_Resource resource,int count)
    {
        var saveData = resourceDataLinker[resource];
        if (saveData.slotState == InventorySlot.SlotState.Locked) return false;
        if (saveData.count < count) return false;

        saveData.count -= count;
        if (saveData.count > 0) saveData.slotState = InventorySlot.SlotState.Opened;
        else saveData.slotState = InventorySlot.SlotState.Found;
        Save();
        return true;
    }
    [Button]
    public void FindWeapon(Item_Weapon weapon)
    {
        var saveData = weaponDataLinker[weapon];
        if (saveData.slotState != InventorySlot.SlotState.Locked) return;
        saveData.slotState = InventorySlot.SlotState.Found;
        Save();
    }
    [Button]
    public void AddWeapon(Item_Weapon weapon,int count)
    {
        if (count <= 0) return;
        var saveData = weaponDataLinker[weapon];
        saveData.slotState = InventorySlot.SlotState.Opened;
        saveData.count += count;
        Save();
    }
    [Button]
    public bool RemoveWeapon(Item_Weapon weapon,int count)
    {
        var saveData = weaponDataLinker[weapon];
        if (saveData.slotState == InventorySlot.SlotState.Locked) return false;
        if (saveData.count < count) return false;

        saveData.count -= count;
        if (saveData.count > 0) saveData.slotState = InventorySlot.SlotState.Opened;
        else saveData.slotState = InventorySlot.SlotState.Found;
        Save();
        return true;
    }
    
    public void LoadAdressables(string tag)
    {
        Addressables.LoadAssetAsync<Item_Resource>(tag).Completed +=
            (AsyncOperationHandle<Item_Resource> resource) =>
            {
                handle = resource;
            };
    }
    public void UnloadAdressables()
    {
        Addressables.Release(handle);
    }
}

[System.Serializable]
public class SaveData
{
    public InventorySlot.SlotState slotState;
    public int count;
}

[System.Serializable]
public class WeaponSaveData: SaveData
{
    public Item_Weapon weapon;
    public WeaponSaveData(Item_Weapon _weapon,int _count,InventorySlot.SlotState _slotState)
    {
        weapon = _weapon;
        count = _count;
        slotState = _slotState;
    }
}
[System.Serializable]
public class ResourceSaveData: SaveData
{
    public Item_Resource resource;
    public ResourceSaveData(Item_Resource _resource,int _count,InventorySlot.SlotState _slotState)
    {
        resource = _resource;
        count = _count;
        slotState = _slotState;
    }
}
