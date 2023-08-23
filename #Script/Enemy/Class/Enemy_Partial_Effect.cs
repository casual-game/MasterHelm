using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public partial class Enemy : MonoBehaviour
{
    [HideInInspector] public ParticleSystem particle_guard, particle_parrying,particle_smoke,particle_charge;
    
    protected virtual void FirstSetting_Effect()
    {
        Transform t = transform.Find("Particle");
        particle_guard = t.Find("Guard").GetComponent<ParticleSystem>();
        particle_parrying = t.Find("Parrying").GetComponent<ParticleSystem>();
        particle_smoke = t.Find("Smoke").GetComponent<ParticleSystem>();
        particle_charge = t.Find("Charge").GetComponent<ParticleSystem>();
    }
    //정말 파티클만. 아래 Effect와 중복사용 가능
    public void Particle_Hit_Base()
    {
        GameObject effect_hit = Manager_Pooler.instance.Get("Weapon_Hit_Effect");

        Vector3 particleVec = Player.instance.transform.position - transform.position;
        particleVec.y = 0; particleVec.Normalize(); particleVec *= 0.5f;

        effect_hit.transform.position = (transform.position + Vector3.up * 1.2f) + particleVec;
        effect_hit.transform.rotation = Quaternion.Euler(hitWeaponRot);
        effect_hit.transform.localScale = Vector3.one * 0.35f;
        effect_hit.SetActive(true);
    }
    public void Particle_Hit_Revenge()
    {
        GameObject effect_hit = Manager_Pooler.instance.Get("Weapon_RevengeHit_Effect");
        Vector3 particleVec = Player.instance.transform.position - transform.position;
        particleVec.y = 0; particleVec.Normalize(); particleVec *= 0.5f;
        Vector3 lookRot = hitWeaponRot;
        lookRot.x = 0;
        lookRot.z = 0;
        
        effect_hit.transform.position = (transform.position + Vector3.up * 1.2f) + particleVec;
        effect_hit.transform.rotation = Quaternion.Euler(lookRot);
        effect_hit.SetActive(true);
    }
    protected void Particle_Blood_Normal()
    {
        GameObject blood = Manager_Blood.instance.Blood_Hit();
        blood.transform.position = transform.position + Vector3.up * 1.5f;
        Vector3 particleVec = transform.position - Player.instance.transform.position;
        particleVec.y = 0;
        if (blood != null)
        {
            blood.transform.rotation = Quaternion.LookRotation(particleVec)*Quaternion.Euler(0,-90,0);
            blood.SetActive(true);
        }
    }
    protected void Particle_Blood_Smash()
    {
        GameObject blood = Manager_Blood.instance.Blood_Smash();
        blood.transform.position = transform.position + Vector3.up * 1.5f;
        Vector3 particleVec = transform.position - Player.instance.transform.position;
        particleVec.y = 0;
        if (blood != null)
        {
            blood.transform.rotation = Quaternion.LookRotation(particleVec)*Quaternion.Euler(0,-90,0);
            blood.SetActive(true);
        }
        
    }
    public void Particle_Text_Damage(float damage)
    {
        Vector3 numPos = transform.position - Player.instance.transform.position;
        numPos.y = 0;
        numPos = transform.position + numPos.normalized * 0.5f;
        Manager_Main.instance.Text_Num(numPos, damage);
    }
    //Effct_~~ 끼리는 중복사용할 수 없다.
    public void Effect_Hit_Normal()
    {
        //sfx
        HitVoice();
        Player.instance.audio_Hit_Gore.Play();
        Player.instance.audio_Hit_Impact.Play();
        //vfx
        particle_guard.Play();
        Particle_Blood_Normal();
        Particle_Hit_Base();
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Hit);
        //Text
        Particle_Text_Damage(Random.Range(20, 50));
        Manager_Main.instance.Text_Damage_Main();
    }
    public void Effect_Hit_Strong()
    {
        //sfx
        HitVoice();
        Player.instance.audio_Hit_Gore.Play();
        Player.instance.audio_Hit_Impact.Play();
        //vfx
        particle_parrying.Play();
        Particle_Blood_Smash();
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Smash);
        //Text
        Particle_Text_Damage(Random.Range(50, 75));
        Vector3 numPos = transform.position - Player.instance.transform.position;
        numPos = transform.position + numPos.normalized * 0.5f + Vector3.up;
        Manager_Main.instance.Text_Big(numPos, "SMASH");
        Manager_Main.instance.Text_Damage_Main();
        Manager_Main.instance.Text_Damage_Specific("smash");
    }
    public void Effect_Guard()
    {
        //sfx
        HitVoice();
        Player.instance.audio_Hit_Impact.Play();
        //vfx
        particle_guard.Play();
        Particle_Blood_Normal();
        Particle_Hit_Base();
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Hit);
        //Text
        Vector3 numPos = transform.position - Player.instance.transform.position;
        numPos = transform.position + numPos.normalized * 0.5f + Vector3.up;
        Manager_Main.instance.Text_Danger(numPos, "GUARD");
        Manager_Main.instance.Text_Damage_Main();
        
    }
    public void Effect_Hit_Counter()
    {
        //sfx
        HitVoice();
        Player.instance.audio_Hit_Notice.Play();
        Player.instance.audio_Hit_Gore.Play();
        Player.instance.audio_Hit_Impact.Play();
        //vfx
        particle_parrying.Play();
        Particle_Blood_Smash();
        Particle_Hit_Revenge();
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Smash);
        //Text
        Particle_Text_Damage(Random.Range(75, 100));
        Vector3 numPos = transform.position - Player.instance.transform.position;
        numPos = transform.position + numPos.normalized * 0.5f + Vector3.up;
        Manager_Main.instance.Text_Big(numPos, "COUNTER");
        Manager_Main.instance.Text_Damage_Main();
        Manager_Main.instance.Text_Damage_Specific("counter");
    }
    public void Effect_Hit_Revenge()
    {
        //sfx
        HitVoice();
        Player.instance.audio_Hit_Special.Play();
        Player.instance.audio_Hit_Gore.Play();
        Player.instance.audio_Hit_Impact.Play();
        //vfx
        CamArm.instance.speedline_loop.Stop();
        particle_parrying.Play();
        Particle_Hit_Revenge();
        Particle_Blood_Smash();
        Player.instance.Particle_FireRing(transform.position + Vector3.up*0.75f);
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_SpecialHit);
        Manager_Pooler.instance.Shockwave(transform.position);
        Player.instance.Particle_JustParrying();
        //Text
        Particle_Text_Damage(Random.Range(100, 200));
        Vector3 numPos = transform.position - Player.instance.transform.position;
        numPos = transform.position + numPos.normalized * 0.5f + Vector3.up;
        Manager_Main.instance.Text_Highlight(numPos, "PUNISH");
        Manager_Main.instance.Text_Damage_Main();
        Manager_Main.instance.Text_Damage_Specific("punish");
    }
    public void Effect_Hit_GuardBreak(bool isBoss = false)
    {
        //sfx
        if(isBoss) audio_death.Play();
        else HitVoice();
        Player.instance.audio_Hit_Special.Play();
        Player.instance.audio_Hit_Gore.Play();
        Player.instance.audio_Hit_Impact.Play();
        //vfx
        //CamArm.instance.SpeedLine_Play(false);
        particle_parrying.Play();
        Particle_Hit_Revenge();
        Particle_Blood_Smash();
        Player.instance.Particle_FireRing(transform.position + Vector3.up*0.75f);
        if(isBoss) CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Boss_SpecialHit);
        else CamArm.instance.Impact(Manager_Main.instance.mainData.impact_SpecialHit);
        Manager_Pooler.instance.Shockwave(transform.position);
        //Text
        Particle_Text_Damage(Random.Range(100, 200));
        Vector3 numPos = transform.position - Player.instance.transform.position;
        numPos = transform.position + numPos.normalized * 0.5f + Vector3.up;
        Manager_Main.instance.Text_Highlight(numPos, "GUARD BREAK");
        Manager_Main.instance.Text_Damage_Main();
        Manager_Main.instance.Text_Damage_Specific("guard break");
    }
    public void Effect_Death(bool isBoss = false)
    {
        if (enemies.Contains(this)) enemies.Remove(this);
        if (enemies.Count == 0) Player.instance.executedTarget = this;
        //sfx
        audio_death.Play();
        Player.instance.audio_Hit_Finish.Play();
        Player.instance.audio_Hit_Gore.Play();
        Player.instance.audio_Hit_Impact.Play();
        //vfx
        CamArm.instance.speedline_loop.Stop();
        Manager_Main.instance.Spawner_EnemyKill(isBoss,this);


        particle_parrying.Play();
        Particle_Hit_Revenge();
        Particle_Blood_Smash();
        Player.instance.Particle_FireRing(transform.position + Vector3.up * 0.75f);

        Manager_Pooler.instance.Shockwave(transform.position);
        Player.instance.Particle_JustParrying();
        

        Particle_Text_Damage(Random.Range(100, 200));
        Vector3 numPos = transform.position - Player.instance.transform.position;
        numPos = transform.position + numPos.normalized * 0.5f + Vector3.up;
        Manager_Main.instance.Text_Highlight(numPos, "EXECUTE");

    }
}
