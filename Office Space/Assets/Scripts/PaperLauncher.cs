using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperLauncher : MonoBehaviour
{
    [SerializeField] GameObject PaperSpawnPoint;
    [SerializeField] GameObject Projectile;
    [SerializeField] float shurikenRate;
    bool isShooting;

    private void Update()
    {
        if(Input.GetButton("Fire1") && !isShooting)
        {
            StartCoroutine(shoot());         
        }        
    }

    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(Projectile, PaperSpawnPoint.transform.position, PaperSpawnPoint.transform.rotation);

        yield return new WaitForSeconds(shurikenRate); //waits for the shootrate time to pass before setting isShooting to false
        isShooting = false;
    }
}
