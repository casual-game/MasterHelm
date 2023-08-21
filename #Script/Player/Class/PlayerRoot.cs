using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoot : MonoBehaviour
{
    public Player Setting()
    {
        gameObject.SetActive(true);
        return GetComponentInChildren<Player>();
    }
}
