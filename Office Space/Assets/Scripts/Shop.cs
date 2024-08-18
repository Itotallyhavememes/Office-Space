using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] TMP_Text moneyCount;
    [Header("-----Secondaries-----")]
    [SerializeField] int akimboPrice;
    [SerializeField] WeaponStats akimbo;
    [SerializeField] int handCannonPrice;
    [SerializeField] WeaponStats handCannon;
    [SerializeField] int machinePistolPrice;
    [SerializeField] WeaponStats machinePistol;
    [Header("-----Primaries-----")]
    [SerializeField] int shotgunPrice;
    [SerializeField] WeaponStats shotgun;
    [SerializeField] int SMGPrice;
    [SerializeField] WeaponStats SMG;
    [SerializeField] int riflePrice;
    [SerializeField] WeaponStats rifle;
    [SerializeField] int superSoakerPrice;
    [SerializeField] WeaponStats superSoaker;
    [Header("-----Throwables-----")]
    [SerializeField] int shurikenPrice;
    [SerializeField] WeaponStats shuriken;
    [SerializeField] int waterBalloonPrice;
    [SerializeField] WeaponStats waterBalloon;
    [SerializeField] int rubberbandBallPrice;
    [SerializeField] WeaponStats rubberbandBall;
    [SerializeField] int throwable4Price;
    [SerializeField] WeaponStats throwable4;
    [Header("-----Armor-----")]
    [SerializeField] int lightArmorPrice;
    [SerializeField] int heavyArmorPrice;

    Dictionary<String, int> shop = new();

    void Start()
    {
        shop.Add("Akimbo", akimboPrice);
        shop.Add("HandCannon", handCannonPrice);
        shop.Add("machinePistol", machinePistolPrice);
        shop.Add("Shotgun", shotgunPrice);
        shop.Add("SMG", SMGPrice);
        shop.Add("Rifle", riflePrice);
        shop.Add("SuperSoaker", superSoakerPrice);
        shop.Add("Shuriken", shurikenPrice);
        shop.Add("WaterBalloon", waterBalloonPrice);
        shop.Add("RubberbandBall", rubberbandBallPrice);
        shop.Add("Throwable4", throwable4Price);
        shop.Add("LightArmor", lightArmorPrice);
        shop.Add("HeavyArmor", heavyArmorPrice);
        moneyCount.text = "$ " + GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal().ToString();
    }

    public void updateMoneyCount()
    {
        moneyCount.text = "$ " + GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal().ToString();
    }

    public void akimboButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shop["Akimbo"])
            {
                playerMoney -= shop["Akimbo"];
                //add code to add new weapon for secondary
            }
        }
    }

    public void handCannonButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shop["HandCannon"])
            {
                playerMoney -= shop["HandCannon"];
                //add code to add new weapon for secondary
            }
        }
    }
    public void machinePistolButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shop["HandCannon"])
            {
                playerMoney -= shop["HandCannon"];
                //add code to add new weapon for secondary
            }
        }
    }

    public void shotgunButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shop["Shotgun"])
            {
                playerMoney -= shop["Shotgun"];
                //add code to add new weapon for primary
            }
        }
    }

    public void SMGButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shop["SMG"])
            {
                playerMoney -= shop["SMG"];
                //add code to add new weapon for primary
            }
        }
    }

    public void rifleButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shop["Rifle"])
            {
                playerMoney -= shop["Rifle"];
                //add code to add new weapon for primary
            }
        }
    }

    public void superSoakerButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shop["SuperSoaker"])
            {
                playerMoney -= shop["SuperSoaker"];
                //add code to add new weapon for primary
            }
        }
    }
    public void shurikenButton()
    {
        //if (Input.GetMouseButtonDown(0))
        //{ }
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
    public void waterBalloonButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shop["WaterBalloon"])
            {
                playerMoney -= shop["WaterBalloon"];
                //add code to add new weapon to throwables
            }
        }
    }
    public void rubberbandBallButton()
    {
        //if (Input.GetMouseButtonDown(0))
        //{ }
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shop["RubberbandBall"] && GameManager.instance.playerThrowScript.rubberBallCount < GameManager.instance.playerThrowScript.GetRubberBallMax())
            {
                playerMoney -= shop["RubberbandBall"];
                GameManager.instance.playerThrowScript.rubberBallCount++;
            updateMoneyCount();
            //add code to add new weapon to throwables
        }
            GameManager.instance.playerThrowScript.updateGrenadeUI();
        
    }
    public void throwable4Button()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shop["Throwable4"])
            {
                playerMoney -= shop["Throwable4"];
                //add code to add new weapon to throwables
            }
        }
    }
    public void lightArmorButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shop["LightArmor"])
            {
                playerMoney -= shop["LightArmor"];
                //add code to add light armor to player
            }
        }
    }
    public void heavyArmorButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int playerMoney = GameManager.instance.statsTracker[GameManager.instance.player.name].getMoneyTotal();
            if (playerMoney >= shop["HeavyArmor"])
            {
                playerMoney -= shop["HeavyArmor"];
                //add code to add heavy armor to player
            }
        }
    }
}
