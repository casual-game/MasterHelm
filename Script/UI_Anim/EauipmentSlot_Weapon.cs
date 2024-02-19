using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class EauipmentSlot_Weapon : MonoBehaviour
{
    public Image icon;
    [ReadOnly] public Item_Weapon weapon;
    [Button]
    public void ChangeData(Item_Weapon _weapon)
    {
        weapon = _weapon;
        icon.sprite = weapon.icon;
        icon.rectTransform.offsetMin = new Vector2(weapon.left, weapon.bottom);
        icon.rectTransform.offsetMax = new Vector2(-weapon.right, -weapon.top);
        icon.rectTransform.localScale = weapon.scale;
    }
}
