using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomMaterialController : MonoBehaviour
{
    [LabelText("커스텀 Material 리스트")]
    public List<CustomMaterialData> customMaterialDatas = new List<CustomMaterialData>();
    private Dictionary<string, CustomMaterialData> datas = new Dictionary<string, CustomMaterialData>();
    private CustomMaterialData currentCustomMaterial = null;
    public void Setting(List<Renderer> renderers)
    {
        //CustomMaterial들 전부 초기화
        foreach (var cmd in customMaterialDatas)
        {
            datas.Add(cmd.keyword, cmd);
            cmd.Setting();
        }
        //Renderer의 materials 배열 업데이트
        foreach (var renderer in renderers)
        {
            List<Material> newMats = new List<Material>();
            newMats.Add(renderer.material);
            foreach (var cmd in customMaterialDatas)
            {
                if(!cmd.reuse) newMats.Add(cmd.material);
            }

            renderer.materials = newMats.ToArray();
        }
    }
    public void Activate(string keyword)
    {
        if (currentCustomMaterial != null) currentCustomMaterial.Deactivate(this);
        currentCustomMaterial = datas[keyword];
        currentCustomMaterial.Activate(this);
    }

    public (Material _mat,int _id) GetReuseTargetMatData(string keyword)
    {
        var data = datas[keyword];
        return (data.material,data.id);
    }
    public void Deactivate()
    {
        if (currentCustomMaterial != null)
        {
            currentCustomMaterial.Deactivate(this);
            currentCustomMaterial = null;
        }
    }
}

[System.Serializable]
public class CustomMaterialData
{
    [Title("$GetLabelText")]
    public string keyword = "사용할 키워드를 영어로 넣어주세요.";
    public bool finishParticle = true;
    public bool reuse = false;
    
    [HideIf("$reuse")] public Material material;
    [HideIf("$reuse")] public string mat_property;
    [ShowIf("$reuse")] public string reuseMaterialKeyword;
    
    [ColorUsage(true,true)] 
    public Color color_Activated,color_Deactivated;
    public float tweenBeginDurataion = 0.25f,tweenFinDuration = 0.25f;
    public List<ParticleSystem> particles;

    private Tween _tween;
    public int id
    {
        get;
        private set;
    }
    public void Setting()
    {
        if (!reuse)
        {
            material = new Material(material);
            id = Shader.PropertyToID(mat_property);
        }
    }
    
    public void Activate(CustomMaterialController controller)
    {
        Activate_Material(controller);
        Activate_Particle();
    }
    public void Activate_Material(CustomMaterialController controller)
    {
        if (reuse)
        {
            var data = controller.GetReuseTargetMatData(reuseMaterialKeyword);
            _tween.Stop();
            _tween = Tween.MaterialColor(data._mat, data._id, color_Activated, tweenBeginDurataion);
        }
        else
        {
            _tween.Stop();
            _tween = Tween.MaterialColor(material, id, color_Activated, tweenBeginDurataion);
        }
    }
    public void Activate_Particle()
    {
        foreach (var particle in particles)
        {
            particle.Play();
        }
    }
    public void Deactivate(CustomMaterialController controller)
    {
        if (finishParticle)
        {
            foreach (var particle in particles)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }   
        }
        if (reuse)
        {
            var data = controller.GetReuseTargetMatData(reuseMaterialKeyword);
            _tween.Stop();
            _tween = Tween.MaterialColor(data._mat, data._id, color_Deactivated, tweenFinDuration);
        }
        else
        {
            _tween.Stop();
            _tween = Tween.MaterialColor(material, id, color_Deactivated, tweenFinDuration);
        }
    }
    
    
    private string GetLabelText()
    {
        if (keyword == string.Empty) return "사용할 키워드를 영어로 넣어주세요.";
        else return keyword;
    }
}