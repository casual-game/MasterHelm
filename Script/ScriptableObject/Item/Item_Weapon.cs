using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Weapon", menuName = "Item/Weapon", order = 1)]
public class Item_Weapon : ScriptableObject
{
    [TitleGroup("세팅")] 
    [FoldoutGroup("세팅/Setting")] public AssetReference refHighPolyWeaponL;
    [FoldoutGroup("세팅/Setting")] public AssetReference refHighPolyWeaponR;
    [FoldoutGroup("세팅/Setting")] public Vector3 camDeg;
    [FoldoutGroup("세팅/Setting")] public Vector3 camLocalPos;
    [FoldoutGroup("세팅/Setting")] public WeaponHighpolyType highpolyType;
    [FoldoutGroup("세팅/Setting")] public Data_WeaponPack weaponPack;
    [FoldoutGroup("세팅/Setting")] public Sprite icon;
    [FoldoutGroup("세팅/Setting")] public string title; 
    [FoldoutGroup("세팅/Setting")][TextArea] public string info;
    [FoldoutGroup("세팅/Setting")] public int hp = 25, power = 15;
    [TitleGroup("인벤토리")]
    [FoldoutGroup("인벤토리/Inventory")] public float left,right,top,bottom;
    [FoldoutGroup("인벤토리/Inventory")] public Vector3 scale = Vector3.one;
    [TitleGroup("상점")]
    [FoldoutGroup("상점/Shop")][LabelText("Left")] public float sleft;
    [FoldoutGroup("상점/Shop")][LabelText("Right")]  public float sright;
    [FoldoutGroup("상점/Shop")][LabelText("Up")]  public float stop;
    [FoldoutGroup("상점/Shop")][LabelText("Down")]  public float sbottom;
    [FoldoutGroup("상점/Shop")] public Vector3 sscale = Vector3.one;
    [FoldoutGroup("상점/Shop")] public Color decoColor;
    [FoldoutGroup("상점/Shop")] public Sprite star;
    [TitleGroup("가격")]
    [FoldoutGroup("가격/Price")] public int price;
    [FoldoutGroup("가격/Price")] public bool isGem;
    [TitleGroup("대장간")]
    [FoldoutGroup("대장간/Blueprint")] public Item_Resource bpResource1, bpResource2, bpResource3;
    [FoldoutGroup("대장간/Blueprint")] public Item_Weapon bpWeapon1,bpWeapon2,bpWeapon3;
    [FoldoutGroup("대장간/Blueprint")] public int bpCount1, bpCount2, bpCount3;

    //상태 체크용 함수
    public bool CanCreate(SaveManager saveManager)
    {
        bool check1 = false, check2 = false, check3 = false;
        
        if (bpWeapon1 != null && saveManager.weaponDataLinker[bpWeapon1].count >= bpCount1) check1 = true;
        else if (bpResource1 != null && saveManager.resourceDataLinker[bpResource1].count >= bpCount1) check1 = true;
        
        if (bpWeapon2 != null && saveManager.weaponDataLinker[bpWeapon2].count >= bpCount2) check2 = true;
        else if (bpResource2 != null && saveManager.resourceDataLinker[bpResource2].count >= bpCount2) check2 = true;
        
        if (bpWeapon3 != null && saveManager.weaponDataLinker[bpWeapon3].count >= bpCount3) check3 = true;
        else if (bpResource3 != null && saveManager.resourceDataLinker[bpResource3].count >= bpCount3) check3 = true;
        
        return check1 && check2 && check3;
    }
    public bool Create(SaveManager saveManager)
    {
        if (CanCreate(saveManager))
        {
            if (bpWeapon1 != null) saveManager.Weapon_Remove(bpWeapon1, bpCount1);
            else if (bpResource1 != null) saveManager.Resource_Remove(bpResource1, bpCount1);
            if (bpWeapon2 != null) saveManager.Weapon_Remove(bpWeapon2, bpCount2);
            else if (bpResource2 != null) saveManager.Resource_Remove(bpResource2, bpCount2);
            if (bpWeapon3 != null) saveManager.Weapon_Remove(bpWeapon3, bpCount3);
            else if (bpResource3 != null) saveManager.Resource_Remove(bpResource3, bpCount3);
            saveManager.Weapon_Add(this,1);
            return true;
        }
        else return false;
    }
}
public enum WeaponHighpolyType {Greatsword=0,DoubleAxe=1,Hammer=2}
