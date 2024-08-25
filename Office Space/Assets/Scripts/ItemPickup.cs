using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] enum ItemType { health, speedBoost, rubberBand, weapon }
    [SerializeField] ItemType type;
    [SerializeField] float rotationSpeed;
    [SerializeField] int bobSpeed;
    [SerializeField] float bobHeight;
    [SerializeField] PowerUpEffect powerupEffect;
    [SerializeField] WeaponStats weapon;

    [Header("----- Sounds -----")]
    [SerializeField] AudioClip pickUpSound;
    [SerializeField] AudioSource audSource;
    [Range(0, 1)][SerializeField] float volume;

    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.worldItemCount++;
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
                        if (!GameManager.instance.isMultiplayer)
                        {
                            if (GameManager.instance.playerScript.HP < GameManager.instance.playerScript.GetStartHP())
                            {

                                GameManager.instance.worldItemCount--;
                                powerupEffect.ApplyBuff(other.gameObject);
                                audSource.PlayOneShot(pickUpSound);
                                Destroy(gameObject);
                            }
                        }
                        else
                        {
                            ControllerTest multiplayerControlsScript = other.GetComponent<ControllerTest>();
                            if (multiplayerControlsScript.HP < multiplayerControlsScript.GetStartHP())
                            {
                                GameManager.instance.worldItemCount--;
                                powerupEffect.ApplyBuff(other.gameObject);
                                audSource.PlayOneShot(pickUpSound);
                                Destroy(gameObject);
                            }
                        }
                        break;
                    }
                case ItemType.speedBoost:
                    {
                        if (!GameManager.instance.isMultiplayer)
                        {
                            GameManager.instance.playerScript.Munch(pickUpSound, volume);
                            powerupEffect.ApplyBuff(other.gameObject);
                            GameManager.instance.worldItemCount--;
                            Destroy(gameObject);
                        }
                        else
                        {
                            other.GetComponent<ControllerTest>().Munch(pickUpSound, volume);
                            powerupEffect.ApplyBuff(other.gameObject);
                            GameManager.instance.worldItemCount--;
                            Destroy(gameObject);
                        }

                        break;
                    }
                case ItemType.rubberBand:
                    {
                        if (!GameManager.instance.isMultiplayer)
                        {
                            if (GameManager.instance.playerScript.GetComponent<ItemThrow>().rubberBallCount < GameManager.instance.playerScript.GetComponent<ItemThrow>().GetMaxBallCount())
                            {
                                GameManager.instance.playerScript.Munch(pickUpSound, volume);
                                GameManager.instance.playerScript.GetComponent<ItemThrow>().rubberBallCount++;
                                GameManager.instance.playerScript.GetComponent<ItemThrow>().updateGrenadeUI();
                                GameManager.instance.worldItemCount--;
                                Destroy(gameObject);
                            }
                        }
                        else
                        {
                            ItemThrow multiplayerItemThrow = other.GetComponent<ItemThrow>();
                            if (multiplayerItemThrow.rubberBallCount < multiplayerItemThrow.GetMaxBallCount())
                            {
                                other.GetComponent<ControllerTest>().Munch(pickUpSound, volume);
                                multiplayerItemThrow.rubberBallCount++;
                                multiplayerItemThrow.updateGrenadeUI();
                                GameManager.instance.worldItemCount--;
                                Destroy(gameObject);
                            }
                        }

                        break;
                    }
                case ItemType.weapon:
                    {
                        if (!GameManager.instance.isMultiplayer)
                        {
                            GameManager.instance.playerScript.Munch(pickUpSound, volume);
                            GameManager.instance.playerScript.GetWeaponStats(this.weapon);
                            GameManager.instance.worldItemCount--;
                        }
                        else
                        {
                            other.GetComponent<ControllerTest>().Munch(pickUpSound, volume);
                            other.GetComponent<ControllerTest>().GetWeaponStats(this.weapon);
                            GameManager.instance.worldItemCount--;
                        }
                        Destroy(gameObject);

                        break;
                    }
                default:
                    break;
            }


        }

    }
}