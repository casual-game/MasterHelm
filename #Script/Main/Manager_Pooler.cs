using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class Manager_Pooler : MonoBehaviour
{
    [ShowInInspector]
    public Dictionary<string, PoolInstance> pooler = new Dictionary<string, PoolInstance>();
    public static Manager_Pooler instance;
    public Transform parent;
    public void Setting()
    {
        pooler.Clear();
        instance = this;
        parent = new GameObject("pool").transform;
        parent.SetParent(Manager_Main.instance._folder_);
    }
    [System.Serializable]
    public class PoolInstance
    {
        
        public bool loaded = false;
        public string tag;
        public Transform parent;
        public Queue<GameObject> list;
        public GameObject asset;
        public AsyncOperationHandle handle;
        public Quaternion originRot;
        public Vector3 originScale;
        public PoolInstance(string tag,Transform parent)
        {
            list = new Queue<GameObject>();
            this.tag = tag;
            this.parent = new GameObject(tag).transform;
            this.parent.parent = parent;
        }
    }

    public IEnumerator Add(string tag, int count = 2)
    {
        //Pooler에 에셋 로드 후 추가.
        if (!pooler.ContainsKey(tag))
        {
            pooler.Add(tag, new PoolInstance(tag,parent));
            Addressables.LoadAssetAsync<GameObject>(tag).Completed +=
                (AsyncOperationHandle<GameObject> obj) =>
                {
                    pooler[tag].handle = obj;
                    pooler[tag].asset = obj.Result;
                    pooler[tag].loaded = true;
                };
            while (!pooler[tag].loaded) yield return null;
        }

        
        //pool에 asset들 추가.
        for (int i = 0; i < count; i++)
        {
            yield return StartCoroutine(Load(tag));
        }
        //print("LOAD: " + tag);
    }

    IEnumerator Load(string tag)
    {
        while (pooler[tag].asset == null) yield return null;
        GameObject newG = Instantiate(pooler[tag].asset);
        newG.transform.parent = pooler[tag].parent;
        newG.transform.rotation = Quaternion.identity;
        //newG.transform.localScale = Vector3.one;
        newG.SetActive(false);
        if (pooler[tag].list.Count == 0)
        {
            pooler[tag].originRot = pooler[tag].asset.transform.localRotation;
            pooler[tag].originScale = pooler[tag].asset.transform.localScale;
        }
        pooler[tag].list.Enqueue(newG);
        
    }
    public GameObject Get(string tag)
    {
        if (!pooler.ContainsKey(tag)) return null;
        GameObject queueG = pooler[tag].list.Peek();
        if (queueG.activeSelf)
        {
            GameObject gg = Instantiate(pooler[tag].asset);
            gg.transform.parent = pooler[tag].parent;
            pooler[tag].list.Enqueue(gg);
            return gg;
        }
        else
        {
            GameObject gg=pooler[tag].list.Dequeue();
            pooler[tag].list.Enqueue(gg);
            return gg;
        }
    }
    public GameObject GetParticle(string tag,Vector3 position,Quaternion rot,float scale = 1.0f)
    {
        if (!pooler.ContainsKey(tag)) return null;
        GameObject smoke =Get(tag);
        smoke.transform.localScale = pooler[tag].originScale*scale;
        smoke.transform.rotation = rot*pooler[tag].originRot;
        smoke.transform.position = position;
        smoke.SetActive(true);
        return smoke;
    }
    //언젠가 호출할 날이 오겠지..
    private void Unload()
    {
        foreach (var pool in  pooler)
        {
            Addressables.Release(pool.Value.handle);
        }
    }
    //공용 이펙트
    private string s_shockwave = "Shockwave";
    public void Shockwave(Vector3 justPos)
    {
        if (!pooler.ContainsKey(s_shockwave)) return;
        GameObject smoke =Get(s_shockwave);
        smoke.transform.localScale = pooler[s_shockwave].originScale;
        smoke.transform.rotation = pooler[s_shockwave].originRot;
        justPos.y += 3;
        smoke.transform.position = justPos;
        smoke.SetActive(true);
    }
}
