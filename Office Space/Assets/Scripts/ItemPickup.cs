using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] enum itemType { health, speedBoost, shuriken, rubberBand }
    [SerializeField] itemType type;
    [SerializeField] float rotationSpeed;
    [SerializeField] int bobSpeed;
    [SerializeField] float bobHeight;
    [SerializeField] PowerUpEffect powerupEffect;


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
                case itemType.health:
                    {
                        if (GameManager.instance.playerScript.HP < GameManager.instance.playerScript.GetStartHP())
                        {
                            Destroy(gameObject);
                            powerupEffect.ApplyBuff();
                        }
                        break;
                    }
                case itemType.speedBoost:
                    {
                        Destroy(gameObject);
                        powerupEffect.ApplyBuff();
                        break;
                    }
                case itemType.shuriken:
                    {
                        if (GameManager.instance.playerScript.shurikenAmmo < GameManager.instance.playerScript.GetStartShurikenAmmo())
                        {
                            GameManager.instance.playerScript.shurikenAmmo++;
                            GameManager.instance.playerScript.UpdateAmmoUI();
                            Destroy(gameObject);
                        }
                        break;
                    }
                case itemType.rubberBand:
                    {
                        if (GameManager.instance.playerScript.GetComponent<ItemThrow>().rubberBallCount < GameManager.instance.playerScript.GetComponent<ItemThrow>().GetMaxBallCount())
                        {
                            GameManager.instance.playerScript.GetComponent<ItemThrow>().rubberBallCount++;
                            GameManager.instance.playerScript.GetComponent<ItemThrow>().updateGrenadeUI();
                            Destroy(gameObject);
                        }

                        break;
                    }
                default:
                    break;
            }


        }

    }
}