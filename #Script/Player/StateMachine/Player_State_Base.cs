using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player_State_Base : StateMachineBehaviour
{
    [HideInInspector] public bool finished = false;
    protected Player player=null;
    [LabelText("STATE 넘버 기입",true)] public int targetState = 0;
    public bool canChargeMana = false,enforcePointer = true;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (player == null) player = animator.GetComponent<Player>();
        player.stateMachineBahavior = this;
        player.UpdateLeaning(false,0);
        
        if (player.state != targetState) finished = true;
        else finished = false;
        player.SuperArmor(false);
        player.isRevengeSkillActivated = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player.state != targetState) finished = true;
        if (finished) return;
        
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (enforcePointer) player.particle_target.Enforce_Renew();
        if(!canChargeMana) Canvas_Player_World.instance.ManaStone_DelayCharge();
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player.state != targetState) finished = true;
        if (finished) return;
        base.OnStateMove(animator, stateInfo, layerIndex);
    }
    protected bool PERCENT(int _0_100)
    {
        int rd = Random.Range(0,100);
        return _0_100 >= rd;
    }
    public bool PreInput(Animator animator)
    {
        if (player.death || player.clear) return false;
        if (player.CanSkill(animator))
        {
            finished = true;
            player.Skill();
            return true;
        }
        else if (player.CanRoll())
        {
            finished = true;
            player.Roll();
            return true;
        }
        else if (player.CanAttack(animator))
        {
            finished = true;
            player.Attack();
            return true;
        }

        return false;
    }
    public bool IsCloseToTarget()
    {
        if (player.target == null) return false;
        else
        {
            Vector3 dist = player.target.transform.position - player.transform.position;
            dist.y = 0;
            float limit = player.cc.radius + player.attackStopDist + player.target.cc.radius;
            return dist.sqrMagnitude < limit * limit;
        }
    }

    public float ShootDeg()
    {
        float deg;
        if (Canvas_Player.RS_Scale > 0.1f)
        {
            deg = Quaternion.LookRotation(new Vector3(Canvas_Player.RS.x, 0, Canvas_Player.RS.y)).eulerAngles.y
                  + CamArm.Degree();
        }
        else if (player.target != null)
        {
            deg = Quaternion.LookRotation(player.target.transform.position-player.transform.position).eulerAngles.y;
        }
        else if (Canvas_Player.LS_Scale > 0.1f)
        {
            deg = Quaternion.LookRotation(new Vector3(Canvas_Player.LS.x, 0, Canvas_Player.LS.y)).eulerAngles.y
                  + CamArm.Degree();
        }
        else
        {
            deg = player.transform.rotation.eulerAngles.y;
        }
        return deg;
    }
}
