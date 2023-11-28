using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "MonsterInfo", menuName = "Data/MonsterInfo", order = 1)]
public class Data_MonsterInfo : ScriptableObject
{
    public int hp = 1000;
    [ListDrawerSettings(AddCopiesLastElement = true)] public List<MonsterPattern> patterns;
    [ColorUsage(true,true)] public Color c_hit_begin, c_hit_fin;
    
}
[System.Serializable]
public class MonsterPattern
{
    public string patternName = "PatternName";
    public float playSpeed = 1.0f;
    public float motionSpeed = 0.75f;
    public float endRatio = 0.75f;
    public float rotateDuration = 1.0f;
    [MinMaxSlider(0,1,true)] public Vector2 rotateRange = new Vector2(0, 1);
    [ListDrawerSettings(AddCopiesLastElement = true)] public List<TrailData_Monster> trailDatas = new List<TrailData_Monster>();
}

