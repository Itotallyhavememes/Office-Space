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
                //GameManager.instance.UpdateDonutCount(other.gameObject, donutQty);
                //Keeps from null reference when donut is picked up
                for (int i = 0; i < GameManager.instance.PriorityPoint.Count; ++i)
                {
                    if (gameObject.transform == GameManager.instance.PriorityPoint[i])
                        GameManager.instance.PriorityPoint.Remove(GameManager.instance.PriorityPoint[i]);
                }

                //if (gameObject.transform == GameManager.instance.PriorityPoint)
                //    GameManager.instance.PriorityPoint = other.transform;

                if (compare.name == "NMODEL_Player")
                {
                    GameManager.instance.playerScript.Munch(pickupSFX, audPickupVol);
                    //GameManager.instance.playerScript.HealthPickup(); // Heals player by HpRestoreAmount 
                    //FOR DK SPECIFICALLY
                    GameManager.instance.playerScript.DKPickedUp();
                    //GameManager.instance.worldDonutCount--;
                }
                //else
                //{
                //    //compare.GetComponent<enemyAI>().HealHP(HpRestoreAmount);
                //    compare.GetComponent<enemyAI>().ResetHP();
                //}
                //GameManager.instance.TallyActiveScores();

                //CODE BIT: Donut King Status Update
                GameManager.instance.TheDonutKing = other.gameObject;
                //LOGIC TO CHECK WHICH LIGHT TO TOGGLE
                PlayerControl lightSwitchP = other.GetComponent<PlayerControl>();
                if (lightSwitchP != null)
                    lightSwitchP.ToggleMyLight();
                else
                {
                    enemyAI lightSwitchE = other.GetComponent<enemyAI>();
                    lightSwitchE.ToggleMyLight();
                    lightSwitchE.ToggleAmIKing();
                    if (lightSwitchE.getKingStatus())
                        Debug.Log("ALL HAIL: " + other.name);
                }
                GameManager.instance.statsTracker[other.name].updateDKStatus();
                if (GameManager.instance.statsTracker[other.name].getDKStatus() == true)
                {
                    Debug.Log(other.name + " IS THE DONUT KING!");
                    GameManager.instance.DeclareTheDonutKing();
                    GameManager.instance.PriorityPoint.Clear();
                    GameManager.instance.PriorityPoint.Add(other.gameObject.transform);
                    //TEST: Does Coroutine Start Timer here?
                    //StartCoroutine(GameManager.instance.DKTimer());
                }

                Debug.Log(other.name.ToString() + " : " + GameManager.instance.statsTracker[other.name].getAllStats());
                gameObject.SetActive(false);
                if(GameManager.instance.worldDonutCount > 0)
                    --GameManager.instance.worldDonutCount;
            }
        }
    }

    
}
