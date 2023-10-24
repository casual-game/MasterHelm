using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    public static Transform Folder_Hero,Folder_Monster,Folder_MonsterProp;
    
    private void Setting_Resource()
    {
        Transform mainFolder = CreateFolder("Folder", null);
        Folder_Hero = CreateFolder("Hero", mainFolder);
        Folder_Monster = CreateFolder("Monster", mainFolder);
        Folder_MonsterProp = CreateFolder("MonsterProp", mainFolder);
        Transform CreateFolder(string folderName,Transform parent)
        {
            GameObject f = new GameObject(folderName);
            f.transform.SetParent(parent);
            f.transform.SetPositionAndRotation(V3_Zero,Q_Identity);
            f.transform.localScale = GameManager.V3_One;
            return f.transform;
        }
    }
}
