using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Base : StateMachineBehaviour
{
    public Hero.MoveState moveState;
    public bool useNavPosition = true;
    public bool useTrail = false;
    private bool script_entered = false;
    private static TrailData static_trailData = null;
    protected HeroData hero;
    protected Hero movement;
    [HideInInspector] public bool isFinished = false;
    [HideInInspector] public bool cleanFinished = false; //isFinished상태에서도 작동하는 일부 사례에서만 쓰입니다. (히트 시 attack계열 작업 종료)
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (!script_entered)
        {
            movement = animator.GetComponent<Hero>();
            hero = movement.heroData;
            script_entered = true;
        }
        
        isFinished = false;
        cleanFinished = false;
        movement.Set_AnimBase(this);
        movement.Set_HeroMoveState(moveState);
        movement.Get_NavMeshAgent().updatePosition = useNavPosition;
        movement.trailEffect.active = useTrail;
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
        bool trailed = false, collided = false;
        foreach (var trailData in movement.CurrentAttackMotionData.TrailDatas)
        {
            bool canTrail = !trailed && trailData.trailRange.x <= normalizedTime && normalizedTime < trailData.trailRange.y;
            bool canCollide = !trailData.isHitScan && !collided && 
                              trailData.collisionRange.x <= normalizedTime && normalizedTime < trailData.collisionRange.y;
            //trail 설정
            if (canTrail)
            {
                trailed = true;
                movement.Equipment_UpdateTrail(weaponPack,trailData.weaponL,trailData.weaponR,trailData.shield);
                if (static_trailData != trailData)
                {
                    static_trailData = trailData;
                    movement.Set_CurrentTrail(trailData);
                    movement.Equipment_Collision_Reset(weaponPack);
                }
            }
            //충돌 설정
            if (canCollide)
            {
                collided = true;
                movement.Equipment_Collision_Interact(weaponPack,trailData.weaponL,trailData.weaponR,trailData.shield);
            }
        }
        if(!trailed) movement.Equipment_UpdateTrail(weaponPack,false,false,false);
        if(!collided) movement.Equipment_Collision_Interact(weaponPack,false,false,false);
    }
    protected void Update_LookDeg(ref Transform lookT,ref float lookF,ref Quaternion lookRot)
    {
        float turnSpeed = hero.attackTurnSpeed * Time.deltaTime;
        if (lookT != null)
        {
            Vector3 lookVec = lookT.position - movement.transform.position;
            lookVec.y = 0;
            lookF = Quaternion.LookRotation(lookVec).eulerAngles.y;
        }
        lookRot = Quaternion.RotateTowards(movement.transform.rotation, Quaternion.Euler(0,lookF,0), turnSpeed);
    }
    protected void Set_Locomotion()
    {
        movement.Equipment_UpdateTrail(movement.weaponPack_Normal,false,false,false);
        movement.Set_AnimationState(Hero.AnimationState.Locomotion);
        movement.Equipment_Equip(null);
        movement.Set_AttackIndex(-1);
        isFinished = true;
    }
    protected void Set_LookAt(ref Transform lookT, ref float lookF, bool isFirst)
    {
        float? lookDeg = movement.Get_LookDeg();
        Transform myT = movement.transform;
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
                lookF = movement.transform.rotation.eulerAngles.y;
            }
        }
    }

    
}
