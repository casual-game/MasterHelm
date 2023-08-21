using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoot : MonoBehaviour
{
    private EnemyRoot key = null;
    private bool started = false;
    [HideInInspector]public Enemy enemy;

    private Animator animator;

    private Coroutine c_enable = null;
    //Enemy 생성 과정에서는 MANAGER애서 root의 Enable을 호출한다.
    public void Enable(Prefab_Prop weaponL,Prefab_Prop weaponR,Prefab_Prop shield,float delay,EnemyRoot key)
    {
        this.key = key;
        
        if (!started)
        {
            enemy = GetComponentInChildren<Enemy>(true);
            animator = enemy.GetComponent<Animator>();
            started = true;
        }
        //애니메이터 리셋
        animator.ResetTrigger("ChangeState_Slow");
        animator.ResetTrigger("ChangeState_Fast");
        animator.ResetTrigger("ChangeState_Normal");
        animator.ResetTrigger("ChangeState_Immediately");
        animator.ResetTrigger("Hit");
        animator.SetInteger("State",0);
        gameObject.SetActive(true);
        if(c_enable!=null) StopCoroutine(c_enable);
        c_enable = StartCoroutine(C_Enbale(weaponL, weaponR, shield, delay));
    }

    private IEnumerator C_Enbale(Prefab_Prop weaponL,Prefab_Prop weaponR,Prefab_Prop shield,float delay)
    {
        yield return new WaitForSeconds(delay);
        enemy.transform.localPosition = Vector3.zero;
        enemy.transform.localRotation = Quaternion.identity;
        enemy.gameObject.SetActive(true);
        enemy.Setting(weaponL,weaponR,shield);
    }
    //Enemy가 Enemy-Disable을 호출하면 해당 합수 내부에서 Root-Disable을 호출해준다.
    public void Disable()
    {
        if(c_enable!=null) StopCoroutine(c_enable);
        Manager_Enemy.instance.RestoreEnemy(key,this);
        enemy.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
