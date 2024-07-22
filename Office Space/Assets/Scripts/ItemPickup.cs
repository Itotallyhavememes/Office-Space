using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] enum ItemType { health, speedBoost, shuriken, rubberBand, weapon }
    [SerializeField] ItemType type;
    [SerializeField] float rotationSpeed;
    [SerializeField] int bobSpeed;
    [SerializeField] float bobHeight;
    [SerializeField] PowerUpEffect powerupEffect;
    [SerializeField] WeaponStats weapon;


    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Spin();
        Bob();
    }

    void Spin()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }

    void Bob()
    {
        transform.position = new Vector3(startPos.x, startPos.y + (bobHeight * Mathf.Sin(Time.time * bobSpeed)), startPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (type)
            {
                case ItemType.health:
                    {
                        if (GameManager.instance.playerScript.HP < GameManager.instance.playerScript.GetStartHP())
                        {
                            Destroy(gameObject);
                            powerupEffect.ApplyBuff();
                        }
                        break;
                    }
                case ItemType.speedBoost:
                    {
                        Destroy(gameObject);
                        powerupEffect.ApplyBuff();
                        break;
                    }
                case ItemType.shuriken:
                    {
                        if (GameManager.instance.playerScript.shurikenAmmo < GameManager.instance.playerScript.GetStartShurikenAmmo())
                        {
                            GameManager.instance.playerScript.shurikenAmmo += 5;

                            if (GameManager.instance.playerScript.shurikenAmmo < GameManager.instance.playerScript.GetStartShurikenAmmo())
                            {
                                GameManager.instance.playerScript.shurikenAmmo = GameManager.instance.playerScript.GetStartShurikenAmmo();
                            }

                            GameManager.instance.playerScript.UpdateAmmoUI();
                            Destroy(gameObject);
                        }
                        break;
                    }
                case ItemType.rubberBand:
                    {
                        if (GameManager.instance.playerScript.GetComponent<ItemThrow>().rubberBallCount < GameManager.instance.playerScript.GetComponent<ItemThrow>().GetMaxBallCount())
                        {
                            GameManager.instance.playerScript.GetComponent<ItemThrow>().rubberBallCount++;
                            GameManager.instance.playerScript.GetComponent<ItemThrow>().updateGrenadeUI();
                            Destroy(gameObject);
                        }

                        break;
                    }
                case ItemType.weapon:
                    {
                        if (GameManager.instance.playerScript.GetComponent<ItemThrow>().rubberBallCount < GameManager.instance.playerScript.GetComponent<ItemThrow>().GetMaxBallCount())
                        {
                            
                        }

                        break;
                    }
                default:
                    break;
            }


        }

    }
}