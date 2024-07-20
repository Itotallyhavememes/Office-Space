using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShurikenStats : ScriptableObject
{
    public GameObject shurikenHUD;
    public GameObject shurikenSpawnPoint;
    public GameObject shurikenProjectile;
    public float shurikenRate;
    public int shurikenAmmo;
    public float shurikenReloadTime;
    public int shurikenStartAmmo;

}
