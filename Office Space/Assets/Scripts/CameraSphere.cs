using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSphere : MonoBehaviour
{
    public List<GameObject> enemiesInRange = new();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && other.GetComponent<enemyAI>() != null)
        {
            for(int i = 0; i < enemiesInRange.Count; i++)
            {
                if(other.gameObject.name == enemiesInRange[i].gameObject.name)
                {
                    return;
                }
            }
            enemiesInRange.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") && other.GetComponent<enemyAI>() != null)
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }
}
