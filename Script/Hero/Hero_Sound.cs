using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public partial class Hero : MonoBehaviour
{
    [FoldoutGroup("Sound")] public LayerMask mapLayer;
    [FoldoutGroup("Sound")] public SoundData
        sound_footstep_concrete,
        sound_footstep_stone,
        sound_footstep_grass,
        sound_footstep_concrete_turn,
        sound_footstep_stone_turn,
        sound_footstep_grass_turn,
        sound_footstep_roll_begin,
        sound_footstep_roll_fin,
        sound_friction_cloth,
        sound_chain,
        sound_combat_chargebegin,
        sound_combat_chargefin,
        sound_combat_superarmor,
        sound_combat_skill,
        sound_combat_groundsmash,
        sound_weapon_spawn,
        sound_weapon_despawn,
        sound_voice_attack_normal,
        sound_voice_attack_strong,
        sound_voice_short;

    private float _time_footstep,_time_weapon_spawn,_time_weapon_despawn,_time_voice;
    private List<Collider> grassColls = new List<Collider>();
    private RaycastHit[] footstepHits = new RaycastHit[7];
    private void Setting_Sound()
    {
        SoundManager.Add(sound_voice_attack_normal);
        SoundManager.Add(sound_voice_attack_strong);
        SoundManager.Add(sound_voice_short);
    }
    private void Sound_Weapon_Spawn()
    {
        if (Time.unscaledTime - _time_weapon_spawn < 0.3f) return;
        _time_weapon_spawn = Time.unscaledTime;
        SoundManager.Play(sound_weapon_spawn);
    }
    private void Sound_Weapon_Despawn()
    {
        if (Time.unscaledTime - _time_weapon_despawn < 0.3f) return;
        _time_weapon_despawn = Time.unscaledTime;
        SoundManager.Play(sound_weapon_despawn);
    }
    public void Sound_Footstep(Vector3 footPos)
    {
        if (Time.unscaledTime - _time_footstep < 0.3f) return;
        _time_footstep = Time.unscaledTime;
        Physics.RaycastNonAlloc(footPos + Vector3.up*0.75f, Vector3.down,footstepHits,1.25f,mapLayer);
        float height = Mathf.NegativeInfinity;
        SoundData sound= null;
        if (grassColls.Count > 0) sound = sound_footstep_grass;
        else
        {
            foreach (var hit in footstepHits)
            {
                if(hit.collider == null ||hit.point.y<height) continue;
                if (hit.collider.CompareTag(GameManager.s_stone))
                {
                    sound = sound_footstep_stone;
                    height = hit.point.y;
                }
                else if (hit.collider.CompareTag(GameManager.s_concrete))
                {
                    sound = sound_footstep_concrete;
                    height = hit.point.y;
                }
            } 
        }
        
        if(sound!=null) SoundManager.Play(sound);
    }
    public void Sound_Footstep()
    {
       Sound_Footstep(p_footstep_l.transform.position);
    }
    public void Sound_Footstep_Turn()
    {
        Vector3 footPos = p_footstep_l.transform.position;
        
        Physics.RaycastNonAlloc(footPos + Vector3.up*0.75f, Vector3.down,footstepHits,1.25f,mapLayer);
        float height = Mathf.NegativeInfinity;
        SoundData sound= null;
        if (grassColls.Count > 0) sound = sound_footstep_grass_turn;
        else
        {
            foreach (var hit in footstepHits)
            {
                if(hit.collider == null ||hit.point.y<height) continue;
            
                if (hit.collider.CompareTag(GameManager.s_stone))
                {
                    sound = sound_footstep_stone_turn;
                    height = hit.point.y;
                }
                else if (hit.collider.CompareTag(GameManager.s_concrete))
                {
                    sound = sound_footstep_concrete_turn;
                    height = hit.point.y;
                }
            }
        }
       
        if(sound!=null) SoundManager.Play(sound);
    }

    public void Sound_Voice_Attack_Normal()
    {
        if (Time.unscaledTime - _time_voice < 0.75f) return;
        _time_voice = Time.unscaledTime;
        SoundManager.Play(sound_voice_attack_normal);
    }
    public void Sound_Voice_Attack_Strong()
    {
        _time_voice = Time.unscaledTime;
        SoundManager.Play(sound_voice_attack_strong);
    }
    public void Sound_Voice_Short()
    {
        if (Time.unscaledTime - _time_voice < 0.5f) return;
        _time_voice = Time.unscaledTime;
        SoundManager.Play(sound_voice_short);
    }
    //AnimationEvent
    public void Sound_Footstep_Roll_Fin()
    {
        SoundManager.Play(sound_footstep_roll_fin);
    }
    public void Sound_GroundSmash()
    {
        SoundManager.Play(sound_combat_groundsmash);
    }
    //Grass
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(GameManager.s_grass) && !grassColls.Contains(other)) grassColls.Add(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(GameManager.s_grass) && grassColls.Contains(other)) grassColls.Remove(other);
    }
}
