using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winScreenOnTrigger : MonoBehaviour
{
    [SerializeField] Collider playerCollider;
    [SerializeField] GameObject winScore;
    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if(other == playerCollider)
        {
            GameManager.instance.StatePause();
            GameManager.instance.GetNS_GoalScreen();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
