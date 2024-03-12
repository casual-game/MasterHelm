using System;
using System.Collections;
using System.Collections.Generic;
using AmazingAssets.AdvancedDissolve;
using EPOOutline;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class SideUI : MonoBehaviour
{
    [FoldoutGroup("Hero")] public Transform tHandL, tHandR,tBackL,tBackR;
    [FoldoutGroup("Hero")] public Outlinable outlinable;
    [FoldoutGroup("Hero")] public Transform heroCamT;
    [FoldoutGroup("Hero")] public Animator heroAnim;
    [FoldoutGroup("Hero")] public Material heroMat;
    [FoldoutGroup("Hero")] public Transform tRTHero;

    private GameObject tempWeaponL, tempWeaponR;
    private OutlineTarget tempOutlineTargetL, tempOutlineTargetR;
    private Item_Weapon _currentWeapon = null;
    private static string strGreatsword = "Greatsword", strDoubleAxe = "DoubleAxe", strHammer = "Hammer",strIdle = "Idle",
        strShineLocation = "_ShineLocation",strHitEffectBlend = "_HitEffectBlend";
    private Tween _tweenDissolveL, _tweenDissolveR;
    private Sequence _seqHero;
    private bool _isBack = true;
    private AnimationCurve _curve = AnimationCurve.EaseInOut(0,0,1,1);
    [Button]
    public void EquipWeapon(Item_Weapon weapon, bool isBack)
    {
        if(isBack!=_isBack || _currentWeapon!= weapon) Impact();
        _isBack = isBack;
        //애니메이션
        if (isBack)
        {
            if (!heroAnim.GetCurrentAnimatorStateInfo(0).IsName(strIdle)) heroAnim.Play(strIdle);
            heroCamT.SetLocalPositionAndRotation(new Vector3(0,-0.025f,0),Quaternion.Euler(0,15,0));
        }
        else
        {
            switch (weapon.highpolyType)
            {
                case WeaponHighpolyType.Greatsword:
                    if (!heroAnim.GetCurrentAnimatorStateInfo(0).IsName(strGreatsword)) heroAnim.Play(strGreatsword);
                    break;
                case WeaponHighpolyType.DoubleAxe:
                    if (!heroAnim.GetCurrentAnimatorStateInfo(0).IsName(strDoubleAxe)) heroAnim.Play(strDoubleAxe);
                    break;
                case WeaponHighpolyType.Hammer:
                    if (!heroAnim.GetCurrentAnimatorStateInfo(0).IsName(strHammer)) heroAnim.Play(strHammer);
                    break;
            }   
            heroCamT.SetLocalPositionAndRotation(weapon.camLocalPos,Quaternion.Euler(0,weapon.camDeg,0));
        }
        //로딩
        if(_currentWeapon!=null && _currentWeapon!=weapon) ReleaseWeapon(_currentWeapon);
        LoadWeapon(weapon,isBack);
    }
    public void LoadWeapon(Item_Weapon weapon, bool isBack)
    {
        _tweenDissolveL.Stop();
        _tweenDissolveR.Stop();
        if (tempOutlineTargetL != null && tempWeaponL!=null)
        {
            tempOutlineTargetL.CutoutThreshold = 1;
            AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(tempOutlineTargetL.
                renderer.material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1);
        }

        if (tempOutlineTargetR != null && tempWeaponR!=null)
        {
            tempOutlineTargetR.CutoutThreshold = 1;
            AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(tempOutlineTargetR.
                renderer.material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1); 
        }
        
        Transform parent;
        if (_currentWeapon != weapon)
        {
            _currentWeapon = weapon;
            if (weapon.refHighPolyWeaponL.RuntimeKeyIsValid())
            {
                if (isBack) parent = tBackL;
                else parent = tHandL;
                weapon.refHighPolyWeaponL.InstantiateAsync(Vector3.zero, Quaternion.identity, parent).Completed +=
                    (AsyncOperationHandle<GameObject> obj) =>
                    {
                        tempWeaponL = obj.Result;
                        Transform weaponT = tempWeaponL.transform;
                        tempOutlineTargetL = new OutlineTarget(tempWeaponL.GetComponent<Renderer>(),
                            "_AdvancedDissolveCutoutStandardMap1", 1.0f);
                        outlinable.TryAddTarget(tempOutlineTargetL);
                        if (isBack)
                        {
                            Transform copyT = tempWeaponL.transform.GetChild(1);
                            weaponT.SetLocalPositionAndRotation(copyT.localPosition, copyT.localRotation);
                            weaponT.localScale = copyT.localScale;
                        }
                        else
                        {
                            Transform copyT = tempWeaponL.transform.GetChild(0);
                            weaponT.SetLocalPositionAndRotation(copyT.localPosition, copyT.localRotation);
                            weaponT.localScale = copyT.localScale;
                        }

                        _tweenDissolveL = Tween.Custom(1, 0, 0.5f, 
                            onValueChange: val =>
                            {
                                tempOutlineTargetL.CutoutThreshold = val;
                                AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(tempOutlineTargetL.
                                    renderer.material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,val);
                            });
                    };
            }
            else tempOutlineTargetL = null;

            if (weapon.refHighPolyWeaponR.RuntimeKeyIsValid())
            {
                if (isBack) parent = tBackR;
                else parent = tHandR;
                weapon.refHighPolyWeaponR.InstantiateAsync(Vector3.zero, Quaternion.identity, parent).Completed +=
                    (AsyncOperationHandle<GameObject> obj) =>
                    {
                        tempWeaponR = obj.Result;
                        tempOutlineTargetR = new OutlineTarget(tempWeaponR.GetComponent<Renderer>(),
                            "_AdvancedDissolveCutoutStandardMap1", 1.0f);
                        outlinable.TryAddTarget(tempOutlineTargetR);
                        Transform weaponT = tempWeaponR.transform;
                        if (isBack)
                        {
                            Transform copyT = tempWeaponR.transform.GetChild(1);
                            weaponT.SetLocalPositionAndRotation(copyT.localPosition, copyT.localRotation);
                            weaponT.localScale = copyT.localScale;
                        }
                        else
                        {
                            Transform copyT = tempWeaponR.transform.GetChild(0);
                            weaponT.SetLocalPositionAndRotation(copyT.localPosition, copyT.localRotation);
                            weaponT.localScale = copyT.localScale;
                        }

                        _tweenDissolveR = Tween.Custom(1, 0, 0.5f, 
                            onValueChange: val =>
                            {
                                tempOutlineTargetR.CutoutThreshold = val;
                                AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(tempOutlineTargetR.
                                    renderer.material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,val);
                            });
                    };
            }
            else tempOutlineTargetR = null;
        }
        else
        {
            //왼쪽 무기
            if (tempWeaponL != null)
            {
                if (isBack)
                {
                    Transform copyT = tempWeaponL.transform.GetChild(1);
                    tempWeaponL.transform.SetParent(tBackL);
                    tempWeaponL.transform.SetLocalPositionAndRotation(copyT.localPosition,copyT.localRotation);
                    tempWeaponL.transform.localScale = copyT.localScale;
                }
                else
                {
                    tempWeaponL.transform.SetParent(tHandL);
                    Transform copyT = tempWeaponL.transform.GetChild(0);
                    tempWeaponL.transform.SetLocalPositionAndRotation(copyT.localPosition,copyT.localRotation);
                    tempWeaponL.transform.localScale = copyT.localScale;
                }
                
                _tweenDissolveL = Tween.Custom(1, 0, 0.5f, 
                    onValueChange: val =>
                    {
                        tempOutlineTargetL.CutoutThreshold = val;
                        AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(tempOutlineTargetL.
                            renderer.material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,val);
                    });
            }
            //오른쪽 무기
            if (tempWeaponR != null)
            {
                if (isBack)
                {
                    tempWeaponR.transform.SetParent(tBackR);
                    Transform copyT = tempWeaponR.transform.GetChild(1);
                    tempWeaponR.transform.SetLocalPositionAndRotation(copyT.localPosition,copyT.localRotation);
                    tempWeaponR.transform.localScale = copyT.localScale;
                }
                else
                {
                    tempWeaponR.transform.SetParent(tHandR);
                    Transform copyT = tempWeaponR.transform.GetChild(0);
                    tempWeaponR.transform.SetLocalPositionAndRotation(copyT.localPosition,copyT.localRotation);
                    tempWeaponR.transform.localScale = copyT.localScale;
                }
                
                _tweenDissolveR = Tween.Custom(1, 0, 0.5f, 
                    onValueChange: val =>
                    {
                        tempOutlineTargetR.CutoutThreshold = val;
                        AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(tempOutlineTargetR.
                            renderer.material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,val);
                    });
            }
        }
    }
    public void ReleaseWeapon(Item_Weapon weapon)
    {
        if (weapon.refHighPolyWeaponL.RuntimeKeyIsValid() && tempWeaponL!=null)
        {
            outlinable.RemoveTarget(tempOutlineTargetL);
            weapon.refHighPolyWeaponL.ReleaseInstance(tempWeaponL);
            tempWeaponL = null;
        }

        if (weapon.refHighPolyWeaponR.RuntimeKeyIsValid()&& tempWeaponR!=null)
        {
            outlinable.TryAddTarget(tempOutlineTargetR);
            weapon.refHighPolyWeaponR.ReleaseInstance(tempWeaponR);
            tempWeaponR = null;
        }
    }
    public void Impact()
    {
        _seqHero.Stop();
        tRTHero.localScale = Vector3.one;
        heroMat.SetFloat(strHitEffectBlend,1);
        heroMat.SetFloat(strShineLocation,0);
        
        _seqHero = Sequence.Create();
        _seqHero.Group(Tween.Custom(1, 0, 0.25f, onValueChange: val =>
        {
            heroMat.SetFloat(strHitEffectBlend,val);
            heroMat.SetFloat(strShineLocation,1-val);
        }));
        _seqHero.Group(Tween.PunchScale(tRTHero, Vector3.one * -0.2f, 0.15f, 2));
    }
}
