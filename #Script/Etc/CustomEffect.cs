using System;
using System.Collections;
using System.Collections.Generic;
using MagicLightProbes;
using Micosmo.SensorToolkit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CustomEffect : MonoBehaviour
{
    //Editor------------------------------------------------------------------------------------------------------------
    [OnValueChanged("UpdateEffect")]
    [ListDrawerSettings(CustomAddFunction = "AddEffect",CustomRemoveIndexFunction = "RemoveEffectIndex")]
    public List<Effect> effects = new List<Effect>();
    [System.Serializable]
    public class Effect
    {
        [FoldoutGroup("$TitleName")] public string tag;
        [FoldoutGroup("$TitleName")] public Data_Audio audio;
        [FoldoutGroup("$TitleName")] public ParticleSystem particle;
        [ReadOnly]
        [FoldoutGroup("$TitleName")] public Transform createRoot;
        [ReadOnly]
        [FoldoutGroup("$TitleName")] public RaySensor raySensor;
        
        public string TitleName()
        {
            if (particle != null) return tag+" ("+particle.gameObject.name+")";
            else if (createRoot != null) return tag+" ("+createRoot.parent.gameObject.name+")";
            else return "NULL";
        }
    }
    private void AddEffect()
    {
        effects.Add(new Effect());
        
        GameObject newG = new GameObject("Effect_" + transform.childCount);
        newG.transform.SetParent(transform);
        newG.transform.localPosition = Vector3.zero;
        newG.transform.localRotation = Quaternion.identity;
        newG.transform.localScale = Vector3.one;

        GameObject effectT = new GameObject("effectT");
        effectT.transform.SetParent(newG.transform);
        effectT.transform.localPosition = Vector3.zero;
        effectT.transform.localRotation = Quaternion.identity;
        effectT.transform.localScale = Vector3.one;
        
        GameObject sensorT = new GameObject("sensorT");
        sensorT.transform.SetParent(newG.transform);
        sensorT.transform.localPosition = Vector3.zero;
        sensorT.transform.localRotation = Quaternion.identity;
        sensorT.transform.localScale = Vector3.one;
        RaySensor sensor = sensorT.AddComponent<RaySensor>();
        UpdateEffect();
    }
    private void RemoveEffectIndex(int index)
    {
        effects.RemoveAt(index);
        
        DestroyImmediate(transform.GetChild(index).gameObject);
        
        UpdateEffect();
    }
    private void UpdateEffect()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            GameObject root = transform.GetChild(i).gameObject;
            if (effects[i].tag != String.Empty) root.name = effects[i].tag;
            else root.name = "Effect_" + i;

            if (root.transform.childCount == 0)
            {
                CreateEffectT(root.transform,effects[i]);
                CreateSensorT(root.transform,effects[i]);
            }
            else if (root.transform.childCount == 1)
            {
                CreateSensorT(root.transform,effects[i]);
            }
            else if (root.transform.childCount == 2)
            {
                effects[i].createRoot = root.transform.GetChild(0);
                effects[i].raySensor = root.transform.GetChild(1).GetComponent<RaySensor>();
                effects[i].raySensor.transform.localPosition = Vector3.up;
            }
            void CreateEffectT(Transform parent,Effect effect)
            {
                GameObject effectT = new GameObject("effectT");
                effectT.transform.SetParent(parent);
                effectT.transform.localPosition = Vector3.zero;
                effectT.transform.localRotation = Quaternion.identity;
                effectT.transform.localScale = Vector3.one;

                effect.createRoot = effectT.transform;
            }
            void CreateSensorT(Transform parent,Effect effect)
            {
                GameObject sensorT = new GameObject("sensorT");
                sensorT.transform.SetParent(parent);
                sensorT.transform.localPosition = Vector3.zero;
                sensorT.transform.localRotation = Quaternion.identity;
                sensorT.transform.localScale = Vector3.one;
                RaySensor sensor = sensorT.AddComponent<RaySensor>();

                effect.raySensor = sensor;
            }
        }
    }

    //Ingame------------------------------------------------------------------------------------------------------------
    private Dictionary<string, Effect> effectDic = new Dictionary<string, Effect>();
    private Transform parent;
    public void Setting(Transform folderT)
    {
        foreach (var e in effects) effectDic.Add(e.tag, e);
        parent = transform.parent;
        transform.SetParent(folderT);
        foreach (var effect in effects)
        {
            if(effect.audio!=null) SoundManager.instance.Add(effect.audio);
        }
    }
    public void PlayParticle(string tag)
    {
        transform.SetPositionAndRotation(parent.position,parent.rotation);
        
        Effect e = effectDic[tag];
        ParticleSystem p = e.particle;
        Transform t = p.transform;
        
        t.SetParent(e.createRoot);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
        
        p.Play();
    }
    public void PlaySound(string tag)
    {
        Effect e = effectDic[tag];
        if(e.audio!=null) SoundManager.instance.Play(e.audio,1);
    }
    public void Detect(string tag,Vector3 point,Data_EnemyMotion.SingleAttackData attackData)
    {
        Effect e = effectDic[tag];
        e.raySensor.Pulse();
        bool detectPlayer = Player.instance.gameObject == e.raySensor.GetNearestDetection();
        if(detectPlayer) Player.instance.DoHit(point,attackData);
        else print("PD");
    }
}
