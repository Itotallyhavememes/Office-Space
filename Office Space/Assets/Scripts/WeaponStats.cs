using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponStats : ScriptableObject
{
    public GameObject weaponModel;
    public float shootRate;
    public int currentAmmo;
    public float reloadTime;
    public int startAmmo;
}
