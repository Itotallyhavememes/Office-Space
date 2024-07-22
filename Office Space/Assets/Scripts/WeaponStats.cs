using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
public class WeaponStats : ScriptableObject
{
    public enum WeaponType { raycast, projectile }
    public WeaponType type;
    public GameObject weaponModel;
    public float shootRate;
    public int shootDamage;
    public int currentAmmo;
    public float reloadTime;
    public int startAmmo;

    //public ParticleSystem hitEffect;
    //public AudioClip shootSound;
    //public float shootVol;

    //Raycast Weapon
    public float raycastRotationReload;
    public float raycastRotationRecoil;

    //Projectile Weapon
    public GameObject projectileSpawnPoint;
    public GameObject projectileProjectile;
}
