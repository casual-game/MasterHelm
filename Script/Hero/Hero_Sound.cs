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
        sound_friction_cloth,
        sound_chain,
        sound_combat_chargebegin,
        sound_combat_chargefin,
        sound_combat_superarmor,
        sound_combat_skill;

    private float _time_footstep;
    private void Sound_Footstep(Vector3 footPos)
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
}
