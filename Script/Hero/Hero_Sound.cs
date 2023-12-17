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
        sound_footstep_concrete_turn,
        sound_footstep_stone_turn,
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
        sound_voice_attack_strong;

    private float _time_footstep,_time_weapon_spawn,_time_weapon_despawn;

    private void Setting_Sound()
    {
        SoundManager.Add(sound_voice_attack_normal);
        SoundManager.Add(sound_voice_attack_strong);
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
        bool isHit = Physics.Raycast(footPos + Vector3.up*0.5f, Vector3.down, 
            out RaycastHit hit,1,mapLayer,QueryTriggerInteraction.Collide);
        if (isHit)
        {
            if(hit.collider.CompareTag(GameManager.s_stone)) SoundManager.Play(sound_footstep_stone);
            else if(hit.collider.CompareTag(GameManager.s_concrete)) SoundManager.Play(sound_footstep_concrete);
        }
    }
    public void Sound_Footstep()
    {
        Vector3 footPos = p_footstep_l.transform.position;
        bool isHit = Physics.Raycast(footPos + Vector3.up*0.5f, Vector3.down, 
            out RaycastHit hit,1,mapLayer,QueryTriggerInteraction.Collide);
        if (isHit)
        {
            if(hit.collider.CompareTag(GameManager.s_stone)) SoundManager.Play(sound_footstep_stone);
            else if(hit.collider.CompareTag(GameManager.s_concrete)) SoundManager.Play(sound_footstep_concrete);
        }
    }
    public void Sound_Footstep_Turn()
    {
        Vector3 footPos = p_footstep_l.transform.position;
        bool isHit = Physics.Raycast(footPos + Vector3.up*0.5f, Vector3.down, 
            out RaycastHit hit,1,mapLayer,QueryTriggerInteraction.Collide);
        if (isHit)
        {
            if(hit.collider.CompareTag(GameManager.s_stone)) SoundManager.Play(sound_footstep_stone_turn);
            else if(hit.collider.CompareTag(GameManager.s_concrete)) SoundManager.Play(sound_footstep_concrete_turn);
        }
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
}
