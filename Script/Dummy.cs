using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public Monster monster;
    public List<Renderer> renderers;

    public void Setting()
    {
        foreach (var r in renderers) r.enabled = false;
    }
    [Button]
    public void Spawn()
    {
        Transform t = transform;
        Monster m = GameManager.Instance.AI_Dequeue(monster.monsterInfo);
        m.Spawn(t.position - m.transform.position, t.rotation).Forget();
    }

}
