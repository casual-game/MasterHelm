using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "MonsterInfo", menuName = "Data/MonsterInfo", order = 1)]
public class Data_MonsterInfo : ScriptableObject
{
    public int hp = 1000;
    
    [ColorUsage(true,true)] public Color c_hit_begin, c_hit_fin;
    [PropertySpace(16)]
    [Title("몬스터 패턴")]
    [Toggle("usePattern")] public MonsterPattern Pattern_0;
    [Toggle("usePattern")] public MonsterPattern Pattern_1;
    [Toggle("usePattern")] public MonsterPattern Pattern_2;
    [Toggle("usePattern")] public MonsterPattern Pattern_3;
    [Toggle("usePattern")] public MonsterPattern Pattern_4;
    [Toggle("usePattern")] public MonsterPattern Pattern_5;
    [Toggle("usePattern")] public MonsterPattern Pattern_6;
    [Toggle("usePattern")] public MonsterPattern Pattern_7;
}
[System.Serializable]
public class MonsterPattern
{
    //저장용
    public bool usePattern = false;
    [TitleGroup("패턴 이름",subtitle:"인스펙터에서 딕셔너리로 호출하므로, 영어를 사용할 것.")]
    [HideLabel] public string patternName = "PatternName";

    [Title("세부 단계","각각의 세부 단계는 Animator에서의 State를 나타냅니다.")] 
    [LabelText(" N0 종료시점",SdfIconType.CircleFill)][OnValueChanged("Setting")]
    public float pattern_n_0_endratio = 0.75f;
    [LabelText(" State진입 전환시간",SdfIconType.CircleFill),Range(0,4)][OnValueChanged("Setting")]
    public int pattern_n_0_transitionduration = 1;
    [LabelText("애니메이션 클립", SdfIconType.CircleFill)] [OnValueChanged("Setting")]
    public AnimationClip pattern_n_0_clip;
    [ListDrawerSettings(AddCopiesLastElement = true)]
    public List<TrailData_Monster> pattern_n_0 = new List<TrailData_Monster>();
    [LabelText(" N1 종료시점",SdfIconType.CircleFill),PropertySpace(8)]
    public float pattern_n_1_endratio = 0.75f;
    [LabelText(" State진입 전환시간",SdfIconType.CircleFill),Range(0,4)] [OnValueChanged("Setting")]
    public int pattern_n_1_transitionduration = 1;
    [LabelText("애니메이션 클립", SdfIconType.CircleFill)] [OnValueChanged("Setting")]
    public AnimationClip pattern_n_1_clip;
    [ListDrawerSettings(AddCopiesLastElement = true)][OnValueChanged("Setting")]
    public List<TrailData_Monster> pattern_n_1 = new List<TrailData_Monster>();
    [LabelText(" N2 종료시점",SdfIconType.CircleFill),PropertySpace(8)] 
    public float pattern_n_2_endratio = 0.75f;
    [LabelText(" State진입 전환시간",SdfIconType.CircleFill),Range(0,4)] [OnValueChanged("Setting")]
    public int pattern_n_2_transitionduration = 1;
    [LabelText("애니메이션 클립", SdfIconType.CircleFill)] [OnValueChanged("Setting")]
    public AnimationClip pattern_n_2_clip;
    [ListDrawerSettings(AddCopiesLastElement = true)][OnValueChanged("Setting")]
    public List<TrailData_Monster> pattern_n_2 = new List<TrailData_Monster>();
    [LabelText(" N3 종료시점",SdfIconType.CircleFill),PropertySpace(8)] 
    public float pattern_n_3_endratio = 0.75f;
    [LabelText(" State진입 전환시간",SdfIconType.CircleFill),Range(0,4)] [OnValueChanged("Setting")]
    public int pattern_n_3_transitionduration = 1;
    [LabelText("애니메이션 클립", SdfIconType.CircleFill)] [OnValueChanged("Setting")]
    public AnimationClip pattern_n_3_clip;
    [ListDrawerSettings(AddCopiesLastElement = true)][OnValueChanged("Setting")]
    public List<TrailData_Monster> pattern_n_3 = new List<TrailData_Monster>();
    [LabelText(" N4 종료시점",SdfIconType.CircleFill),PropertySpace(8)] 
    public float pattern_n_4_endratio = 0.75f;
    [LabelText(" State진입 전환시간",SdfIconType.CircleFill),Range(0,4)][OnValueChanged("Setting")]
    public int pattern_n_4_transitionduration = 1;
    [LabelText("애니메이션 클립", SdfIconType.CircleFill)] [OnValueChanged("Setting")]
    public AnimationClip pattern_n_4_clip;
    [ListDrawerSettings(AddCopiesLastElement = true)][OnValueChanged("Setting")]
    public List<TrailData_Monster> pattern_n_4 = new List<TrailData_Monster>();
    
    
    
    //실시간
    private List<(float endRatio, int transitionDuration, List<TrailData_Monster> dataMonster)> stateData;
    private (int stateIndex, int trailIndex) statePointer = (0, 0);
    public void Setting()
    {
        stateData = new List<(float endRatio, int transitionDuration, List<TrailData_Monster> dataMonster)>(5);
        if(pattern_n_0.Count>0) stateData.Add((pattern_n_0_endratio, 
            pattern_n_0_transitionduration, pattern_n_0));
        if(pattern_n_1.Count>0) stateData.Add((pattern_n_1_endratio,
            pattern_n_1_transitionduration, pattern_n_1));
        if(pattern_n_2.Count>0) stateData.Add((pattern_n_2_endratio, 
            pattern_n_2_transitionduration, pattern_n_2));
        if(pattern_n_3.Count>0) stateData.Add((pattern_n_3_endratio,
            pattern_n_3_transitionduration, pattern_n_3));
        if(pattern_n_4.Count>0) stateData.Add((pattern_n_4_endratio, 
            pattern_n_4_transitionduration, pattern_n_4));
        
        foreach (var data in stateData)
        {
            foreach (var trailData in data.dataMonster) if(trailData.soundData!=null) SoundManager.Add(trailData.soundData);
        }
    }
    /// <summary>
    /// 각각의 traildata 가리키는 pointer를 다음 순번으로 이동시킵니다.
    /// 만약 전부 이동한 경우 null을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public bool Pointer_Update()
    {
        if (stateData[statePointer.stateIndex].dataMonster.Count-1 <= statePointer.trailIndex)
        {
            if (stateData.Count <= statePointer.stateIndex + 1)
            {
                return false;
            }
            else
            {
                statePointer.stateIndex++;
                statePointer.trailIndex = 0;
            }
        }
        else
        {
            statePointer.trailIndex++;
        }
        return true;
    }
    public void Pointer_Reset()
    {
        statePointer = (0, 0);
    }
    public float Pointer_GetData_EndRatio()
    {
        return stateData[statePointer.stateIndex].endRatio;
    }
    public int Pointer_GetData_TransitionDuration()
    {
        return stateData[statePointer.stateIndex].transitionDuration;
    }
    public TrailData_Monster Pointer_GetData_TrailDataMonster()
    {
        return stateData[statePointer.stateIndex].dataMonster[statePointer.trailIndex];
    }
    public bool Pointer_CompareState(int index)
    {
        return index == statePointer.stateIndex;
    }
}

