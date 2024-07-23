using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] Renderer model;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && GameManager.instance.playerSpawn.transform.position!= this.transform.position)
        {
            GameManager.instance.playerSpawn.transform.position = transform.position;
            StartCoroutine(flashModel());

        }
    }
    IEnumerator flashModel()
    {
        model.material.color = Color.red;
        GameManager.instance.checkPointPos.SetActive(true);
        yield return new WaitForSeconds(.3f);
        GameManager.instance.checkPointPos.SetActive(false);
        model.material.color = Color.white;
    }

}
