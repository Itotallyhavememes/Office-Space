using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UIElements;

public class DonutPickUp : MonoBehaviour
{

    public static event Action OnCollected;

    public static int totalDonuts;
    public static int HPincrease;

    [SerializeField] int donutQty;
    [SerializeField] int HpRestoreAmount;
    [SerializeField] float rotationSpeed;
    [SerializeField] int bobSpeed;
    [SerializeField] float bobHeight;
    [SerializeField] AudioClip pickupSFX;
    [Range(0, 1)][SerializeField] float audPickupVol;

    Vector3 startPos;

    private void Awake()
    {
        totalDonuts++;
        HPincrease = HpRestoreAmount;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.worldDonutCount++;
        startPos = transform.position;
        GameManager.instance.DonutDeclarationDay(this.gameObject);
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
        if (other == other.GetComponentInChildren<CapsuleCollider>())
        {
            GameObject compare = GameManager.instance.ReturnEntity(other.gameObject);
            if (compare != null)
            {
                GameManager.instance.UpdateDonutCount(other.gameObject, donutQty);
                //Keeps from null reference when donut is picked up
                for (int i = 0; i < GameManager.instance.PriorityPoint.Count; ++i)
                {
                    if (gameObject.transform == GameManager.instance.PriorityPoint[i])
                        GameManager.instance.PriorityPoint.Remove(GameManager.instance.PriorityPoint[i]);  
                }

                if (compare.name == "Player")
                {
                    GameManager.instance.playerScript.Munch(pickupSFX, audPickupVol);
                    GameManager.instance.playerScript.HealthPickup(); // Heals player by HpRestoreAmount 
                    GameManager.instance.worldDonutCount--;
                }
                GameManager.instance.TallyActiveScores();
                Destroy(gameObject);
                if(GameManager.instance.worldDonutCount > 0)
                    --GameManager.instance.worldDonutCount;
            }
        }
    }

    
}
