using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
public class WeaponStats : ScriptableObject
{
    public enum WeaponType { raycast, projectile }
    public enum ThrowStyle { none, overhead, chestOut }
    public WeaponType type;
    public ThrowStyle style;
    public GameObject weaponModel;
    public AudioClip shootSound;
    public float shootRate;
    public int shootDamage;
    public int currentAmmo;
    public float reloadTime;
    public int startAmmo;

    public ParticleSystem hitEffect;
    public float shootVol;

    //Raycast Weapon
    public float raycastDist;
    public float raycastRotationReload;
    public float raycastRotationRecoil;

    //Projectile Weapon
    public GameObject projectileProjectile;
}
