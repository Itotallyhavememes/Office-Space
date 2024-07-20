using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/SpeedBuff")]
public class CoffeeStats : PowerUpEffect
{

    public int speedModifier;
    public int speedBoostTime;

    //public AudioClip pickupSFX; //For future add
    //[Range(0, 1)][SerializeField] float audPickupVol;

    public override void ApplyBuff()
    {
        //GameManager.instance.player.GetComponent<AudioSource>().PlayOneShot(pickupSFX, audPickupVol);
        GameManager.instance.playerScript.ActivateSpeedBoost(this);


    }

}