using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnim_Attack : MonsterAnim_Base
{
    public int index;
    
    private float _endRatio;
    private MonsterPattern _pattern;//OnStateEnter(...)에서 설정
    private TrailData_Monster _trailData;//Setting_Data(...) 에서 설정
    private float _rotateDuration, _moveSpeed,_playSpeed;//Setting_Data(...) 에서 설정
    private bool _rotateToHero;//Setting_Data(...) 에서 설정
    private bool _toIdle,_finishState;//Update_Trail(...) 에서 설정.
    
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _monster.Set_HitState(Monster.HitState.Ground);

        _pattern = _monster.Get_CurrentMonsterPattern();
        _endRatio = _pattern.Pointer_GetData_EndRatio();
        _finishState = false;
        Setting_Data(animator,true);
        _monster.Voice_Attack();
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator, stateInfo)) return;
        float normalizedTime = stateInfo.normalizedTime;
        Transform t = _monster.transform;
        
        Update_Trail(normalizedTime,animator);
        bool overRatio = _endRatio < stateInfo.normalizedTime;
        Update_Movement(t,overRatio,animator);
        Update_Finish(overRatio,animator);
    }
    void Setting_Data(Animator animator,bool updateTrail)
    {
        if (updateTrail)
        {
            _trailData = _pattern.Pointer_GetData_TrailDataMonster();
            _monster.Set_CurrentTrail(_trailData);
        }
        _moveSpeed = _trailData.moveSpeed;
        _rotateDuration = _trailData.rotateDuration;
        _rotateToHero = _trailData.rotateToHero;
        _playSpeed = _trailData.playSpeed;
        animator.SetFloat(GameManager.s_speed,_playSpeed);
    }
    void Update_Trail(float normalizedTime,Animator animator)
    {
        if (_monster.Get_MonsterMoveState() != Monster.MoveState.Pattern) return;
        bool trailed = false, collided = false;
        var pattern = _monster.Get_CurrentMonsterPattern();
        bool canTrail = _trailData.trailRange.x <= normalizedTime && normalizedTime < _trailData.trailRange.y;
        bool canCollide = !_trailData.isHitScan && _trailData.collisionRange.x <= normalizedTime 
                                                && normalizedTime < _trailData.collisionRange.y;
        //trail 설정
        if (canTrail)
        {
            trailed = true;
            _monster.Equipment_UpdateTrail(_trailData.weaponL,_trailData.weaponR,_trailData.shield);
        }
        //충돌했으면 충돌계산
        if (canCollide)
        {
            collided = true;
            _monster.Equipment_Collision_Interact(_trailData.weaponL,_trailData.weaponR,_trailData.shield);
        }
        //종료 처리. 다음 trail로 pattern의 포인터 이동
        if (!_finishState && pattern.Pointer_GetData_TrailDataMonster() == _trailData 
                          && normalizedTime > _trailData.trailRange.y)
        {
            if (!collided)
            {
                _monster.Equipment_Collision_Interact(_trailData.weaponL,_trailData.weaponR,_trailData.shield);
                _monster.Equipment_Collision_Reset();
            }
            
            _toIdle = !pattern.Pointer_Update();
            if (!_toIdle) Setting_Data(animator,pattern.Pointer_CompareState(index));
            else _finishState = true;
        }
        if(!trailed) _monster.Equipment_UpdateTrail(false,false,false);
        if(!collided) _monster.Equipment_Collision_Interact(false,false,false);
    }
    void Update_Movement(in Transform t,in bool overRatio,in Animator animator)
    {
        Quaternion targetRot;
        Vector3 targetPos,distVec = t.position - Hero.instance.transform.position;
        distVec.y = 0;
        bool tooClose = distVec.sqrMagnitude < 4;
        if (tooClose) targetPos = Vector3.zero;
        else targetPos = animator.deltaPosition * _moveSpeed;
        if (!overRatio && _rotateToHero)
        {
            float targetDeg;
            float playerDeg = -90 - Mathf.Atan2(distVec.z, distVec.x) * Mathf.Rad2Deg;
            float degDiff = -t.rotation.eulerAngles.y + playerDeg;
            while (degDiff < -180) degDiff += 360;
            while (degDiff > 180) degDiff -= 360;
            degDiff = Mathf.Clamp(degDiff, -60, 60) / 60.0f;
            targetDeg = Mathf.SmoothDampAngle(t.eulerAngles.y, playerDeg,
                ref _monster.rotateCurrentVelocity, _rotateDuration * Mathf.Abs(degDiff));
            targetRot = Quaternion.Euler(0, targetDeg, 0);
        }
        else targetRot = animator.rootRotation;

        _monster.transform.rotation = targetRot;
        _monster.Move_Nav(targetPos);
    }
    void Update_Finish(in bool overRatio,in Animator animator)
    {
        if (overRatio && _toIdle)
        {
            animator.SetInteger(GameManager.s_state_type,0);
            animator.SetInteger(GameManager.s_125ms,-1);
            animator.SetTrigger(GameManager.s_transition);
            _pattern.Pointer_Reset();
            isFinished = true;
        }
        else if(overRatio)
        {
            animator.SetInteger(GameManager.s_125ms,_pattern.Pointer_GetData_TransitionDuration());
            animator.SetTrigger(GameManager.s_transition);
            isFinished = true;
        }
    }
    
    
}
