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
    public static string 
        strWeaponSaveData = "WeaponSaveData", 
        strResourceSaveData = "ResourceSaveData",
        strForgeWeaponsData = "ForgeWeaponData",
        strEquipWeaponMainData = "EquipWeaponMainData",
        strEquipWeaponSkillLData = "EquipWeaponSkillLData",
        strEquipWeaponSkillRData = "EquipWeaponSkillRData",
        strCoinData = "CoinData",
        strGemData = "GemData";
    public Item_Database itemDatabase;
    public List<WeaponSaveData> weaponSaveDatas = new List<WeaponSaveData>();
    public List<ResourceSaveData> resourceSaveDatas = new List<ResourceSaveData>();
    public List<int> forgeWeaponDatas = new List<int>();
    public int equipWeaponMain,equipWeaponSkillL,equipWeaponSkillR;
    public int coin, gem;


    public Dictionary<Item_Resource, ResourceSaveData> resourceDataLinker =
        new Dictionary<Item_Resource, ResourceSaveData>();
    public Dictionary<Item_Weapon, WeaponSaveData> weaponDataLinker =
        new Dictionary<Item_Weapon, WeaponSaveData>();

    public int maxForgeCount = 6;
    private AsyncOperationHandle handle;

    public void Awake()
    {
        instance = this;
        Load();
        Save();
        resourceDataLinker = new Dictionary<Item_Resource, ResourceSaveData>();
        weaponDataLinker = new Dictionary<Item_Weapon, WeaponSaveData>();
        foreach (var rsd in resourceSaveDatas) resourceDataLinker.Add(itemDatabase.resources[rsd.resourceIndex],rsd);
        foreach (var wsd in weaponSaveDatas) weaponDataLinker.Add(itemDatabase.weapons[wsd.weaponIndex],wsd);
    }
    private void Load()
    {
        //인벤토리
        if (ES3.KeyExists(strResourceSaveData)) resourceSaveDatas = ES3.Load<List<ResourceSaveData>>(strResourceSaveData);
        else
        {
            resourceSaveDatas = new List<ResourceSaveData>();
            for(int i=0;i<itemDatabase.resources.Count; i++)
            {
                int count = 0;
                var state = InventorySlot.SlotState.Locked;
                resourceSaveDatas.Add(new ResourceSaveData(i, count,state));
            }
        }
        if (ES3.KeyExists(strWeaponSaveData)) weaponSaveDatas = ES3.Load<List<WeaponSaveData>>(strWeaponSaveData);
        else
        {
            weaponSaveDatas = new List<WeaponSaveData>();
            for(int i=0;i<itemDatabase.weapons.Count; i++)
            {
                int count = i < 3 ? 1 : 0;
                var state = i < 3 ? InventorySlot.SlotState.Opened : InventorySlot.SlotState.Locked;
                weaponSaveDatas.Add(new WeaponSaveData(i, count,state));
            }
        }
        //대장간
        if (ES3.KeyExists(strForgeWeaponsData)) forgeWeaponDatas = ES3.Load<List<int>>(strForgeWeaponsData);
        else
        {
            forgeWeaponDatas = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                print("newnew");
                forgeWeaponDatas.Add(i);
            }
        }
        //착용장비
        if (ES3.KeyExists(strEquipWeaponMainData)) equipWeaponMain = ES3.Load<int>(strEquipWeaponMainData);
        else equipWeaponMain = weaponSaveDatas[0].weaponIndex;
        if (ES3.KeyExists(strEquipWeaponSkillLData)) equipWeaponSkillL = ES3.Load<int>(strEquipWeaponSkillLData);
        else equipWeaponSkillL = weaponSaveDatas[1].weaponIndex;
        if (ES3.KeyExists(strEquipWeaponSkillRData)) equipWeaponSkillR = ES3.Load<int>(strEquipWeaponSkillRData);
        else equipWeaponSkillR = weaponSaveDatas[2].weaponIndex;
        //돈
        if (ES3.KeyExists(strCoinData)) coin = ES3.Load<int>(strCoinData);
        else coin = 9999;
        if (ES3.KeyExists(strGemData)) coin = ES3.Load<int>(strGemData);
        else gem = 9999;
    }
    private void Save()
    {
        ES3.Save(strResourceSaveData,resourceSaveDatas);
        ES3.Save(strWeaponSaveData,weaponSaveDatas);
        ES3.Save(strForgeWeaponsData,forgeWeaponDatas);
        ES3.Save(strEquipWeaponMainData,equipWeaponMain);
        ES3.Save(strEquipWeaponSkillLData,equipWeaponSkillL);
        ES3.Save(strEquipWeaponSkillRData,equipWeaponSkillR);
    }

    public Item_Weapon GetWeapon(int index)
    {
        return itemDatabase.weapons[index];
    }

    public Item_Resource GetResource(int index)
    {
        return itemDatabase.resources[index];
    }
    [Button]
    public void ClearData()
    {
        if(ES3.KeyExists(strWeaponSaveData)) ES3.DeleteKey(strWeaponSaveData);
        if(ES3.KeyExists(strResourceSaveData)) ES3.DeleteKey(strResourceSaveData);
        if(ES3.KeyExists(strForgeWeaponsData)) ES3.DeleteKey(strForgeWeaponsData);
        if(ES3.KeyExists(strEquipWeaponMainData)) ES3.DeleteKey(strEquipWeaponMainData);
        if(ES3.KeyExists(strEquipWeaponSkillLData)) ES3.DeleteKey(strEquipWeaponSkillLData);
        if(ES3.KeyExists(strEquipWeaponSkillRData)) ES3.DeleteKey(strEquipWeaponSkillRData);
        if(ES3.KeyExists(strCoinData)) ES3.DeleteKey(strCoinData);
        if(ES3.KeyExists(strGemData)) ES3.DeleteKey(strGemData);
    }
    //인벤토리
    [Button]
    public void Resource_Add(Item_Resource resource,int count)
    {
        if (count < 0) return;
        ResourceSaveData saveData = resourceDataLinker[resource];
        saveData.count += count;
        if(saveData.count ==0) saveData.slotState = InventorySlot.SlotState.Found;
        else saveData.slotState = InventorySlot.SlotState.Opened;
        Save();
    }
    public bool Resource_Remove(Item_Resource resource,int count)
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
    public void Weapon_Add(Item_Weapon weapon,int count)
    {
        if (count < 0) return;
        var saveData = weaponDataLinker[weapon];
        saveData.count += count;
        if(saveData.count ==0) saveData.slotState = InventorySlot.SlotState.Found;
        else saveData.slotState = InventorySlot.SlotState.Opened;
        Save();
    }
    public bool Weapon_Remove(Item_Weapon weapon,int count)
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
    //대장간
    public bool Forge_Add(Item_Weapon weapon)
    {
        int index = itemDatabase.weapons.IndexOf(weapon);
        if (forgeWeaponDatas.Count<maxForgeCount && !forgeWeaponDatas.Contains(index))
        {
            forgeWeaponDatas.Add(index);
            Save();
            return true;
        }
        else return false;
    }
    public bool Forge_Remove(Item_Weapon weapon)
    {
        int index = itemDatabase.weapons.IndexOf(weapon);
        if (forgeWeaponDatas.Contains(index))
        {
            forgeWeaponDatas.Remove(index);
            Save();
            return true;
        }
        else return false;
    }
    //착용
    public void EquipUpdate(Item_Weapon main,Item_Weapon skillL,Item_Weapon skillR)
    {
        equipWeaponMain = itemDatabase.weapons.IndexOf(main);
        equipWeaponSkillL = itemDatabase.weapons.IndexOf(skillL);
        equipWeaponSkillR = itemDatabase.weapons.IndexOf(skillR);
        Save();
    }
    //돈
    public void Coin_Add(int value)
    {
        coin += value;
        Save();
    }
    public bool Coin_Remove(int value)
    {
        if (coin <= value) return false;
        else
        {
            coin -= value;
            Save();
            return true;
        }
    }
    public void Gem_Add(int value)
    {
        gem += value;
        Save();
    }
    public bool Gem_Remove(int value)
    {
        if (gem <= value) return false;
        else
        {
            gem -= value;
            Save();
            return true;
        }
    }

    [Button]
    public void SetDebugSetting()
    {
        Resource_Add(itemDatabase.resources[0],0);
        Resource_Add(itemDatabase.resources[1],25);
        Resource_Add(itemDatabase.resources[2],5);
        Resource_Add(itemDatabase.resources[3],30);
        Resource_Add(itemDatabase.resources[4],15);
        Resource_Add(itemDatabase.resources[5],25);
        Resource_Add(itemDatabase.resources[7],0);
        Save();
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
    public int weaponIndex;
    public WeaponSaveData(int _weaponIndex,int _count,InventorySlot.SlotState _slotState)
    {
        weaponIndex = _weaponIndex;
        count = _count;
        slotState = _slotState;
    }
}
[System.Serializable]
public class ResourceSaveData: SaveData
{
    public int resourceIndex;
    public ResourceSaveData(int _resourceIndex,int _count,InventorySlot.SlotState _slotState)
    {
        resourceIndex = _resourceIndex;
        count = _count;
        slotState = _slotState;
    }
}
