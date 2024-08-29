using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NerfBullet : MonoBehaviour
{

    [SerializeField] Rigidbody rb;

    [SerializeField] float forwardSpeed;
    [SerializeField] float upSpeed;
    [SerializeField] float destroyTime;
    [SerializeField] WeaponStats weaponStats;
    [SerializeField] bool hasPhysics;
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = (transform.forward * forwardSpeed) + (transform.up * upSpeed);
        Destroy(gameObject, destroyTime);
    }
    //was told to not have realistic physics for nerf dart but made it as an option
    // Update is called once per frame
    void Update()
    {
        if (hasPhysics)
        {
            rb.useGravity = true;
            transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);//gives the nerf bullet realistic physics 
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(weaponStats.shootDamage);
        }
        Destroy(gameObject);
    }
}
