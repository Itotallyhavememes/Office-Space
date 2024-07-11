using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class DonutPickUp : MonoBehaviour
{

    public static event Action OnCollected;

    public static int totalDonuts;
    public static int HPincrease;

    [SerializeField] float rotationSpeed;
    [SerializeField] int HpRestoreAmount;
    

    private void Awake()
    {
        totalDonuts++;
        HPincrease = HpRestoreAmount;
    }

    //[SerializeField] float maxHeight;
    //[SerializeField] float minHeight;
    //Vector3 targetPosition;
    //Vector3 originalPosition;
    //Vector3 maxPosition;
    //Vector3 minPosition;

    // Start is called before the first frame update
    void Start()
    {
        //originalPosition = transform.position;
        //minPosition = new Vector3(0, originalPosition.y - minHeight, 0);
        //maxPosition = new Vector3(0, originalPosition.y + maxHeight, 0);
        //targetPosition = maxPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Spin();
    }

    void Spin()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnCollected?.Invoke();
            Destroy(gameObject);
        }
        

    }

    //void Bounce() //More research required for prototype 2
    //{
    //    if (transform.position.y <= minPosition.y)
    //    {
    //        targetPosition = maxPosition;
    //        Debug.Log("Min Height Reached");

    //    }
    //    else if (transform.position.y >= maxPosition.y)
    //    {
    //        targetPosition = minPosition;
    //        Debug.Log("Max Height Reached");
    //    }
    //    transform.Translate((targetPosition - transform.position) * Time.deltaTime, Space.World);
    //}

}
