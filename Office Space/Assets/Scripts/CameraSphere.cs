using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSphere : MonoBehaviour
{
    public List<GameObject> enemiesInRange = new();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }
}
