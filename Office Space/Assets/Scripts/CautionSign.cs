using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CautionSign : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Collider EggGuardian;
    [SerializeField] bool GateGuarded;
    void Start()
    {
        if(EggGuardian.enabled == true)
            EggGuardian.enabled = false;
        GateGuarded = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        ControllerTest compare = other.GetComponent<ControllerTest>();
        if (compare != null)
        {
            if (GameManager.instance.statsTracker[other.name].getDKStatus() == true && EggGuardian.enabled == false)
            {
                EggGuardian.enabled = true;
            }
            //else if(other.gameObject.name != GameManager.instance.TheDonutKing.name && EggGuardian.enabled == true)
            //    EggGuardian.enabled = false; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject compare = GameManager.instance.ReturnEntity(other.gameObject);
        if (compare != null && EggGuardian.enabled == true)
        {
            EggGuardian.enabled = false;
        }
    }
}
