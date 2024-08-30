using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shop : MonoBehaviour
{
    //public static Shop instance;
    public int test;
    [SerializeField] GameObject myPlayer;
    [SerializeField] ControllerTest playerCT;
    public int myPlayerBudget;
    //[SerializeField] ControllerTest playersWeaponList;
    [SerializeField] TMP_Text moneyCount;
    [Header("-----Primaries-----")]
    [SerializeField] int shotgunPrice;
    [SerializeField] WeaponStats shotgun;
    [SerializeField] int SMGPrice;
    [SerializeField] WeaponStats SMG;
    [SerializeField] int riflePrice;
    [SerializeField] WeaponStats rifle;
    [Header("-----Throwables-----")]
    [SerializeField] int shurikenPrice;
    [SerializeField] WeaponStats shuriken;
    [SerializeField] int rubberbandBallPrice;
    [SerializeField] WeaponStats rubberbandBall;
    [Header("----- Sounds -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip audShopFire;
    [Range(0, 1)][SerializeField] float audShopVol;
    //private void Awake()
    //{
    //    //instance = this;
    //}

    void Start()
    {
        //moneyCount.text = "$ " + GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal().ToString();
        if(myPlayer != null)
            playerCT = myPlayer.GetComponent<ControllerTest>();
    }

    public void updateMoneyCount()
    {
        //foreach (var player in PlayerManager.instance.players)
        //{
        moneyCount.text = string.Empty;
        moneyCount.text = GameManager.instance.statsTracker[myPlayer.name].getMoneyTotal().ToString();
        //}
    }

    public void shotgunButton()
    {
        //When calling this method, needs to pass in whoever called this function
        
        if (GameManager.instance.statsTracker[myPlayer.name].getMoneyTotal() >= shotgunPrice)
        {
            aud.PlayOneShot(audShopFire, audShopVol);
            Debug.Log("Buying Shotgun");
            GameManager.instance.statsTracker[myPlayer.name].withdrawMoney(shotgunPrice);
            updateMoneyCount();
            //ControllerTest playerCT = myPlayer.GetComponent<ControllerTest>();
            for (int i = 0; i < playerCT.weaponList.Count; i++)
            {
                Debug.Log("Searching Weapons at Index: " + i.ToString());
                if (playerCT.weaponList[i].weaponModel == shotgun.weaponModel)
                {
                    Debug.Log("You already have Shotgun");
                    //GameManager.instance.playerScript.weaponList[i].currentAmmo = GameManager.instance.playerScript.weaponList[i].startAmmo;
                    Debug.Log("Refilling Shotgun Ammo");
                    playerCT.weaponList[i].currentAmmo = playerCT.weaponList[i].startAmmo;
                    return;
                }
            }
            playerCT.GetWeaponStats(shotgun);
        }
    }

    public void SMGButton()
    {
        if (GameManager.instance.statsTracker[myPlayer.name].getMoneyTotal() >= SMGPrice)
        {
            aud.PlayOneShot(audShopFire, audShopVol);
            GameManager.instance.statsTracker[myPlayer.name].withdrawMoney(SMGPrice);
            updateMoneyCount();
            //ControllerTest playerCT = myPlayer.GetComponent<ControllerTest>();
            for (int i = 0; i < playerCT.weaponList.Count; i++)
            {
                if (playerCT.weaponList[i].weaponModel == SMG.weaponModel)
                {
                    //GameManager.instance.playerScript.weaponList[i].currentAmmo = GameManager.instance.playerScript.weaponList[i].startAmmo;
                    playerCT.weaponList[i].currentAmmo = playerCT.weaponList[i].startAmmo;
                    return;
                }
            }
            playerCT.GetWeaponStats(SMG);
        }
    }

    public void rifleButton()
    {
        if (GameManager.instance.statsTracker[myPlayer.name].getMoneyTotal() >= riflePrice)
        {
            aud.PlayOneShot(audShopFire, audShopVol);
            GameManager.instance.statsTracker[myPlayer.name].withdrawMoney(riflePrice);
            updateMoneyCount();
           // ControllerTest playerCT = myPlayer.GetComponent<ControllerTest>();
            for (int i = 0; i < playerCT.weaponList.Count; i++)
            {
                if (playerCT.weaponList[i].weaponModel == rifle.weaponModel)
                {
                    //GameManager.instance.playerScript.weaponList[i].currentAmmo = GameManager.instance.playerScript.weaponList[i].startAmmo;
                    playerCT.weaponList[i].currentAmmo = playerCT.weaponList[i].startAmmo;
                    return;
                }
            }
            playerCT.GetWeaponStats(rifle);
            //GameManager.instance.playerScript.GetWeaponStats(rifle);
        }
    }

    public void shurikenButton()
    {

        if (GameManager.instance.statsTracker[myPlayer.name].getMoneyTotal() >= shurikenPrice)
        {
            Debug.Log("BUYING SHURIKEN!");
            aud.PlayOneShot(audShopFire, audShopVol);
            GameManager.instance.statsTracker[myPlayer.name].withdrawMoney(shurikenPrice);
            //GameManager.instance.playerScript.GetWeaponStats(shuriken);
            updateMoneyCount();

            //ControllerTest playerCT = myPlayer.GetComponent<ControllerTest>();
            for (int i = 0; i < playerCT.weaponList.Count; i++)
            {
                
                Debug.Log("SEARCHING WEAPONS AT INDEX: " + i.ToString());
                if (playerCT.weaponList[i].weaponModel == shuriken.weaponModel)
                {
                    Debug.Log("You have a Shuriken Already");
                    if (playerCT.weaponList[i].currentAmmo < playerCT.weaponList[i].startAmmo)
                    {
                        Debug.Log("refilling Shuriken");
                        playerCT.weaponList[i].currentAmmo = playerCT.weaponList[i].startAmmo;
                        return;
                    }
                    //else { break; }
                    //GameManager.instance.playerScript.weaponList[i].currentAmmo = GameManager.instance.playerScript.weaponList[i].startAmmo;
                }
            }
            playerCT.GetWeaponStats(shuriken);
            //add code to add new weapon to throwables
        }
        
    }
    public void rubberbandBallButton()
    {
        if (GameManager.instance.statsTracker[myPlayer.name].getMoneyTotal() >= rubberbandBallPrice && myPlayer.GetComponent<ItemThrow>().rubberBallCount < myPlayer.GetComponent<ItemThrow>().GetRubberBallMax())
        {
            aud.PlayOneShot(audShopFire, audShopVol);
            GameManager.instance.statsTracker[myPlayer.name].withdrawMoney(rubberbandBallPrice);
            //GameManager.instance.playerThrowScript.rubberBallCount++;
            updateMoneyCount();
            //add code to add new weapon to throwables

            ItemThrow playerCTI = myPlayer.GetComponent<ItemThrow>();
            if (playerCTI != null)
                playerCTI.rubberBallCount++;
                    //GameManager.instance.playerScript.weaponList[i].currentAmmo = GameManager.instance.playerScript.weaponList[i].startAmmo;
                    

        }
        //GameManager.instance.playerThrowScript.updateGrenadeUI();
        myPlayer.GetComponent<ItemThrow>().updateGrenadeUI();
    }
}
