using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [TitleGroup("메인 설정 인스펙터")]
    [TabGroup("메인 설정 인스펙터/AreaUI", "기본 설정", SdfIconType.Gear)]
    public AnimatorOverrideController baseOverride_Monster,baseOverride_Boss;
    
    private Dictionary<Data_MonsterInfo, AnimatorOverrideController> _aiAnimators = new Dictionary<Data_MonsterInfo, AnimatorOverrideController>();
    private Dictionary<Data_MonsterInfo, Transform> folders = new Dictionary<Data_MonsterInfo, Transform>();
    [ShowInInspector]
    private Dictionary<Data_MonsterInfo, Queue<Monster>> pools = new Dictionary<Data_MonsterInfo, Queue<Monster>>();
    private Dictionary<Data_MonsterInfo, List<Monster>> createPools = new Dictionary<Data_MonsterInfo, List<Monster>>();
    private void Setting_AI()
    {
        Transform monsterFolder = transform.Find("Pool_AI");
        string pattern_ = "Pattern-", bar = "-";
        //더미 내부의 데이터 수집, 저장.
        foreach (var dummy in FindObjectsOfType<Dummy>())
        {
            dummy.Setting();
            var monster = dummy.monster;
            if (!_aiAnimators.ContainsKey(monster.monsterInfo))
            {
                AnimatorOverrideController overrideController;
                if (monster is Monster_Boss)
                {
                    overrideController = new AnimatorOverrideController(baseOverride_Boss);
                }
                else
                {
                    var norm = (Monster_Normal)monster;
                    overrideController = new AnimatorOverrideController(baseOverride_Monster);
                    overrideController[s_deathflip] = norm.animDeathFlip;
                    overrideController[s_deathnorm] = norm.animDeathNorm;
                    overrideController[s_hitstrong1] = norm.animHitStrong1;
                    overrideController[s_hitstrong2] = norm.animHitStrong2;
                    overrideController[s_idle] = norm.animIdle;
                    overrideController[s_smashbegin] = norm.animSmashBegin;
                    overrideController[s_smashloop] = norm.animSmashLoop;
                    overrideController[s_smashfin] = norm.animSmashFin;
                    if(norm.animSpawnGround == null) overrideController[s_spawn] = norm.animSpawnAir;
                    else overrideController[s_spawn] = Random.Range(0, 2) == 1 ? norm.animSpawnAir : norm.animSpawnGround;
                    overrideController[s_stunbegin] = norm.animStunBegin;
                    overrideController[s_stunloop] = norm.animStunLoop;
                    overrideController[s_stunfin] = norm.animStunFin;
                    overrideController[s_run] = norm.animRun;
                    overrideController[s_strafefwd] = norm.animStrafeFwd;
                    overrideController[s_strafeleft] = norm.animStrafeLeft;
                    overrideController[s_straferight] = norm.animStrafeRight;
                }
                ApplyPatternAnim(monster.monsterInfo.Pattern_0,0);
                ApplyPatternAnim(monster.monsterInfo.Pattern_1,1);
                ApplyPatternAnim(monster.monsterInfo.Pattern_2,2);
                ApplyPatternAnim(monster.monsterInfo.Pattern_3,3);
                ApplyPatternAnim(monster.monsterInfo.Pattern_4,4);
                ApplyPatternAnim(monster.monsterInfo.Pattern_5,5);
                ApplyPatternAnim(monster.monsterInfo.Pattern_6,6);
                ApplyPatternAnim(monster.monsterInfo.Pattern_7,7);
                void ApplyPatternAnim(MonsterPattern pattern, int index)
                {
                    if (pattern.usePattern)
                    {
                        string _name = pattern_ + index + bar + 0;
                        overrideController[_name] = pattern.pattern_n_0_clip;
                        _name = pattern_ + index + bar + 1;
                        overrideController[_name] = pattern.pattern_n_1_clip;
                        _name = pattern_ + index + bar + 2;
                        overrideController[_name] = pattern.pattern_n_2_clip;
                        _name = pattern_ + index + bar + 3;
                        overrideController[_name] = pattern.pattern_n_3_clip;
                        _name = pattern_ + index + bar + 4;
                        overrideController[_name] = pattern.pattern_n_4_clip;
                    }
                }
                //애니메이션 업데이트
                _aiAnimators.Add(monster.monsterInfo, overrideController);
            }
            
            //새로운 종류의 몬스터면 폴더 생성, 풀 데이터 초기화
            if (!pools.ContainsKey(monster.monsterInfo))
            {
                var folder = (new GameObject(monster.monsterInfo.name)).transform;
                folder.transform.SetParent(monsterFolder);
                folders.Add(monster.monsterInfo,folder);
                pools.Add(monster.monsterInfo,new Queue<Monster>());
                createPools.Add(monster.monsterInfo,new List<Monster>());
            }
            //몬스터 생성,풀에 넣기
            Monster m = Instantiate(monster,folders[monster.monsterInfo]);
            m.Setting_Monster(_aiAnimators[monster.monsterInfo]);
            pools[monster.monsterInfo].Enqueue(m);
            if (!createPools[monster.monsterInfo].Contains(monster)) createPools[monster.monsterInfo].Add(monster);
        }
        
        //디버그용 적 바로 생성하는 코드
        #if UNITY_EDITOR
        foreach (var dummy in FindObjectsOfType<Dummy>()) dummy.Spawn();
        #endif
    }

    public Monster AI_Dequeue(Data_MonsterInfo monsterInfo)
    {
        if (pools[monsterInfo].Count == 0)
        {
            Monster targetPrefab = createPools[monsterInfo][Random.Range(0, createPools[monsterInfo].Count)];
            Monster m = Instantiate(targetPrefab,folders[monsterInfo]);
            m.Setting_Monster(_aiAnimators[monsterInfo]);
            return m;
        }
        else
        {
            return pools[monsterInfo].Dequeue();
        }
    }
    public void AI_Enqueue(Monster monster)
    {
        pools[monster.monsterInfo].Enqueue(monster);
    }
    
    
}
