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
        if (other == other.GetComponentInChildren<CapsuleCollider>())
        {
            GameObject compare = GameManager.instance.ReturnEntity(other.gameObject);
            if (compare != null /*&& compare.transform.position == transform.position*/)
            {
                GameManager.instance.UpdateDonutCount(other.gameObject);
                Destroy(gameObject);
            }
        }
    }

    
}
