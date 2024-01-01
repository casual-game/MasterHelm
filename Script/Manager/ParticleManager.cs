using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;
    [TableList(AlwaysExpanded = true,ShowIndexLabels = true)][TitleGroup("수동 추가 파티클")]
    public List<ParticleGroup> particleGroups = new List<ParticleGroup>();

    [TitleGroup("필수 파티클")]
    public ParticleData pd_sparkle, pd_break, pd_smoke, pd_blood_normal, pd_blood_strong, pd_blood_combo;
    
    private Dictionary<ParticleData, ParticleGroup> ingameData = new Dictionary<ParticleData, ParticleGroup>();
    public void Setting()
    {
        foreach (var group in particleGroups)
        {
            group.originalScale = group.particleSystems[0].transform.localScale;
            ingameData.Add(group.particleData, group);
        }
        instance = this;
    }
    public static void Play(ParticleData particleData,Vector3 position,Quaternion rotation,float scale = 1)
    {
        if (!instance.ingameData.ContainsKey(particleData))
        {
            Debug.Log(particleData.name + "가 없습니다.");
            return;
        }
        instance.ingameData[particleData].Play(position,rotation,scale);
    }
    public static void Stop(ParticleData particleData)
    {
        if (!instance.ingameData.ContainsKey(particleData))
        {
            Debug.Log("해당 SoundData가 없습니다.");
            return;
        }
        instance.ingameData[particleData].Stop();
    }
    public static void Add(ParticleData particleData,int count = 1)
    {
        if (particleData == null) return;
        if (instance.ingameData.TryGetValue(particleData, out var pg1))
        {
            GameObject particleGroup = pg1.particleSystems[0].transform.parent.gameObject;
            for (int j = 0; j < count; j++)
            {
                ParticleSystem newp = Instantiate(pg1.particleData.particle, particleGroup.transform);
                newp.gameObject.name = (pg1.poolLength + j + 1) + "_" + pg1.particleData.particle.name;
                pg1.particleSystems.Add(newp);
            }
            
            pg1.poolLength += count;
        }
        else
        {
            var pg = new ParticleGroup();
            pg.poolLength = count;
            pg.particleData = particleData;
            
            GameObject particleGroup = new GameObject(particleData.particle.name);
            particleGroup.transform.parent = instance.transform;
            for (int j = 0; j < pg.poolLength; j++)
            {
                ParticleSystem newp = Instantiate(pg.particleData.particle);
                newp.gameObject.name = (j + 1) + "_" + pg.particleData.particle.name;
                newp.transform.SetParent(particleGroup.transform);
                pg.particleSystems.Add(newp);
            }
            instance.particleGroups.Add(pg);
            instance.ingameData.Add(particleData,pg);
            pg.originalScale = pg.particleSystems[0].transform.localScale;
        }
    }
    [Button]
    public void UpdateManager()
    {
        //모든 데이터 초기화
        while (transform.childCount>0) DestroyImmediate(transform.GetChild(0).gameObject);
        foreach (var pg in particleGroups)
        {
            pg.particleSystems.Clear();
            pg.particleIndex = 0;
        }
        //새로 제작
        for (int i = 0; i < particleGroups.Count; i++)
        {
            if (particleGroups[i].particleData == null)
            {
                Debug.Log("Null인 값이 있습니다!");
                break;
            }
            GameObject particleGroup = new GameObject(particleGroups[i].particleData.particle.name);
            particleGroup.transform.parent = transform;
            for (int j = 0; j < particleGroups[i].poolLength; j++)
            {
                var pg = particleGroups[i];
                ParticleSystem newp = Instantiate(pg.particleData.particle);
                newp.gameObject.name = (j + 1) + "_" + pg.particleData.particle.name;
                newp.transform.SetParent(particleGroup.transform);
                pg.particleSystems.Add(newp);
            }
        }
    }
}
[System.Serializable]
public class ParticleGroup
{
    public ParticleData particleData;
    [MinValue(1)]
    public int poolLength = 1;
    
    [HideInInspector] public Vector3 originalScale;
    [HideInInspector] public List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    [HideInInspector] public int particleIndex = 0;

    public void Play(Vector3 position,Quaternion rotation,float scale = 1)
    {
        particleIndex = (particleIndex + 1) % particleSystems.Count;
        var t = particleSystems[particleIndex].transform;
        t.SetPositionAndRotation(position,rotation);
        t.localScale = originalScale*scale;
        particleSystems[particleIndex].Play();
    }
    public void Stop()
    {
        foreach (var particle in particleSystems)
        {
            if(particle.isPlaying) particle.Stop(true);
        }
    }
}