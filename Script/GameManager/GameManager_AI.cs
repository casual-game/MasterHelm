using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    private void Setting_AI()
    {
        
    }

    private Dictionary<Data_MonsterInfo, AnimatorOverrideController> _aiAnimators =
        new Dictionary<Data_MonsterInfo, AnimatorOverrideController>();

    public void AIAnimators_Add(Data_MonsterInfo info)
    {
        if (_aiAnimators.ContainsKey(info)) return;
        
    }
}
