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
                        if (GameManager.instance.playerScript.HP < GameManager.instance.playerScript.GetStartHP())
                        {
                            powerupEffect.ApplyBuff();
                        }
                        break;
                    }
                case ItemType.speedBoost:
                    {
                        powerupEffect.ApplyBuff();
                        break;
                    }
                case ItemType.rubberBand:
                    {
                        if (GameManager.instance.playerScript.GetComponent<ItemThrow>().rubberBallCount < GameManager.instance.playerScript.GetComponent<ItemThrow>().GetMaxBallCount())
                        {
                            GameManager.instance.playerScript.GetComponent<ItemThrow>().rubberBallCount++;
                            GameManager.instance.playerScript.GetComponent<ItemThrow>().updateGrenadeUI();
                        }

                        break;
                    }
                case ItemType.weapon:
                    {
                        GameManager.instance.playerScript.GetWeaponStats(this.weapon);

                        break;
                    }
                default:
                    break;
            }
            GameManager.instance.worldItemCount--;
            Destroy(gameObject);


        }

    }
}