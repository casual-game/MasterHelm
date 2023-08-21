using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Tutorial : MonoBehaviour
{
    private TMP_Text tmp;
    private Animator anim;
    private SingleTutorial currentTutorial = null;
    public List<SingleTutorial> singleTutorials = new List<SingleTutorial>();
    // Start is called before the first frame update
    public void Setting()
    {
	    tmp = GetComponentInChildren<TMP_Text>(true);
	    anim = GetComponentInChildren<Animator>(true);
	    StartCoroutine(C_Tutorial());
    }
	
    public void StartText(string text)
    {
	    tmp.text = text;
	    anim.Play("CreateText",0,0.25f);
    }
    public void StartQuest(string text)
    {
	    tmp.text = text;
	    anim.Play("CreateQuest",0,0.25f);
    }
    [Button]
    public void ClearQuest()
    {
	    if (currentTutorial != null)
	    {
		    currentTutorial.cleared = true;
	    }
    }
    public void ClearText()
    {
	    if (currentTutorial != null && currentTutorial.canInput)
	    {
		    currentTutorial.cleared = true;
	    }
    }
    private IEnumerator C_Tutorial()
    {
	    StartCoroutine(C_Buff_Skill());
	    StartCoroutine(C_Buff_Stone());
	    anim.speed = 0;
	    yield return new WaitForSeconds(2.75f);
	    anim.speed = 1;
	    foreach (var tutorial in singleTutorials)
	    {
		    currentTutorial = tutorial;
		    if (tutorial.isQuest)
		    {
			    StartQuest(tutorial.text);
			    while (!tutorial.cleared)
			    {
				    tutorial.check.Invoke();
				    yield return null;
			    }
			    anim.Play("NextQuest",0,0.0f);
			    yield return new WaitForSeconds(1.0f);
		    }
		    else
		    {
			    StartText(tutorial.text);
			    yield return new WaitForSeconds(1.0f);
			    if (currentTutorial == singleTutorials[singleTutorials.Count - 1])
				    yield return new WaitForSeconds(1.0f);
			    tutorial.canInput = true;
			    while (!tutorial.cleared) yield return null;
			    anim.Play("NextText",0,0.25f);
		    }

		    yield return new WaitForSecondsRealtime(0.75f);
	    }
	    Manager_Main.instance.NextScene();
    }

    private float skill_left, skill_right;
    private int skill_count = 0;
    private IEnumerator C_Buff_Skill()
    {
	    while (true)
	    {
		    if (skill_left == Canvas_Player.instance.skillGauge_L.current
		        && skill_right == Canvas_Player.instance.skillGauge_R.current)
		    {
			    skill_count++;
		    }
		    else
		    {
			    skill_left = Canvas_Player.instance.skillGauge_L.current;
			    skill_right = Canvas_Player.instance.skillGauge_R.current;  
		    }

		    if (skill_count > 7 && (Canvas_Player.instance.skillGauge_L.current<99 
		                            || Canvas_Player.instance.skillGauge_R.current<99))
		    {
			    Canvas_Player.instance.skillGauge_L.SetValue(100);
			    Canvas_Player.instance.skillGauge_R.SetValue(100);
		    }
		    yield return new WaitForSeconds(0.25f);
	    }
    }

    private int bullet=0;
    private int bullet_count=0;
    private IEnumerator C_Buff_Stone()
    {
	    while (true)
	    {
		    if (Canvas_Player_World.instance.manaStone == bullet) bullet_count++;
		    else bullet = Canvas_Player_World.instance.manaStone;

		    if (bullet_count > 7 && bullet<3)
		    {
			    Canvas_Player_World.instance.ManaStone_Append(3);
		    }
		    yield return new WaitForSeconds(0.25f);
	    }
    }
    public void QUEST_1_1()
    {
	    if (Player.instance.isStrong && !Player.instance.isSkill)
	    {
		    Player.instance.isStrong = false;
		    ClearQuest();
	    }
    }
    public void QUEST_1_2()
    {
	    if (Player.instance.isStrong && !Player.instance.isSkill && Player.instance.target != null
	        && Player.instance.target.state_danger && !Player.instance.animator.GetBool("Charge"))
	    {
		    ClearQuest();
		    Player.instance.isStrong = false;
	    }
    }
    public void QUEST_2()
    {
	    if (Player.instance.target != null && Player.instance.target.state_danger
	                                       && Player.instance.animator.GetBool("Charge"))
	    {
		    ClearQuest();
		    Player.instance.isStrong = false;
	    }
    }
    public void QUEST_3()
    {
	    if(Player.instance.guard) ClearQuest();
    }
    public void QUEST_4()
    {
	    if(Player.instance.state == 3) ClearQuest();
    }
    public void QUEST_5()
    {
	    if(Player.instance.state == 7) ClearQuest();
    }
    public void QUEST_6_1()
    {
	    if(Player.instance.state == 2 && Player.instance.isLeftSkill.HasValue && !Player.instance.isLeftSkill.Value) ClearQuest();
    }
    public void QUEST_6_2()
    {
	    if(Player.instance.state == 2 && Player.instance.isLeftSkill.HasValue && Player.instance.isLeftSkill.Value) ClearQuest();
    }
}
[System.Serializable]
public class SingleTutorial
{
	public bool isQuest = false;
	[TextArea]
	public string text;

	[HideInInspector] public bool cleared = false,canInput = false;
	[ShowIf("IsNotQuest")]public float textDuration = 3.5f;
	[ShowIf("IsQuest")]public UnityEvent check;

	public bool IsQuest()
	{
		return isQuest;
	}
	public bool IsNotQuest()
	{
		return !isQuest;
	}
}

