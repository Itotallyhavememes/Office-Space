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
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shotgunPrice)
            {
                playerMoney -= shotgunPrice;
                //add code to add new weapon for primary
            }
        }
    }

    public void SMGButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= SMGPrice)
            {
                playerMoney -= SMGPrice;
                //add code to add new weapon for primary
            }
        }
    }

    public void rifleButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= riflePrice)
            {
                playerMoney -= riflePrice;
                //add code to add new weapon for primary
            }
        }
    }

    public void shurikenButton()
    {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shurikenPrice)
            {
                Debug.Log("Shuriken bought");
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
