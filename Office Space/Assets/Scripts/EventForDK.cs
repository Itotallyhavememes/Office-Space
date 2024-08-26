using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventForDK : MonoBehaviour
{
    [SerializeField] GameObject DK;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (DK == false)
        {
            //DebugLog("NO");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        
    }



}
