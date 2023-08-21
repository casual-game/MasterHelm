using System;
using System.Collections;
using System.Collections.Generic;
using DuloGames.UI;
using Pathfinding;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class Enemy : MonoBehaviour
{
	private void State_Setting()
	{
        State_Finish();
	}

	private void State_Disable()
	{
        if(state_current!=null) StopCoroutine(state_current);
	}
	
	[TitleGroup("AI")][FoldoutGroup("AI/State")][ReadOnly] public bool state_enabled = false;
	[TitleGroup("AI")] [FoldoutGroup("AI/State")][ReadOnly] public bool state_danger = false;
	[TitleGroup("AI")][FoldoutGroup("AI/State")][ReadOnly] public NormalState state_NormalState;
	[TitleGroup("AI")][FoldoutGroup("AI/State")][ReadOnly] public DangerState state_DangerState;
	[TitleGroup("AI")][FoldoutGroup("AI/State")][ReadOnly] public float state_beginTime;
	[TitleGroup("AI")][FoldoutGroup("AI/State")][ReadOnly] public float state_finishTime;
	public enum DangerState {Hit=1,Smashed=2,Dead=3};
	public enum	NormalState {Idle=0, Attack=4,Strafe=5,Backstep=6,Run=7}
	public enum TransitionType {Immediately=0,Fast=1,Normal=2,Slow=3}
	private Coroutine state_current=null;
	
	//메인 함수
	public IEnumerator State_Set(Coroutine cstate)
	{
		while (state_danger) yield return null;
		if(state_current!=null) StopCoroutine(state_current);
		state_current = cstate;
		yield return cstate;
	}
	//Animator에서 사용
	public void State_Finish()
	{
		state_enabled = false;
		state_finishTime = Time.time;
		state_danger = false;
	}
	public void State_Idle(TransitionType transition = TransitionType.Fast)
	{
		State_Begin(NormalState.Idle,transition);
	}
	//내부에서 사용
	protected void State_Begin(NormalState normalState,TransitionType transition = TransitionType.Fast)
	{
		if (state_danger) return;
		state_enabled = true;
		state_beginTime = Time.time;
		state_NormalState = normalState;
		int state = (int) normalState;
		switch (transition)
		{
			case TransitionType.Immediately:
				animator.SetTrigger("ChangeState_Immediately");
				break;
			case TransitionType.Fast:
				animator.SetTrigger("ChangeState_Fast");
				break;
			case TransitionType.Normal:
				animator.SetTrigger("ChangeState_Normal");
				break;
			case TransitionType.Slow:
				animator.SetTrigger("ChangeState_Slow");
				break;
		}
		
		animator.SetInteger("State",state);
	}
	protected void State_Begin_Danger(DangerState dangerState,TransitionType transition = TransitionType.Fast)
	{
		state_DangerState = dangerState;
		state_enabled = true;
		state_danger = true;
		int state = (int) dangerState;
		switch (transition)
		{
			case TransitionType.Immediately:
				animator.SetTrigger("ChangeState_Immediately");
				break;
			case TransitionType.Fast:
				animator.SetTrigger("ChangeState_Fast");
				break;
			case TransitionType.Normal:
				animator.SetTrigger("ChangeState_Normal");
				break;
			case TransitionType.Slow:
				animator.SetTrigger("ChangeState_Slow");
				break;
		}

		switch (dangerState)
		{
			case DangerState.Dead:
				Pattern_Cancel();
				break;
		}
		animator.SetInteger("State",state);
	}
	
	//CState
	protected IEnumerator CState_Wait(float period)
	{
		while (true)
		{
			if (state_enabled || state_danger) yield return new WaitForSeconds(period);
			else break;
		}
	}
	protected IEnumerator CState_Chase_Slow(float dist)
	{
		State_Begin(NormalState.Strafe,TransitionType.Normal);
		yield return CPF_StartPath(Manager_Main.instance.mainData.ai_period,transform,Player.instance.transform,dist);
		yield return StartCoroutine(CState_Wait(Manager_Main.instance.mainData.ai_period));
		PF_StopPath();
	}
	protected IEnumerator CState_Chase_Fast(float dist,int num=0)
	{
		animator.SetInteger("Num",num);
		State_Begin(NormalState.Run,TransitionType.Normal);
		yield return CPF_StartPath(Manager_Main.instance.mainData.ai_period,transform,Player.instance.transform,dist);
		yield return StartCoroutine(CState_Wait(Manager_Main.instance.mainData.ai_period));
		PF_StopPath();
	}
	protected IEnumerator CState_Flee_Slow(float dist)
	{
		State_Begin(NormalState.Strafe,TransitionType.Normal);
		yield return CPF_FleePath(dist);
		yield return StartCoroutine(CState_Wait(Manager_Main.instance.mainData.ai_period));
	}
	protected IEnumerator CState_Attack(int num)
	{
		animator.SetInteger("Num",num);
		State_Begin(NormalState.Attack,TransitionType.Fast);
		yield return StartCoroutine(CState_Wait(Manager_Main.instance.mainData.ai_period));
	}
	protected IEnumerator CState_Backstep()
	{
		State_Begin(NormalState.Backstep,TransitionType.Fast);
		yield return StartCoroutine(CState_Wait(Manager_Main.instance.mainData.ai_period));
	}
	
}
