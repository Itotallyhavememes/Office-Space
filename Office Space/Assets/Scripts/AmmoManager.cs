using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoManager : MonoBehaviour
{
    float Ammo, maxAmmo;
    public Image ammoImage;
    // Start is called before the first frame update
    void Start()
    {
        Ammo = maxAmmo = GameManager.instance.playerAmmo;
        ammoImage.fillAmount = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Shoot();
        }
    }
    public void Shoot()
    {
        --Ammo;
        if (Ammo >= 0f)
        {
            GameManager.instance.playerAmmoBar.fillAmount = (float)Ammo / maxAmmo;
        }
        if(Input.GetKeyDown(KeyCode.R)) 
        {
            Ammo = maxAmmo;
        }
        
    }
}