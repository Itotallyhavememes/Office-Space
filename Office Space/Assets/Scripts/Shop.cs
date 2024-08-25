using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public static Shop instance;

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

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        moneyCount.text = "$ " + GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal().ToString();
    }

    public void updateMoneyCount()
    {
        moneyCount.text = "$ " + GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal().ToString();
    }

    public void shotgunButton()
    {
        if (GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal() >= shotgunPrice)
        {
            GameManager.instance.statsTracker[GameManager.instance.player.name].withdrawMoney(shotgunPrice);
            updateMoneyCount();
            for (int i = 0; i < GameManager.instance.playerScript.weaponList.Count; i++)
            {
                if (GameManager.instance.playerScript.weaponList[i] == shotgun)
                {
                    GameManager.instance.playerScript.weaponList[i].currentAmmo = GameManager.instance.playerScript.weaponList[i].startAmmo;
                    return;
                }
            }
            GameManager.instance.playerScript.GetWeaponStats(shotgun);
        }
    }

    public void SMGButton()
    {
        if (GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal() >= SMGPrice)
        {
            GameManager.instance.statsTracker[GameManager.instance.player.name].withdrawMoney(SMGPrice);
            updateMoneyCount();
            for (int i = 0; i < GameManager.instance.playerScript.weaponList.Count; i++)
            {
                if (GameManager.instance.playerScript.weaponList[i] == SMG)
                {
                    GameManager.instance.playerScript.weaponList[i].currentAmmo = GameManager.instance.playerScript.weaponList[i].startAmmo;
                    return;
                }
            }
            GameManager.instance.playerScript.GetWeaponStats(SMG);
        }
    }

    public void rifleButton()
    {
        if (GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal() >= riflePrice)
        {
            GameManager.instance.statsTracker[GameManager.instance.player.name].withdrawMoney(riflePrice);
            updateMoneyCount();
            for (int i = 0; i < GameManager.instance.playerScript.weaponList.Count; i++)
            {
                if (GameManager.instance.playerScript.weaponList[i] == rifle)
                {
                    GameManager.instance.playerScript.weaponList[i].currentAmmo = GameManager.instance.playerScript.weaponList[i].startAmmo;
                    return;
                }
            }
            GameManager.instance.playerScript.GetWeaponStats(rifle);
        }
    }

    public void shurikenButton()
    {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shurikenPrice)
            {
                GameManager.instance.statsTracker[GameManager.instance.player.name].withdrawMoney(shurikenPrice);
                GameManager.instance.playerScript.GetWeaponStats(shuriken);
                updateMoneyCount();
                //add code to add new weapon to throwables
            }
        
    }
    public void rubberbandBallButton()
    {
        int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
        if (playerMoney >= rubberbandBallPrice && GameManager.instance.playerThrowScript.rubberBallCount < GameManager.instance.playerThrowScript.GetRubberBallMax())
        {
            GameManager.instance.statsTracker[GameManager.instance.player.name].withdrawMoney(rubberbandBallPrice);
            GameManager.instance.playerThrowScript.rubberBallCount++;
            updateMoneyCount();
            //add code to add new weapon to throwables
        }
            GameManager.instance.playerThrowScript.updateGrenadeUI();
    }
}
