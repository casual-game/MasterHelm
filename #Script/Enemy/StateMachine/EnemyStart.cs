using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyStart : BaseStart
{
    public static Dictionary<string, List<EnemyStart>> starts = new Dictionary<string, List<EnemyStart>>();
    public string tag = String.Empty;
    public EnemyRoot enemy;
    public Prefab_Prop weaponL,weaponR,shield;
    public float delay = 0.1f;
    private bool used = false;

    public static void CreateTag(string tag)
    {
        List<EnemyStart> removeList= new List<EnemyStart>();
        if (!starts.ContainsKey(tag)) return;
        foreach (var start in starts[tag])
        {
            start.Setting();
            removeList.Add(start);
        }

        return;
        foreach (var start in removeList)
        {
            starts[tag].Remove(start);
        }
    }
    public void Awake()
    {
        if (!used)
        {
            if(!starts.ContainsKey(tag)) starts.Add(tag,new List<EnemyStart>());
            starts[tag].Add(this);
        }
    }
    public void Setting()
    {
        used = true;
        if (starts.ContainsKey(tag)) starts.Remove(tag);
        StartCoroutine(C_Setting());
    }

    private IEnumerator C_Setting()
    {
        yield return new WaitForSeconds(delay);
        
        EnemyRoot e = Manager_Enemy.instance.GetEnemy(enemy);
        e.transform.SetParent(null);
        e.transform.position = transform.position;
        e.transform.rotation = transform.rotation;
        e.transform.localScale = Vector3.one;
        e.Enable(weaponL,weaponR,shield,0,enemy);
        gameObject.SetActive(false);
        
    }
}
