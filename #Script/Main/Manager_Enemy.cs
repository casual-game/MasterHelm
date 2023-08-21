using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Manager_Enemy : MonoBehaviour
{
    public static Manager_Enemy instance;
    [ShowInInspector]
    public Dictionary<Prefab_Prop, Queue<Prefab_Prop>> Props = new Dictionary<Prefab_Prop, Queue<Prefab_Prop>>();
    [ShowInInspector]
    public Dictionary<EnemyRoot, Queue<EnemyRoot>> Enemies = new Dictionary<EnemyRoot, Queue<EnemyRoot>>();

    private Transform folder_enemy, folder_prop;
    
    public void Setting()
    {
        instance = this;
        Transform _root_ = transform.Find("_manager_enemy_");
        folder_enemy = _root_.Find("_enemy_");
        folder_prop = _root_.Find("_prop_");
    }

    [Button]
    public void TestTest()
    {
        EnemyStart.CreateTag(String.Empty);
    }

    public void AddProp(Prefab_Prop key)
    {
        if(!Props.ContainsKey(key)) Props.Add(key,new Queue<Prefab_Prop>());
        Prefab_Prop p = Instantiate(key,folder_prop);
        p.key = key;
        p.gameObject.SetActive(false);
        Props[key].Enqueue(p);
    }

    public void AddEnemy(EnemyRoot key)
    {
        if(!Enemies.ContainsKey(key)) Enemies.Add(key,new Queue<EnemyRoot>());
        //if(key.gameObject.activeSelf) key.gameObject.SetActive(false);
        EnemyRoot e = Instantiate(key,folder_enemy);
        e.gameObject.SetActive(false);
        e.GetComponentInChildren<Enemy>(true).gameObject.SetActive(false);
        Enemies[key].Enqueue(e);
    }
    public Prefab_Prop GetProp(Prefab_Prop key)
    {
        if(!Props.ContainsKey(key)) Props.Add(key,new Queue<Prefab_Prop>());
        if (Props[key].Count == 0)
        {
            Prefab_Prop p = Instantiate(key);
            p.key = key;
            p.gameObject.SetActive(false);
            Props[key].Enqueue(p);
        }
        return Props[key].Dequeue();
    }
    public EnemyRoot GetEnemy(EnemyRoot key)
    {
        if(!Enemies.ContainsKey(key)) Enemies.Add(key,new Queue<EnemyRoot>());
        if (Enemies[key].Count == 0)
        {
            EnemyRoot e = Instantiate(key);
            e.gameObject.SetActive(false);
            e.GetComponentInChildren<Enemy>(true).gameObject.SetActive(false);
            Enemies[key].Enqueue(e);
        }
        return Enemies[key].Dequeue();
    }
    public void RestoreProp(Prefab_Prop key,Prefab_Prop prop)
    {
        prop.gameObject.SetActive(false);
        prop.transform.SetParent(folder_prop);
        Props[key].Enqueue(prop);
    }
    public void RestoreEnemy(EnemyRoot key,EnemyRoot enemy)
    {
        //enemy.gameObject.SetActive(false);
        enemy.transform.SetParent(folder_enemy);
        Enemies[key].Enqueue(enemy);
    }
}
