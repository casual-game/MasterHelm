using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ImageDefaultMaterialChanger : MonoBehaviour
{
    public Material targetMat;
    public Material defaultMat;
    [Button]
    public void ChangeDefaultMaterial()
    {
        Image[] images = FindObjectsOfType<Image>(true);
        int count = 0;
        foreach (var image in images)
        {
            print(image.material.name);
            if (image.material == targetMat)
            {
                
                image.material = defaultMat;
                count++;
            }
        }
        print("시도한 Image 개수: "+images.Length);
        print(count + "개의 Image의 Material을 교체하였습니다.");
    }
}
