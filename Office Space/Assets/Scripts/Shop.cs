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
    [SerializeField] TMP_Text MsgField;
    [SerializeField] TMP_Text RoundBudget;
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
    [SerializeField] AudioClip audShopBuy;
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
        updateRoundBudget();
        //}
    }

    public void updateRoundBudget()
    {
        RoundBudget.text = string.Empty;
        RoundBudget.text = "ROUND " + GameManager.instance.CurrRound.ToString() + " BUDGET:";
        if(GameManager.instance.CurrRound > 1)
        {
            //Display how much they've earned
            MsgField.text = string.Empty;
            MsgField.text = "EARNED: " + GameManager.instance.statsTracker[myPlayer.name].getRTHeld().ToString() + "sec X $120 = $" + GameManager.instance.statsTracker[myPlayer.name].getDepositAmount().ToString(); 
            MsgField.color = Color.yellow;
        }
    }

    public void shotgunButton()
    {
        if (GameManager.instance.statsTracker[myPlayer.name].getMoneyTotal() >= shotgunPrice)
        {
            for (int i = 0; i < playerCT.weaponList.Count; i++)
            {
                if (playerCT.weaponList[i].weaponModel == shotgun.weaponModel && playerCT.weaponList[i].currentAmmo >= playerCT.weaponList[i].startAmmo)
                {
                    return;
                }
            }
            aud.PlayOneShot(audShopBuy, audShopVol);
            playerCT.GetWeaponStats(shotgun);
            GameManager.instance.statsTracker[myPlayer.name].withdrawMoney(shotgunPrice);
            updateMoneyCount();
        }
    }

    public void SMGButton()
    {
        if (GameManager.instance.statsTracker[myPlayer.name].getMoneyTotal() >= SMGPrice)
        {
            for (int i = 0; i < playerCT.weaponList.Count; i++)
            {
                if (playerCT.weaponList[i].weaponModel == SMG.weaponModel && playerCT.weaponList[i].currentAmmo >= playerCT.weaponList[i].startAmmo)
                {
                    return;
                }
            }
            aud.PlayOneShot(audShopBuy, audShopVol);
            playerCT.GetWeaponStats(SMG);
            GameManager.instance.statsTracker[myPlayer.name].withdrawMoney(SMGPrice);
            updateMoneyCount();
        }
    }

    public void rifleButton()
    {
        if (GameManager.instance.statsTracker[myPlayer.name].getMoneyTotal() >= riflePrice)
        {
            for (int i = 0; i < playerCT.weaponList.Count; i++)
            {
                if (playerCT.weaponList[i].weaponModel == rifle.weaponModel && playerCT.weaponList[i].currentAmmo >= playerCT.weaponList[i].startAmmo)
                {
                    return;
                }
            }
            aud.PlayOneShot(audShopBuy, audShopVol);
            playerCT.GetWeaponStats(rifle);
            GameManager.instance.statsTracker[myPlayer.name].withdrawMoney(riflePrice);
            updateMoneyCount();
        }
    }

    public void shurikenButton()
    {
        if (GameManager.instance.statsTracker[myPlayer.name].getMoneyTotal() >= shurikenPrice)
        {          
            for (int i = 0; i < playerCT.weaponList.Count; i++)
            {                
                if (playerCT.weaponList[i].weaponModel == shuriken.weaponModel && playerCT.weaponList[i].currentAmmo >= playerCT.weaponList[i].startAmmo)
                {
                    //Display full ammo message
                    return;
                }
            }
            aud.PlayOneShot(audShopBuy, audShopVol);
            playerCT.GetWeaponStats(shuriken);
            GameManager.instance.statsTracker[myPlayer.name].withdrawMoney(shurikenPrice); 
            updateMoneyCount();
        }        
    }

    public void rubberbandBallButton()
    {
        if (GameManager.instance.statsTracker[myPlayer.name].getMoneyTotal() >= rubberbandBallPrice && myPlayer.GetComponent<ItemThrow>().rubberBallCount < myPlayer.GetComponent<ItemThrow>().GetRubberBallMax())
        {
            aud.PlayOneShot(audShopBuy, audShopVol);
            GameManager.instance.statsTracker[myPlayer.name].withdrawMoney(rubberbandBallPrice);
            updateMoneyCount();
            ItemThrow playerCTI = myPlayer.GetComponent<ItemThrow>();
            if (playerCTI != null)
                playerCTI.rubberBallCount++;
        }
        myPlayer.GetComponent<ItemThrow>().updateGrenadeUI();
    }
}
