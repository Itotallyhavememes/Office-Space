using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        GameObject obstruction = GameManager.instance.ReturnEntity(other.gameObject);
        GameManager.instance.retryAmount = 1;
        if(obstruction != null)
            gameObject.SetActive(false);
        
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject obstruction = null ;
        if (gameObject.activeSelf == false)
        {
            obstruction = GameManager.instance.ReturnEntity(other.gameObject);
            if (obstruction != null)
                gameObject.SetActive(false);
        }
    }
}
