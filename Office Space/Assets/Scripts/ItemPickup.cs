using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] enum itemType { health, speedBoost, ammo }
    [SerializeField] itemType type;
    [SerializeField] float rotationSpeed;
    [SerializeField] int speedModifier;
    [SerializeField] int speedBoostTime;
    [SerializeField] int bobSpeed;
    [SerializeField] float bobHeight;


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
            Debug.Log("Player Collided");

            switch (type)
            {
                case itemType.health:
                    {
                        GameManager.instance.playerScript.HealthPickup();
                        break;
                    }
                case itemType.speedBoost:
                    {
                        //StartCoroutine(GameManager.instance.playerScript.SpeedPowerUp(speedModifier));
                        gameObject.GetComponent<MeshRenderer>().enabled = false;
                        gameObject.GetComponent<CapsuleCollider>().enabled = false;
                        StartCoroutine(SpeedPowerUp(speedModifier));
                        break;
                    }
                case itemType.ammo:
                    break;
                default:
                    break;
            }

            
        }

    }

    IEnumerator SpeedPowerUp(int speedBoost) //Triggers buffs for a good cup of Joe (mediocre office brew)
    {
        GameManager.instance.playerScript.AddSpeed(speedBoost);
        yield return new WaitForSeconds(speedBoostTime);
        GameManager.instance.playerScript.AddSpeed(-speedBoost);
        Destroy(gameObject);

    }
}
