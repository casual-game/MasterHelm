using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Base : StateMachineBehaviour
{
    public Hero.MoveState moveState;
    public bool useNavPosition = true;
    public bool useUnscaledTime = false;
    public bool useTrail = false;
    public float trailDuration = 0.5f;
    [HideInInspector] public bool isFinished = false;
    [HideInInspector] public bool cleanFinished = false; //isFinished상태에서도 작동하는 일부 사례에서만 쓰입니다. (히트 시 attack계열 작업 종료)
    
    private static TrailData _staticTrailData = null;
    private bool _scriptEntered = false;
    private bool _collisionChecked;
    protected HeroData _heroData;
    protected Hero _hero;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (!_scriptEntered)
        {
            _hero = animator.GetComponent<Hero>();
            _heroData = _hero.heroData;
            _scriptEntered = true;
        }

        _collisionChecked = false;
        isFinished = false;
        cleanFinished = false;
        _hero.Set_AnimBase(this);
        _hero.Set_HeroMoveState(moveState);
        _hero.Get_NavMeshAgent().updatePosition = useNavPosition;
        _hero.Set_AnimatorUnscaledTime(useUnscaledTime);
        if(useTrail) _hero.Tween_Trail(trailDuration);
        animator.speed = 1.0f;
    }

    protected bool IsNotAvailable(Animator animator,AnimatorStateInfo stateInfo)
    {
        bool isNotCurrentState = animator.IsInTransition(0) &&
                              animator.GetNextAnimatorStateInfo(0).shortNameHash != stateInfo.shortNameHash;
        return isNotCurrentState || isFinished;
    }
    protected void Update_Trail(float normalizedTime,Data_WeaponPack weaponPack)
    {
        bool cando = _hero.Get_HeroMoveState() is Hero.MoveState.NormalAttack or Hero.MoveState.StrongAttack;
        if (!cando) return;
        bool trailed = false, collided = false;
        foreach (var trailData in _hero.CurrentAttackMotionData.TrailDatas)
        {
            bool canTrail = !trailed && trailData.trailRange.x <= normalizedTime && normalizedTime < trailData.trailRange.y;
            bool canCollide = !trailData.isHitScan && !collided && 
                              trailData.collisionRange.x <= normalizedTime && normalizedTime < trailData.collisionRange.y;
            //trail 설정
            if (canTrail)
            {
                trailed = true;
                _hero.Equipment_UpdateTrail(weaponPack,trailData.weaponL,trailData.weaponR,trailData.shield);
                if (_staticTrailData != trailData)
                {
                    //trail 정보가 바뀌는 순간에 만약 이전 trail에서 한번도 충돌계산이 없었다면, 1회 진행.
                    if (_hero.CurrentAttackMotionData.TrailDatas.IndexOf(trailData) != 0 && !_collisionChecked)
                    {
                        _hero.Equipment_Collision_Interact(weaponPack,trailData.weaponL,trailData.weaponR,trailData.shield);
                    }
                    //기타 정보들 갱신
                    _collisionChecked = false;
                    _hero.Equipment_Collision_Reset(weaponPack);
                    _staticTrailData = trailData;
                    _hero.Set_CurrentTrail(trailData);
                    
                    
                }
            }
            //충돌했으면 충돌계산
            if (canCollide)
            {
                collided = true;
                _collisionChecked = true;
                _hero.Equipment_Collision_Interact(weaponPack,trailData.weaponL,trailData.weaponR,trailData.shield);
            }
            //모든 충돌범위를 지나왔지만, 마지막 충돌 계산 내역이 없는 경우, 1회 진행.
            else if (normalizedTime > _hero.CurrentAttackMotionData.TrailDatas[^1].collisionRange.y && !_collisionChecked)
            {
                _collisionChecked = true;
                collided = true;
                _hero.Equipment_Collision_Interact(weaponPack,trailData.weaponL,trailData.weaponR,trailData.shield);
            }
        }
        if(!trailed) _hero.Equipment_UpdateTrail(weaponPack,false,false,false);
        if(!collided) _hero.Equipment_Collision_Interact(weaponPack,false,false,false);
    }
    protected void Update_LookDeg(ref Transform lookT,ref float lookF,ref Quaternion lookRot)
    {
        float turnSpeed = _heroData.attackTurnSpeed * Time.deltaTime;
        if (lookT != null)
        {
            Vector3 lookVec = lookT.position - _hero.transform.position;
            lookVec.y = 0;
            lookF = Quaternion.LookRotation(lookVec).eulerAngles.y;
        }
        lookRot = Quaternion.RotateTowards(_hero.transform.rotation, Quaternion.Euler(0,lookF,0), turnSpeed);
    }
    protected void Set_Locomotion(Animator animator)
    {
        _hero.Equipment_UpdateTrail(_hero.weaponPack_Normal,false,false,false);
        _hero.Effect_SuperArmor(false);
        _hero.Set_SpeedRatio(0);
        animator.SetInteger(GameManager.s_state_type, (int)Hero.AnimationState.Locomotion);
        animator.SetTrigger(GameManager.s_state_change);
        _hero.Equipment_Equip(null);
        _hero.Set_AttackIndex(-1);
        isFinished = true;
    }

    protected void Set_Attack_Normal(Animator animator)
    {
        if(animator.GetInteger(GameManager.s_state_type) == (int)Hero.AnimationState.Attack_Strong) 
            _hero.Effect_SuperArmor(false);
        animator.SetInteger(GameManager.s_state_type, (int)Hero.AnimationState.Attack_Normal);
        animator.SetTrigger(GameManager.s_state_change);
    }

    protected void Set_Attack_Strong(Animator animator)
    {
        _hero.Effect_SuperArmor(true);
        animator.SetInteger(GameManager.s_state_type, (int)Hero.AnimationState.Attack_Strong);
        animator.SetTrigger(GameManager.s_state_change);
    }
    protected void Set_LookAt(ref Transform lookT, ref float lookF, bool isFirst)
    {
        float? lookDeg = _hero.Get_LookDeg();
        Transform myT = _hero.transform;
        Vector3 myPos = myT.position;
        //드래그
        if (lookDeg.HasValue)
        {
            //활성화된 적 중 각도가 가장 근접한 적
            Vector3 dragVec = Quaternion.Euler(0,lookDeg.Value,0)*Vector3.forward;
            int? index = null;
            float dist = Mathf.Infinity;
            for (int i = 0; i < Monster.Monsters.Count; i++)
            {
                if(!Monster.Monsters[i].Get_IsAlive()) continue;
                Vector3 monsterVec = Monster.Monsters[i].transform.position - myPos;
                monsterVec.y = 0;
                float _dist = Vector3.Angle(dragVec, monsterVec);
                if (_dist < dist && _dist<45)
                {
                    dist = _dist;
                    index = i;
                }
            }
            //활성화된 적 있을 때
            if (index.HasValue)
            {
                lookT = Monster.Monsters[index.Value].transform;
                Vector3 lookVec = lookT.position - myPos;
                lookVec.y = 0;
                lookF = Quaternion.LookRotation(lookVec).eulerAngles.y;
            }
            //없으면 그냥 원래 드래그 각도
            else
            {
                lookT = null;
                lookF = lookDeg.Value;
            }
        }
        //탭
        else if(isFirst || lookT == null)
        {
            //활성화된 적 중 가장 가까운 적
            int? index = null;
            float dist = Mathf.Infinity;
            
            for (int i = 0; i < Monster.Monsters.Count; i++)
            {
                if(!Monster.Monsters[i].Get_IsAlive()) continue;
                float monsterDist = (myPos - Monster.Monsters[i].transform.position).sqrMagnitude;
                if (monsterDist < dist)
                {
                    dist = monsterDist;
                    index = i;
                }
            }
            //활성화된 적 있을 때
            if (index.HasValue)
            {
                lookT = Monster.Monsters[index.Value].transform;
                Vector3 lookVec = lookT.position - myPos;
                lookVec.y = 0;
                lookF = Quaternion.LookRotation(lookVec).eulerAngles.y;
            }
            //횔성화된 적 없으면 그냥 현 각도
            else 
            {
                lookT = null;
                lookF = _hero.transform.rotation.eulerAngles.y;
            }
        }
    }
    protected void Set_Roll(Animator animator)
    {
        
        //기본
        if (_hero.HeroMoveState == Hero.MoveState.Roll) return;
        _hero.Set_HeroMoveState(Hero.MoveState.Roll);
        _hero.Effect_SuperArmor(false);
        CamArm.instance.Tween_ResetTimescale();
        animator.SetBool(GameManager.s_roll,true);
        //저스트 구르기인지 확인, 저스트 관련 데이터 저장
        bool isJustRoll = false;
        float minDist = _hero.heroData.justEvadeDistance * _hero.heroData.justEvadeDistance;
        Vector3 lookVec = Vector3.zero;
        Monster mtarget = null;
        foreach (var m in Monster.Monsters)
        {
            if (m.Get_CanJustRoll(_hero.heroData.justEvadeFreeTime))
            {
                Vector3 _lookvec = m.transform.position - animator.transform.position; 
                float dist = Vector3.SqrMagnitude(_lookvec);
                if (minDist > dist)
                {
                    mtarget = m;
                    minDist = dist;
                    lookVec = _lookvec;
                    m.Equipment_Collision_Skip();
                }
                isJustRoll = true;
                break;
            }
        }
        if(mtarget!=null) _hero.Set_RoolLookT(mtarget.transform);
        //일반 구르기
        if(!isJustRoll || mtarget == null) animator.SetInteger(GameManager.s_state_type,0);
        //저스트 구르기
        else
        {
            //회전
            lookVec.y = 0;
            animator.transform.rotation = Quaternion.LookRotation(lookVec);
            //기본 설정들
            GameManager.Instance.Combo(GameManager.s_evade);
            _hero.Set_AnimatorUnscaledTime(true);
            CamArm.instance.Tween_JustEvade();
            
            var state = mtarget.Get_CurrentTrail().swingFinState;
            int rollState;
            if (state == PlayerAttackType.LeftState) rollState = 1;
            else rollState = 2;
            animator.SetInteger(GameManager.s_state_type,rollState);
        }
        animator.SetBool(GameManager.s_leftstate,false);
        animator.SetBool(GameManager.s_hit,false);
        animator.ResetTrigger(GameManager.s_state_change);
        _hero.Set_AttackIndex(-1);
        _hero.Equipment_UpdateTrail(_hero.weaponPack_Normal,false,false,false);
        _hero.Equipment_UpdateTrail(_hero.weaponPack_StrongL,false,false,false);
        _hero.Equipment_UpdateTrail(_hero.weaponPack_StrongR,false,false,false);
        _hero.Equipment_Equip(null);
        _hero.Equipment_CancelEffect();
        isFinished = true;
    }
    
}
