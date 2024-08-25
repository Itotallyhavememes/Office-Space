using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/HealthRestore")]
public class HealthRestore : PowerUpEffect
{
    public int HpRestoreAmount;
    public AudioClip pickupSFX;
    [Range(0, 1)][SerializeField] float audPickupVol;

    public override void ApplyBuff(GameObject player)
    {
        player.GetComponent<AudioSource>().PlayOneShot(pickupSFX, audPickupVol);
        player.GetComponent<ControllerTest>().HealthPickup(HpRestoreAmount);
    }
}
