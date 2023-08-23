using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dest.Math;
using Dest.Math.Tests;
using DG.Tweening;
using Sirenix.OdinInspector;
using TrailsFX;
using Unity.VisualScripting;
using UnityEngine;
using XftWeapon;
using Random = UnityEngine.Random;

public class Prefab_Prop : MonoBehaviour
{
    //Public 컴포넌트
    public DropTable dropTable;
    
    //Private 컴포넌트 (Default)
    [HideInInspector] public Color color;
    private float returnspeed=0.2f;
    [HideInInspector] public Transform T_hold;
    [HideInInspector] public XWeaponTrail xWeaponTrailtrail;
    [HideInInspector] public TrailEffect trailEffect;
    [HideInInspector] public ParticleSystem charge_Effect = null;
    private Material charge_EffectMat=null;
    [HideInInspector] public Animator animator;
    private List<Transform> boxT;
    private List<Box3> currentBox, pastBox,interpolationBox;
    [HideInInspector] public Transform parent;
    private Rigidbody rigidbody;
    private MeshCollider meshCollider;
    private bool isPlayer =false;
    [HideInInspector] public ParticleSystem sp_slash,sp_hit;
    [HideInInspector] public ParticleSystem[] sp_slashes; 
    //Private 컴포넌트 (Player)
    private bool isMain = false;
    //Private 컴포넌트 (Enemy)
    private Enemy enemy;
    [HideInInspector] public Prefab_Prop key;
    public void Setting_Player(Transform parent,ElementalAttributes elementalAttributes,bool isMain=false)
    {
        isPlayer = true;
        this.isMain = isMain;
        this.parent = parent;
        T_hold = transform.Find("Hold");
        foreach (Transform t in transform)
        {
            if (t.gameObject.name.ToLower().Contains("slash"))
            {
                sp_slash = t.GetComponent<ParticleSystem>();
                sp_slashes = t.GetComponentsInChildren<ParticleSystem>();
            }
            if (t.gameObject.name.ToLower().Contains("hit")) sp_hit = t.GetComponent<ParticleSystem>();
        }
        if(sp_slash!=null)sp_slash.transform.SetParent(Manager_Main.instance._folder_);
        if(sp_hit!=null)sp_hit.transform.SetParent(Manager_Main.instance._folder_);
        Setting_Default(elementalAttributes);
    }
    public void Setting_Enemy(Transform parent,Enemy enemy)
    {
        enemy_dropped = false;
        this.enemy = enemy;
        isPlayer = false;
        restored = false;
        this.parent = parent;
        T_hold = transform.Find("EHold");
        gameObject.SetActive(true);
        Setting_Default(ElementalAttributes.Fire);
        animator.speed = 0.75f;
        //if (xWeaponTrailtrail != null) xWeaponTrailtrail.MyMaterial = Manager_Main.instance.mat_EnemyXweaponTrail;
    }
    private void Setting_Default(ElementalAttributes elementalAttributes)
    {
        foreach (var t in GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = LayerMask.NameToLayer("Ragdoll");
        }
        xWeaponTrailtrail = transform.GetComponentInChildren<XWeaponTrail>(true);
        trailEffect = transform.GetComponent<TrailEffect>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.enabled = false;
        dropped = false;
        trail = true;
        Trail_Off();
        
        currentBox = new List<Box3>();
        pastBox = new List<Box3>();
        interpolationBox = new List<Box3>();
        boxT = new List<Transform>();
        for (int i = 0; i < 5; i++)
        {
            Transform t = transform.Find("Box3_" + (i + 1));
            if (t == null) break;
            boxT.Add(t);
            currentBox.Add(CreateBox3(t));
            pastBox.Add(CreateBox3(t));
        }
        charge_Effect = transform.Find("Charge_Effect").GetComponent<ParticleSystem>();
        charge_Effect.transform.SetParent(transform);
        charge_Effect.transform.localPosition = Vector3.zero;
        charge_Effect.transform.localRotation = Quaternion.identity;
        charge_EffectMat = new Material(charge_Effect.GetComponent<Renderer>().material);
        charge_Effect.GetComponent<Renderer>().material = charge_EffectMat;
        color = Manager_Main.instance.mainData.ElementalColor(elementalAttributes);
        charge_EffectMat.SetColor("_TintColor",color*5f);
        charge_Effect.gameObject.SetActive(true);
    }

    #region Trail
    private bool trail;
    private List<Sparkable> sparkables = new List<Sparkable>();
    public void Trail_On()
    {
        //주변의 spark대상 스캔
        float limit = 10.0f;
        Vector3 currentPos = Player.instance.transform.position;
        sparkables.Clear();
        if (isPlayer)
        {
            /*
            foreach (var sparkable in Sparkable.Sparkables)
            {
                sparkables.Add(sparkable);
                if (Vector3.SqrMagnitude(sparkable.transform.position - currentPos) < (limit * limit))
                {
                    //sparkables.Add(sparkable);
                }
                    
            }
            */
        }
        //
        if (isPlayer)
        {
            int pstate = Player.instance.animator.GetInteger("State");
            if (pstate == 4 || pstate == 5 || pstate == 6)
            {
                Trail_Off();
            }
        }
        else if(enemy.state_danger) Trail_Off();
            
        //------------------------------------------------------------------------
        if (trail) return;
        trail = true;
        trailEffect.active = true;
        if (xWeaponTrailtrail != null) xWeaponTrailtrail.Activate();
        for (int i = 0; i < boxT.Count; i++)
        {
            currentBox[i]=CreateBox3(boxT[i]);
            pastBox[i]=CreateBox3(boxT[i]);
        }
        if(isPlayer)Canvas_Player.instance.OnLateUpdate.AddListener(Trail_Update_Player);
        else
        {
            Canvas_Player.instance.OnLateUpdate.AddListener(Trail_Update_Enemy);
            enemy.attack_attacked = false;
            print("공격 시작");
        }
        
    }
    public void Trail_Off()
    {
        if (!trail) return;
        trail = false;
        trailEffect.active = false;
        if (xWeaponTrailtrail != null) xWeaponTrailtrail.StopSmoothly(returnspeed);
        
        if(isPlayer)Canvas_Player.instance.OnLateUpdate.RemoveListener(Trail_Update_Player);
        else Canvas_Player.instance.OnLateUpdate.RemoveListener(Trail_Update_Enemy);
    }
    public void Trail_Update_Player()
    {
        interpolationBox.Clear();
        for (int i = 0; i < boxT.Count; i++)
        {
            pastBox[i] = currentBox[i];
            currentBox[i] = CreateBox3(boxT[i]);
            float dist = Vector3.Distance(pastBox[i].Center, currentBox[i].Center);
            if (dist > Manager_Main.instance.mainData.weaponPhysicsCalculateMin)
            {
                int repeat = Mathf.FloorToInt(dist / Manager_Main.instance.mainData.weaponPhysicsCalculateMin);
                for (int j = 1; j < repeat; j++)
                {
                    float ratio = Mathf.Clamp01(j * 1.0f / (repeat - 1));
                    Vector3 pos = Vector3.Lerp(currentBox[i].Center, pastBox[i].Center, ratio);
                    Vector3 axis0 = Vector3.Lerp(currentBox[i].Axis0, pastBox[i].Axis0, ratio);
                    Vector3 axis1 = Vector3.Lerp(currentBox[i].Axis1, pastBox[i].Axis1, ratio);
                    Vector3 axis2 = Vector3.Lerp(currentBox[i].Axis2, pastBox[i].Axis2, ratio);
                    interpolationBox.Add(new Box3(pos, axis0, axis1, axis2, boxT[i].lossyScale));
                }
            }
        }
        
        List<Box3> detectBox = new List<Box3>();
        detectBox.AddRange(currentBox);
        detectBox.AddRange(interpolationBox);
        detectBox.AddRange(pastBox);
        for (int i = 0; i < detectBox.Count; i++)
        {
            //일반 충돌판정
            Box3 box3 = detectBox[i];
            int enemyCount = Enemy.enemies.Count;
            for (int j = enemyCount-1; j >= 0; j--)
            {
                Enemy enemy = Enemy.enemies[j];
                if(Player.instance.attackedTarget.Contains(enemy)) continue;
                Capsule3 enemyCapsule = enemy.Get_Capsule3();
                if (Intersection.TestBox3Capsule3(ref box3, ref enemyCapsule)) DetectEnemy(enemy);
            }
            /*
            int jlength = DestructibleObject.destructibleObjects.Count;
            for (int j = jlength-1; j >= 0; j--)
            {
                DestructibleObject destructible = DestructibleObject.destructibleObjects[j];
                Box3 destructibleBox = destructible.box;
                if (Intersection.TestBox3Box3(ref box3, ref destructibleBox)) destructible.Explode(Player.instance.transform.position);
            }
            */
            //spark 효과
            Vector3 size = box3.Axis1 * box3.Extents.y, lossyscale = transform.lossyScale;
            Vector3 begin = box3.Center - size;
            
            for (int j = sparkables.Count-1; j >= 0 ; j--)
            {
                bool detected = Intersection.TestBox3Box3(ref box3, ref sparkables[j].box);
                if (detected)
                {
                    //CamArm.instance.Shake(Manager_Main.instance.mainData.impact_Hit);
                    Player.instance.audio_Hit_Spark.Play();
                    Manager_Pooler.instance.GetParticle("Spark", begin + size, Quaternion.identity);
                    Manager_Main.instance.Vibrate(0.1f,0.25f);
                    sparkables.RemoveAt(j);
                    break;
                }
            }
        }

        
    }
    public void Trail_Update_Enemy()
    {
        interpolationBox.Clear();
        if (enemy.attack_attacked) return;
        for (int i = 0; i < boxT.Count; i++)
        {
            pastBox[i] = currentBox[i];
            currentBox[i] = CreateBox3(boxT[i]);
            float dist = Vector3.Distance(pastBox[i].Center, currentBox[i].Center);
            if (dist > Manager_Main.instance.mainData.weaponPhysicsCalculateMin)
            {
                int repeat = Mathf.FloorToInt(dist / Manager_Main.instance.mainData.weaponPhysicsCalculateMin);
                for (int j = 1; j < repeat; j++)
                {
                    float ratio = Mathf.Clamp01(j * 1.0f / (repeat - 1));
                    Vector3 pos = Vector3.Lerp(currentBox[i].Center, pastBox[i].Center, ratio);
                    Vector3 axis0 = Vector3.Lerp(currentBox[i].Axis0, pastBox[i].Axis0, ratio);
                    Vector3 axis1 = Vector3.Lerp(currentBox[i].Axis1, pastBox[i].Axis1, ratio);
                    Vector3 axis2 = Vector3.Lerp(currentBox[i].Axis2, pastBox[i].Axis2, ratio);
                    interpolationBox.Add(new Box3(pos, axis0, axis1, axis2, boxT[i].lossyScale));
                }
            }
        }
        
        List<Box3> detectBox = new List<Box3>();
        detectBox.AddRange(currentBox);
        detectBox.AddRange(interpolationBox);
        detectBox.AddRange(pastBox);
        for (int i = 0; i < detectBox.Count; i++)
        {
            Box3 box3 = detectBox[i];
            Capsule3 playerCapsule = Player.instance.Get_Capsule3();
            if (Intersection.TestBox3Capsule3(ref box3, ref playerCapsule))
            {
                DetectPlayer();
                break;
            }
        }
    }
    #endregion
    #region Effect
    public void On(bool on, Transform parent, bool immediately = false)
    {
        if (parent != null)
        {
            transform.SetParent(parent);
            transform.localPosition = T_hold.localPosition;
            transform.localRotation = T_hold.localRotation;
            transform.localScale = T_hold.localScale;
            if(!gameObject.activeSelf) gameObject.SetActive(true);
        }
        else transform.SetParent(Manager_Main.instance._folder_);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("On") && !on)
        {
            float currentRatio = Mathf.Clamp01(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            if(!immediately) animator.Play("Off",0,1-currentRatio);
            else animator.Play("Off",0,1);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Off") && on)
        {
            float currentRatio = Mathf.Clamp01(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            if(!immediately) animator.Play("On",0,1-currentRatio);
            else animator.Play("On", 0,1);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Disabled"))
        {
            if(on) animator.Play("On", 0);
            else animator.Play("Off", 0);
        }
    }

    public void SP_Slash()
    {
        Transform t = sp_slash.transform;
        t.position = Player.instance.transform.position + Player.instance.skillData.sp_slash_pos;
        t.rotation = Player.instance.transform.rotation * Quaternion.Euler(Player.instance.skillData.sp_slash_rot);
        t.localScale = Player.instance.skillData.sp_slash_scale;
        foreach (var slash in sp_slashes)
        {
            var main = slash.main;
            main.simulationSpeed = Player.instance.skillData.sp_slash_speed;
            main.startDelay = Player.instance.skillData.sp_slash_delay;
        }
        sp_slash.Play();
    }

    public void Emit()
    {
        if(!isMain) charge_Effect.Play();
    }
    #endregion
    #region Box

    

    #if UNITY_EDITOR
    public List<Box3> gizmoBox = new List<Box3>();
    private void OnDrawGizmos()
    {
        
        gizmoBox = new List<Box3>();
        for (int i = 0; i < 5; i++)
        {
            Transform t = transform.Find("Box3_" + (i + 1));
            if (t != null) gizmoBox.Add(CreateBox3(t));
        }

        foreach (var _box in gizmoBox)
        {
            DrawBox(_box);
        }
        /*
        foreach (var box in gizmoBox)
        {
            Box3 box3 = box;
            Vector3 size = box3.Axis1 * box3.Extents.y, lossyscale = transform.lossyScale;
            Vector3 begin = box3.Center - size;
            Vector3 end = begin + new Vector3(size.x / lossyscale.x, size.y / lossyscale.y, size.z / lossyscale.z) * 2;
            Segment3 segment3 = new Segment3(begin,end);
            Debug.DrawLine(begin,end,Color.red);
            bool found = false;
            
            foreach (var sparkable in FindObjectsOfType<Sparkable>())
            {
                Segment3Box3Intr intr;
                if (Intersection.FindSegment3Box3(ref segment3, ref sparkable.box, out intr))

                {
                    print("Found: "+sparkable.name);
                    Gizmos.DrawSphere(intr.Point0, 0.5f);
                    return;
                }
            }
        }
        print("NO~");
        */
        if(!trail) return;



        return;
        foreach (var box in currentBox)
        {
            DrawBox(box);
        }

        foreach (var box in pastBox)
        {
            DrawBox(box);
        }

        foreach (var box in interpolationBox)
        {
            DrawBox(box);
        }

    }
    #endif
    private void DrawBox(Box3 box)
    {
        Vector3 v0, v1, v2, v3, v4, v5, v6, v7;
        box.CalcVertices(out v0, out v1, out v2, out v3, out v4, out v5, out v6, out v7);
        Gizmos.DrawLine(v0, v1);
        Gizmos.DrawLine(v1, v2);
        Gizmos.DrawLine(v2, v3);
        Gizmos.DrawLine(v3, v0);
        Gizmos.DrawLine(v4, v5);
        Gizmos.DrawLine(v5, v6);
        Gizmos.DrawLine(v6, v7);
        Gizmos.DrawLine(v7, v4);
        Gizmos.DrawLine(v0, v4);
        Gizmos.DrawLine(v1, v5);
        Gizmos.DrawLine(v2, v6);
        Gizmos.DrawLine(v3, v7);
    }
    private Box3 CreateBox3(Transform box)
    {
        return new Box3(box.position, box.right, box.up, box.forward, box.lossyScale);
    }
    #endregion
    #region Player

    private bool player_dropped = false;
    private Coroutine c_player_pickup = null;
    void DetectEnemy(Enemy enemy)
    {
        if (enemy.death) return;
        Player.instance.attackedTarget.Add(enemy);
        
        bool isCounter = !enemy.state_danger && enemy.state_NormalState == Enemy.NormalState.Attack
                                             && (Player.instance.isStrong || Player.instance.isSkill);
        if (pastBox.Count > 0 && currentBox.Count > 0) enemy.Hit(isCounter,currentBox[0].Center - pastBox[0].Center);
        else enemy.Hit(isCounter,transform.forward);
        #region 스킬 충전

        float skillPoint = 20;
        if (Player.instance.isCharge) skillPoint += 40;
        Canvas_Player.instance.skillGauge_L.SetValue(Canvas_Player.instance.skillGauge_L.current + skillPoint);
        Canvas_Player.instance.skillGauge_R.SetValue(Canvas_Player.instance.skillGauge_R.current + skillPoint);

        #endregion
        Effect_Hit();

        void Effect_Hit()
        {
            //스킬일 경우 고유이펙트 만들기
            if (!enemy.IsGuard()&&Player.instance.isSkill)
            {
                Transform spT = sp_hit.transform;
                spT.position = enemy.transform.position + Vector3.up * 1.2f;
                spT.localScale = Vector3.one*Player.instance.skillData.sp_hit_scale;
                sp_hit.Play();
                SP_Slash();
            }
            //Bullet 추가
            if (Player.instance.isStrong) Canvas_Player_World.instance.ManaStone_Append(0.5f);
        }

    }
    public void Drop_Player()
    {
        //설정
        player_dropped = true;
        if (c_player_pickup != null) StopCoroutine(c_player_pickup);
        transform.SetParent(Manager_Main.instance._folder_);
        //물리
        rigidbody.isKinematic = false;
        meshCollider.enabled = true;
        Vector3 explodePos = Player.instance.transform.position;
        Vector3 dist = (explodePos - transform.position).normalized * 2.5f;
        dist.y = 0;
        explodePos = transform.GetChild(0).position + dist;
        rigidbody.AddExplosionForce (4, explodePos, 2.0f,1.5f, ForceMode.VelocityChange);
        
    }

    public void Pickup_Player()
    {
        if (!player_dropped) return;
        if (c_player_pickup != null) StopCoroutine(c_player_pickup);
        c_player_pickup = StartCoroutine(C_Puckup_Player());
    }
    private IEnumerator C_Puckup_Player()
    {
        On(false,null);
        yield return new WaitForSeconds(0.5f);
        rigidbody.isKinematic = true;
        meshCollider.enabled = false;
        player_dropped = false;
        On(true,parent);
    }
    #endregion
    #region Enemy

    private bool restored = false;
    private bool enemy_dropped = false;
    private static int dropcount = 0;

    public void Restore()
    {

        if (restored)
        {
            //print(gameObject.name +"이미저ㅏㅈㅇ됨.");
            return;
        }
        StopCoroutine("C_Drop_Enemy");
        restored = true;
        Manager_Enemy.instance.RestoreProp(key,this);
    }
    public void Drop_Enemy()
    {
        if (enemy_dropped) return;
        enemy_dropped = true;
        transform.SetParent(Manager_Main.instance._folder_);
        rigidbody.isKinematic = false;
        meshCollider.enabled = true;
        
        Vector3 explodePos = Player.instance.transform.position;
        Vector3 dist = (explodePos - transform.position).normalized * 2.5f;
        dist.y = 0;
        explodePos = transform.GetChild(0).position + dist;
        rigidbody.AddExplosionForce (4, explodePos, 3.0f,4.5f, ForceMode.VelocityChange);
        
        if (!restored)
        {
            StartCoroutine("C_Drop_Enemy");
        }
        else print(gameObject.name);
    }

    
    private IEnumerator C_Drop_Enemy()
    {
        dropcount = (dropcount + 1) % 2;
        yield return new WaitForSeconds(2.0f + dropcount*0.5f);
        On(false,null);
        yield return new WaitForSeconds(2.0f);
        Restore();
    }
    private void DetectPlayer()
    {
        if (Player.instance.death) return;
        Vector3 dist1 = Player.instance.transform.position - enemy.transform.position;
        Vector3 dist2 = enemy.transform.forward;
        if (Vector3.Dot(dist1, dist2) < 0)
        {
            print("플레이어가 뒤에 있음");
            return;
        }

        enemy.attack_attackedTime = Time.time;
        enemy.attack_attacked = true;
        Vector3 effectVec = enemy.transform.position - Player.instance.transform.position;
        effectVec.y = 0;
        if (Player.instance == null) return;
        
        //가드중이면 가드한다.
        Player.instance.DoHit(enemy.transform.position,enemy.currentSingleAttackData);

    }
   

    #endregion

    private bool dropped = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (!meshCollider.enabled || dropped) return;
        dropped = true;
    }
}
