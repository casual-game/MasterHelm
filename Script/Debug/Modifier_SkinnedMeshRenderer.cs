using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
public class Modifier_SkinnedMeshRenderer : MonoBehaviour
{
    [Button][InfoBox("모든 Child의 SkinnedMeshRenderer.bones에서 null인 인덱스들을 전부 삭제합니다.")]
    public void Modify_Bones()
    {
        int count = 0;
        foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < smr.bones.Length; i++)
            {
                if (smr.bones[i] == null)
                {
                    count++;
                    indexes.Add(i);
                }
            }
            indexes.Reverse();
            List<Transform> bones = smr.bones.ToList();
            foreach (var index in indexes)
            {
                bones.RemoveAt(index);
            }

            smr.bones = bones.ToArray();
        }
        print("모든 SkinnedMeshRenderer의 bones 수정 완료. 총 "+count+"회의 수정.");
    }
}
