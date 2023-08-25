using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Dest.Math;
using DG.Tweening;
using FIMSpace;
using HighlightPlus;
using Pathfinding;
using RootMotion.Dynamics;
using Random = UnityEngine.Random;

public partial class Player : MonoBehaviour
{

    [HideInInspector] public int state=0;
    [HideInInspector] public Vector3 rollVec;
    [HideInInspector] public List<Enemy> attackedTarget = new List<Enemy>();
    [HideInInspector] public bool guard = false;
    [HideInInspector] public Vector3 guardPoint;
    [HideInInspector] public bool guarded = false;
    [HideInInspector] public bool death = false, clear = false;
    [HideInInspector] public float skillBeginTime = -100,rollBeginTime = -100;
    [HideInInspector] public bool isRevengeSkill = false;
    
    public bool CanRoll()
    {
        if (death || clear) return false;
        bool canRoll = !Canvas_Player.AS_Dragged && !Canvas_Player.AB_Pressed
                       && Canvas_Player.AB_ReleasedTime -  Canvas_Player.AB_PressedTime < preInput_Roll
                       && state != 3
                       && (lastState != 3 || !animator.IsInTransition(0));
        if (canRoll)
        {
            Canvas_Player.AB_PressedTime = -100;
        }
        return canRoll;
    }
    public void Roll()
    {
        if (death || clear) return;
        Cancel();
        ChangeState(3);
        
        rollVec = Canvas_Player.LS_Scale < 0.1f
            ? Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * Vector3.forward
            : Quaternion.Euler(0,CamArm.Degree(),0)*new Vector3(Canvas_Player.LS.x,0,Canvas_Player.LS.y);
    }
    public bool CanAttack(Animator animator)
    {
        if (death || clear) return false;
        return Time.time - Canvas_Player.RB_PressedTime < preInput_Attack;
    }
    public void Attack()
    {
        if (death || clear) return;
        Canvas_Player.RB_PressedTime = -100;
        ChangeState(1);
    }
    public bool CanSkill(Animator animator)
    {
        if (death || clear) return false;
        bool canSkill = Time.unscaledTime - Canvas_Player.SB_PressedTime < preInput_Attack;
        bool skillGauge = Canvas_Player.instance.skillGauge_R.fullCharged;
        if(canSkill && !skillGauge) Canvas_Player.instance.InfoText(Canvas_Player.InfoType.No_SkillGauge);
        return canSkill && skillGauge;
    }
    public void Skill()
    {
        if (death || clear) return;
        Canvas_Player.instance.skillGauge_R.Use();
        Canvas_Player.SB_PressedTime = -100;
        ChangeState(2);
        animator.SetInteger("SkillState",1);
    }
    public bool CanBow()
    {
        if (death || clear) return false;
        bool canBow = Canvas_Player.RS_Scale > 0.1f && Canvas_Player.RB_Pressed;
        bool manaStone = Canvas_Player_World.instance.manaStone > 0;
        if(canBow&&!manaStone) Canvas_Player.instance.InfoText(Canvas_Player.InfoType.No_ManaStone);
        return canBow && manaStone;
    }
    public void Bow()
    {
        if (death || clear) return;
        if (leaning_GlobalRatio < 0.01f)
        {
            if(prefab_weaponL!=null) prefab_weaponL.animator.Play("Off",0,1);
            if(prefab_weaponR!=null) prefab_weaponR.animator.Play("Off",0,1);
            if(prefab_shield!=null) prefab_shield.animator.Play("Off",0,1);
        }
        animator.SetBool("Bow",true);
        Canvas_Player_World.instance.ManaStone_Append(-1);
        ChangeState(7);
    }
    public bool CanStrafe()
    {
        if (death || clear) return false;
        return Canvas_Player.AB_Pressed;
    }

    public void Strafe()
    {
        if (death || clear) return;
        ChangeWeaponData(CurrentWeaponData.Main);
        animator.SetBool("Strafe",true);
        ChangeState(0,false,true);
    }
    public void Shoot()
    {
        if (death || clear) return;
        //CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Shoot);
        Particle_Dash(-0.5f);
    }

    public void DoHit(Vector3 _point,Data_EnemyMotion.SingleAttackData enemyAttack)
    {
        if (death || clear) return;
        //가드브레이크 아닐때
        if (!enemyAttack.isGuardBreak && state!=5)
        {
            //구르기 중
            if (state == 3)
            {
                SuperArmorDamage();
                Particle_SuperArmorHit();
                return;
            }

            //슈퍼아머
            if (IsSuperArmor())
            {
                SuperArmorDamage();
                Particle_SuperArmorHit();
                //CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Hit);
                highlight.HitFX(Manager_Main.instance.mainData.player_RollColor, 0.5f);
                Particle_Smoke();
                return;
            }
            //일반 공격
            else
            {
                Data_EnemyMotion.SingleAttackData.AttackType data = enemyAttack.attackType;
                //즉시 해당 방향으로 회전
                Vector3 lookVec = _point - transform.position;
                lookVec.y = 0;
                Move(transform.position, Quaternion.LookRotation(lookVec));
                //가드중 + 집중 슬롯이 남아있으면 가드,패링
                if (guard && Canvas_Player_World.instance.manaStone > 0)
                {
                    Canvas_Player_World.instance.ManaStone_Append(-1);
                    Particle_Guard(_point);
                    GuardDamage();
                    _guard(_point, data == Data_EnemyMotion.SingleAttackData.AttackType.Strong);
                }
                //아닐경우 히트
                else
                {
                    //피격 처리
                    Damage();
                    if (enemyAttack.attackType != Data_EnemyMotion.SingleAttackData.AttackType.Strong)
                    {
                        Hit_Normal();
                        Particle_Hit_Normal();
                    }
                    else
                    {
                        Hit_Strong();
                        Particle_Hit_Strong();
                    }

                }
            }
        }
        //가드 브레이크
        else
        {
            //구르기로 패턴 회피
            if (CanEvade())
            {
                Particle_GuardBreakEvade();
            }
            //스킬로 Revenge
            else if (CanRevenge())
            {
                Particle_GuardBreakRevenge();
            }
            //피격
            else
            {
                //즉시 해당 방향으로 회전
                Vector3 lookVec = _point - transform.position;
                lookVec.y = 0;
                Move(transform.position, Quaternion.LookRotation(lookVec));
                
                Hit_Strong();
                Particle_GuardBreak();
            }
        }
        
        bool GuardDamage()
        {
            float damage = hitDamage*0.2f;
            if (death || clear) return true;
            Canvas_Player.instance.health.SetValue(Canvas_Player.instance.health.current- damage);
            if (Canvas_Player.instance.health.current < 1)
            {
                Death();
                return true;
            }
            return false;
        }
        bool SuperArmorDamage()
        {
            float damage = hitDamage*0.5f;
            if (death || clear) return true;
            Canvas_Player.instance.health.SetValue(Canvas_Player.instance.health.current- damage);
            if (Canvas_Player.instance.health.current < 1)
            {
                Death();
                return true;
            }
            return false;
        }
        bool Damage()
        {
            float damage = hitDamage;
            if (death || clear) return true;
            Canvas_Player.instance.health.SetValue(Canvas_Player.instance.health.current- damage);
            if (Canvas_Player.instance.health.current < 1)
            {
                Death();
                return true;
            }
            return false;
        }
        //기본 공격 히트, 강공격 히트
        void Hit_Normal()
        {
            if (death || clear) return;
            Cancel();
            //위치 계산
            Vector3 _dist = _point - transform.position;
            _dist.y = 0;
            float dot = Vector3.Dot(transform.forward, _dist.normalized);
            //애니메이션
            animator.SetInteger("HitState",(int)(enemyAttack.attackType));
            ChangeState(4, true);
        }
        void Hit_Strong()
        {
            if (death || clear) return;
            Cancel();
            //무기 놓치기
            if (prefab_weaponL != null) prefab_weaponL.Drop_Player();
            if (prefab_weaponR != null) prefab_weaponR.Drop_Player();
            if (prefab_shield != null) prefab_shield.Drop_Player();
            //애니메이션
            animator.SetInteger("HitState", 0);
            ChangeState(5, true);
            StartCoroutine("C_Smash");
        }
        void _guardbreak()
        {
            if (death || clear) return;
            Cancel();
            Manager_Main.instance.Text_Danger(transform.position,"GuardBreak");
            ChangeState(8);
        }
        void _guard(Vector3 _point,bool strong)
        {
            Canvas_Player.AB_PressedTime = -100;
            guardPoint = _point;
            guarded = true;
            animator.SetBool("GuardBreak",strong);
            animator.SetTrigger("Transition");
        }
    }
    
    public bool IsSuperArmor()
    {
        return isSuperArmor;
    }
    //적 특수공격 회피 가능 여부
    public bool CanRevenge()
    {
        bool value = Time.unscaledTime < skillBeginTime + revengeDelay && isSkill;
        if (value)
        {
            skillBeginTime = -100;
            isRevengeSkill = true;
        }
        return value;
    }
    public bool CanEvade()
    {
        return state==3 && Time.unscaledTime < rollBeginTime + revengeDelay;
    }

    private bool isSuperArmor = false;
    public void SuperArmor(bool value)
    {
        isSuperArmor = value;
        highlight.outlineColor = value ? Manager_Main.instance.mainData.outline_superarmor 
            : Manager_Main.instance.mainData.outline_normal;
    }
    
    private IEnumerator C_Smash()
    {
        float physicsTime = Time.time;
        while (Time.time-physicsTime<0.2f)
        {
            /*
            int jlength = DestructibleObject.destructibleObjects.Count;
            for (int j = jlength-1; j >= 0; j--)
            {
                DestructibleObject destructible = DestructibleObject.destructibleObjects[j];
                Vector3 distVec = transform.position-destructible.transform.GetChild(0).position;
                distVec.y = 0;
                if (Vector3.Magnitude(distVec) < cc.radius + 0.1f+destructible.radius)
                {
                    
                    destructible.Explode(transform.position,false);
                }
            }
            */
            yield return null;
        }
        puppetMaster.mode = PuppetMaster.Mode.Active;
        physicsTime = Time.time;
        while (Time.time-physicsTime<1.0f)
        {
            /*
            int jlength = DestructibleObject.destructibleObjects.Count;
            for (int j = jlength-1; j >= 0; j--)
            {
                DestructibleObject destructible = DestructibleObject.destructibleObjects[j];
                Vector3 distVec = transform.position-destructible.transform.GetChild(0).position;
                distVec.y = 0;
                if (Vector3.Magnitude(distVec) < cc.radius + 0.1f+destructible.radius)
                {
                    
                    destructible.Explode(transform.position,false);
                }
            }
            */
            yield return null;
        }
        
    }


    
    [Button]
    public void Death()
    {
        //카메라 효과
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Death);
        //중요 세팅
        
        particle_target.Activate(false);
        death = true;
        Time.timeScale = 1;
        target = null;
        Deactivate_Pointer();
        SetLeaning(false);
        //
        foreach (var enemy in Enemy.enemies)
        {
            enemy.animator.speed = 0.05f;
        }
        //무기 초기화
        if (prefab_weaponL != null)
        {
            prefab_weaponL.On(true,T_Hand_L);
            prefab_weaponL.charge_Effect.Play();
        }
        if (prefab_weaponR != null)
        {
            prefab_weaponR.On(true,T_Hand_R);
            prefab_weaponR.charge_Effect.Play();
        }
        if (prefab_shield != null)
        {
            if(data_Weapon_Main.useShield)prefab_shield.On(true,T_Shield);
            else
            {
                prefab_shield.transform.parent = T_Back;
                prefab_shield.transform.localPosition = data_Shield.localPos;
                prefab_shield.transform.localRotation = Quaternion.Euler(data_Shield.localRot);
                prefab_shield.transform.localScale = data_Shield.localScale;
                prefab_shield.animator.SetBool("On",true);
            }
            prefab_shield.charge_Effect.Play();
        }
                
        if(prefab_weaponL_SkillL!=null) prefab_weaponL_SkillL.On(false,T_Hand_L);
        if(prefab_weaponR_SkillL!=null) prefab_weaponR_SkillL.On(false,T_Hand_R);
        if(prefab_shield_SkillL!=null) prefab_shield_SkillL.On(false,T_Shield);
                
        if(prefab_weaponL_SkillR!=null) prefab_weaponL_SkillR.On(false,null);
        if(prefab_weaponR_SkillR!=null) prefab_weaponR_SkillR.On(false,null);
        if(prefab_shield_SkillR!=null) prefab_shield_SkillR.On(false,null);
        Cancel();
        highlight.highlighted = false;
        highlight.HitFX(Color.red,2.0f,2.0f);
        //Particles
        Manager_Pooler.instance.GetParticle("Shockwave", transform.position + Vector3.up, Quaternion.identity,1);
        Particle_Smoke();
        //Blood
        GameObject blood = Manager_Blood.instance.Blood_Smash();
        if (blood != null)
        {
            blood.transform.SetPositionAndRotation(transform.position + Vector3.up * 0.8f, transform.rotation);
            blood.SetActive(true);
        }
        
        //마무리
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetInteger("HitState",Random.Range(0,2));
        ChangeState(6);  
        CamArm.instance.DeathFOV();
        Canvas_Player.instance.Activate_Death();
    }
    private void Cancel()
    {
        animator.speed = 1;
        animator.SetBool("Strafe",false);
        CamArm.instance.SpeedLine_Stop();
        
        Canvas_Player.AB_PressedTime = -100;
        Canvas_Player.RB_PressedTime = -100;
        Canvas_Player.SB_PressedTime = -100;
        ChangeWeaponData(CurrentWeaponData.Main);
        prefab_bow.Cancel();
        if (prefab_shield != null) prefab_shield.Trail_Off();
        if (prefab_weaponL != null) prefab_weaponL.Trail_Off();
        if (prefab_weaponR != null) prefab_weaponR.Trail_Off();
        if (prefab_shield_SkillL != null) prefab_shield_SkillL.Trail_Off();
        if (prefab_shield_SkillR != null) prefab_shield_SkillR.Trail_Off();
        if (prefab_weaponL_SkillL != null) prefab_weaponL_SkillL.Trail_Off();
        if (prefab_weaponL_SkillR != null) prefab_weaponL_SkillR.Trail_Off();
        if (prefab_weaponR_SkillL != null) prefab_weaponR_SkillL.Trail_Off();
        if (prefab_weaponR_SkillR != null) prefab_weaponR_SkillR.Trail_Off();
        
        guarded = false;
    }
    public bool CanHit()
    {
        return state != 3 && state != 5;
    }
}
