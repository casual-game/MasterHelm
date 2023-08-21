using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : BaseStart
{
    public PlayerRoot player;
    public Data_Shield shield;
    public Data_Weapon weapon;
    public Data_Bow bow;
    public Data_Weapon skillL;
    public Data_Weapon skillR;
    public Player Setting()
    {
        if(player.gameObject.activeSelf) player.gameObject.SetActive(false);
        
        PlayerRoot root = Instantiate(player,transform.position,transform.rotation);
        Player created = root.Setting();
        created.data_Shield = shield;
        created.data_Weapon_Main = weapon;
        created.data_Bow = bow;
        created.data_Weapon_SkillL = skillL;
        created.data_Weapon_SkillR = skillR;
        created.gameObject.SetActive(true);
        created.Setting();
        return created;
    }
}
