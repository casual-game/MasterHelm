using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnim_Base : StateMachineBehaviour
{
    [HideInInspector] public bool isFinished = false;
    protected Monster _monster;
    private bool script_entered = false;
    private bool _collisionChecked;
    private static TrailData _staticTrailData = null;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (!script_entered)
        {
            script_entered = true;
            _monster = animator.GetComponent<Monster>();
        }

        _collisionChecked = false;
        _monster.Set_AnimBase(this);
        isFinished = false;
    }
    protected bool IsNotAvailable(Animator animator, AnimatorStateInfo stateInfo)
    {
        bool isNotCurrentState = animator.IsInTransition(0) &&
                                 animator.GetNextAnimatorStateInfo(0).shortNameHash != stateInfo.shortNameHash;
        return isNotCurrentState || isFinished;
    }
    protected void Update_Trail(float normalizedTime,List<TrailData> trailDatas)
    {
        bool trailed = false, collided = false;

        for (int i = 0; i < trailDatas.Count; i++)
        {
            var trailData = trailDatas[i];
            bool canTrail = !trailed && trailData.trailRange.x <= normalizedTime && normalizedTime < trailData.trailRange.y;
            bool canCollide = !trailData.isHitScan && !collided && 
                              trailData.collisionRange.x <= normalizedTime && normalizedTime < trailData.collisionRange.y;
            //trail 설정
            if (canTrail)
            {
                trailed = true;
                _monster.Equipment_UpdateTrail(trailData.weaponL,trailData.weaponR,trailData.shield);
                if (_staticTrailData != trailData)
                {
                    //trail 정보가 바뀌는 순간에 만약 이전 trail에서 한번도 충돌계산이 없었다면, 1회 진행.
                    if (i != 0 && !_collisionChecked)
                    {
                        _monster.Equipment_Collision_Interact(trailData.weaponL,trailData.weaponR,trailData.shield);
                    }
                    //기타 정보들 갱신
                    _collisionChecked = false;
                    _monster.Equipment_Collision_Reset();
                    _staticTrailData = trailData;
                    _monster.Set_CurrentTrail(trailData);
                    
                    
                }
            }
            //충돌했으면 충돌계산
            if (canCollide)
            {
                collided = true;
                _collisionChecked = true;
                _monster.Equipment_Collision_Interact(trailData.weaponL,trailData.weaponR,trailData.shield);
            }
            //모든 충돌범위를 지나왔지만, 마지막 충돌 계산 내역이 없는 경우, 1회 진행.
            else if (normalizedTime > trailDatas[^1].collisionRange.y && !_collisionChecked)
            {
                _collisionChecked = true;
                collided = true;
                _monster.Equipment_Collision_Interact(trailData.weaponL,trailData.weaponR,trailData.shield);
            }
        }
        if(!trailed) _monster.Equipment_UpdateTrail(false,false,false);
        if(!collided) _monster.Equipment_Collision_Interact(false,false,false);
    }
}
