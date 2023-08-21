using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Map_Template : MonoBehaviour
{
    [FoldoutGroup("Parent")] [ReadOnly] public Transform
        map_base,
        map_deco_lamp,
        map_deco_ivy,
        map_deco_water,
        map_deco_chain,
        map_deco_pillar,
        map_deco_simple,
        map_deco_root,
        map_deco_candle,
        map_rock,
        map_breakable,
        map_fx,
        map_light;
    List<Transform> Scan()
    {
        map_base = transform.Find("Map_Base");
        Transform deco = transform.Find("Map_Deco");
        map_deco_lamp = deco.Find("Lamp");
        map_deco_ivy = deco.Find("Ivy");
        map_deco_water = deco.Find("Water");
        map_deco_chain= deco.Find("Chain");
        map_deco_pillar = deco.Find("Pillar");
        map_deco_simple = deco.Find("Simple");
        map_deco_root = deco.Find("Root");
        map_deco_candle = deco.Find("Candle");
        map_rock = transform.Find("Map_Rock");
        map_breakable = transform.Find("Map_Breakable");
        map_fx = transform.Find("FX");
        map_light = transform.Find("Light");
        
        List<Transform> ts = new List<Transform>();
        ts.Add(map_base);
        ts.Add(map_deco_lamp);
        ts.Add(map_deco_ivy);
        ts.Add(map_deco_water);
        ts.Add(map_deco_chain);
        ts.Add(map_deco_pillar);
        ts.Add(map_deco_simple);
        ts.Add(map_deco_root);
        ts.Add(map_deco_candle);
        ts.Add(map_rock);
        ts.Add(map_light);
        ts.Add(map_breakable);
        foreach (var t in ts)
        {
            for (int i = 0; i < 10; i++)
            {
                string _name = "Type_" + i.ToString();
                if (t.Find(_name) == null)
                {
                    GameObject g = new GameObject(_name);
                    g.transform.parent = t;
                    g.transform.localPosition = Vector3.zero;
                    g.transform.localRotation = Quaternion.identity;
                    g.transform.localScale= Vector3.one;
                }
            }
        }

        return ts;
    }
    //레벨 디자인
    public enum NUM { Zero=0,One=1,Two=2,Three=3,Four=4,Five=5,Six=6,Seven=7,Eight=8,Nine=9 }
    [HideLabel][EnumToggleButtons,OnValueChanged("ChangeType")] public NUM num;
    public void ChangeType()
    {
        foreach (Transform t in Scan())
        {
            foreach (Transform tt in t)
            {
                tt.gameObject.SetActive(tt.gameObject.name == "Type_"+(int)num);
            }
        }
    }
    //맵 빌드
    [Button]
    public void Build()
    {
        #region Root 생성
        GameObject build = new GameObject("Build");
        GameObject build_base_mesh = new GameObject("Base_Mesh");
        GameObject build_base_collider = new GameObject("Base_Collider");
        GameObject build_base_sparkable = new GameObject("Base_Sparkable");
        GameObject build_deco = new GameObject("Deco");
        GameObject build_breakable = new GameObject("Breakable");
        GameObject build_rock = new GameObject("Rock");
        GameObject build_fx = new GameObject("FX");
        GameObject build_light = new GameObject("Light");
        
        
        GameObject build_deco_lamp = new GameObject("Lamp");
        GameObject build_deco_ivy = new GameObject("Ivy");
        GameObject build_deco_water = new GameObject("Water");
        GameObject build_deco_chain = new GameObject("Chain");
        GameObject build_deco_pillar = new GameObject("Pillar");
        GameObject build_deco_simple = new GameObject("Simple");
        GameObject build_deco_root = new GameObject("Root");
        GameObject build_deco_cnadle = new GameObject("Cnadle");
        
        
        build_base_mesh.transform.SetParent(build.transform);
        build_base_collider.transform.SetParent(build.transform);
        build_base_sparkable.transform.SetParent(build.transform);
        build_deco.transform.SetParent(build.transform);
        build_deco.transform.SetParent(build.transform);
        build_breakable.transform.SetParent(build.transform);
        build_rock.transform.SetParent(build.transform);
        build_fx.transform.SetParent(build.transform);
        build_light.transform.SetParent(build.transform);
        
        build_deco_lamp.transform.SetParent(build_deco.transform);
        build_deco_ivy.transform.SetParent(build_deco.transform);
        build_deco_water.transform.SetParent(build_deco.transform);
        build_deco_chain.transform.SetParent(build_deco.transform);
        build_deco_pillar.transform.SetParent(build_deco.transform);
        build_deco_simple.transform.SetParent(build_deco.transform);
        build_deco_root.transform.SetParent(build_deco.transform);
        build_deco_cnadle.transform.SetParent(build_deco.transform);
        #endregion

        //Build_Base
        foreach (MeshFilter mf in map_base.GetComponentsInChildren<MeshFilter>(false))
        {
            if (mf.gameObject.activeSelf)
            {
                GameObject g = Instantiate(mf.gameObject);
                g.transform.position = mf.transform.position;
                g.transform.rotation = mf.transform.rotation;
                g.transform.localScale = mf.transform.lossyScale;
                g.transform.SetParent(build_base_mesh.transform);
            }
        }
        foreach (BoxCollider bc in map_base.GetComponentsInChildren<BoxCollider>(false))
        {
            GameObject g = Instantiate(bc.gameObject);
            g.transform.position = bc.transform.position;
            g.transform.rotation = bc.transform.rotation;
            g.transform.localScale = bc.transform.lossyScale;
            g.transform.SetParent(build_base_collider.transform);
        }
        foreach (Sparkable sp in gameObject.GetComponentsInChildren<Sparkable>(false))
        {
            GameObject g = Instantiate(sp.gameObject);
            g.transform.position = sp.transform.position;
            g.transform.rotation = sp.transform.rotation;
            g.transform.localScale = sp.transform.lossyScale;
            g.transform.SetParent(build_base_sparkable.transform);
        }
        //Deco_Lamp
        foreach (Transform t in map_deco_lamp.transform)
        {
            if(!t.gameObject.activeSelf) continue;
            GameObject g = Instantiate(t.gameObject);
            g.transform.position = t.position;
            g.transform.rotation = t.rotation;
            g.transform.localScale = t.lossyScale;
            g.transform.SetParent(build_deco_lamp.transform);
        }
        //Deco_Ivy
        foreach (Transform t in map_deco_ivy.transform)
        {
            if(!t.gameObject.activeSelf) continue;
            GameObject g = Instantiate(t.gameObject);
            g.transform.position = t.position;
            g.transform.rotation = t.rotation;
            g.transform.localScale = t.lossyScale;
            g.transform.SetParent(build_deco_ivy.transform);
        }
        //Deco_Water
        foreach (Transform t in map_deco_water.transform)
        {
            if(!t.gameObject.activeSelf) continue;
            GameObject g = Instantiate(t.gameObject);
            g.transform.position = t.position;
            g.transform.rotation = t.rotation;
            g.transform.localScale = t.lossyScale;
            g.transform.SetParent(build_deco_water.transform);
        }
        //Deco_Chain
        foreach (Transform t in map_deco_chain.transform)
        {
            if(!t.gameObject.activeSelf) continue;
            GameObject g = Instantiate(t.gameObject);
            g.transform.position = t.position;
            g.transform.rotation = t.rotation;
            g.transform.localScale = t.lossyScale;
            g.transform.SetParent(build_deco_chain.transform);
        }
        //Deco_Pillar
        foreach (MeshFilter mf in map_deco_pillar.GetComponentsInChildren<MeshFilter>(false))
        {
            if (mf.gameObject.activeSelf)
            {
                GameObject g = Instantiate(mf.gameObject);
                g.transform.position = mf.transform.position;
                g.transform.rotation = mf.transform.rotation;
                g.transform.localScale = mf.transform.lossyScale;
                g.transform.SetParent(build_deco_pillar.transform);
            }
        }
        //Deco_Simple
        foreach (Transform t in map_deco_simple.transform)
        {
            if(!t.gameObject.activeSelf) continue;
            GameObject g = Instantiate(t.gameObject);
            g.transform.position = t.position;
            g.transform.rotation = t.rotation;
            g.transform.localScale = t.lossyScale;
            g.transform.SetParent(build_deco_simple.transform);
        }
        //Deco_Root
        foreach (Transform t in map_deco_root.transform)
        {
            if(!t.gameObject.activeSelf) continue;
            GameObject g = Instantiate(t.gameObject);
            g.transform.position = t.position;
            g.transform.rotation = t.rotation;
            g.transform.localScale = t.lossyScale;
            g.transform.SetParent(build_deco_root.transform);
        }
        //Deco_Candle
        foreach (Transform t in map_deco_candle.transform)
        {
            if(!t.gameObject.activeSelf) continue;
            GameObject g = Instantiate(t.gameObject);
            g.transform.position = t.position;
            g.transform.rotation = t.rotation;
            g.transform.localScale = t.lossyScale;
            g.transform.SetParent(build_deco_cnadle.transform);
        }
        //Breakable
        foreach (Transform t in map_breakable.transform)
        {
            if(!t.gameObject.activeSelf) continue;
            GameObject g = Instantiate(t.gameObject);
            g.transform.position = t.position;
            g.transform.rotation = t.rotation;
            g.transform.localScale = t.lossyScale;
            g.transform.SetParent(build_breakable.transform);
        }
        //Rock
        foreach (MeshFilter mf in map_rock.GetComponentsInChildren<MeshFilter>(false))
        {
            if (mf.gameObject.activeSelf)
            {
                GameObject g = Instantiate(mf.gameObject);
                g.transform.position = mf.transform.position;
                g.transform.rotation = mf.transform.rotation;
                g.transform.localScale = mf.transform.lossyScale;
                g.transform.SetParent(build_rock.transform);
            }
        }
        //FX
        foreach (Transform t in map_fx.transform)
        {
            if(!t.gameObject.activeSelf) continue;
            GameObject g = Instantiate(t.gameObject);
            g.transform.position = t.position;
            g.transform.rotation = t.rotation;
            g.transform.localScale = t.lossyScale;
            g.transform.SetParent(build_fx.transform);
        }
        //Light
        foreach (Transform t in map_light.transform)
        {
            if(!t.gameObject.activeSelf) continue;
            GameObject g = Instantiate(t.gameObject);
            g.transform.position = t.position;
            g.transform.rotation = t.rotation;
            g.transform.localScale = t.lossyScale;
            g.transform.SetParent(build_light.transform);
        }
    }
    
    
    //머티리얼 관리
    public List<Material> materials;
    [Button]
    public void FindMaterial()
    {
        materials = new List<Material>();
        foreach (var m in FindObjectsOfType<MeshRenderer>())
        {
            if(!m.gameObject.activeSelf) continue;
            if(!materials.Contains(m.sharedMaterial)) materials.Add(m.sharedMaterial);
        }
        foreach (var m in FindObjectsOfType<SkinnedMeshRenderer>())
        {
            if(!m.gameObject.activeSelf) continue;
            if(!materials.Contains(m.sharedMaterial)) materials.Add(m.sharedMaterial);
        }
    }
}
