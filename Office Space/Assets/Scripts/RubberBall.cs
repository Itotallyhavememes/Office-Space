using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubberBall : MonoBehaviour
{
    [SerializeField] float delay;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] float blastRadius;
    [SerializeField] float blastForce;
    [SerializeField] int damageAmount;

    [Header("----- Sounds -----")]
    [SerializeField] AudioClip rubberBall;
    [Range(0, 1)][SerializeField] float volume;

    float countdown;
    bool hasExploded;

    bool hasDamaged;

    // Start is called before the first frame update
    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            // rubberOut.PlayOneShot(rubberBall, why);
            hasExploded = false;

        }
    }

    void Explode()
    {
        if (!GameManager.instance.isMultiplayer)
            GameManager.instance.playerScript.Munch(rubberBall, volume);
        else
        {
            PlayerManager.instance.players[0].GetComponent<ControllerTest>().Munch(rubberBall, volume);
        }
        // show effect
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Collider array stores the info of every collider in the blast radius
        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider nearbyObject in collidersToDestroy)
        {

            IDamage dmg = nearbyObject.GetComponent<IDamage>();
            if (dmg != null && !hasDamaged)
            {
                Vector3 direction = nearbyObject.transform.position - transform.position;
                float distance = direction.magnitude;
                ////DebugLog("Distance from the explosion= " + distance);
                int inflictedDamage = (int)(damageAmount * (1 - distance / blastRadius));
                ////DebugLog("Damage value= " + inflictedDamage);
                if (inflictedDamage <= 0)
                    inflictedDamage = 1;
                dmg.takeDamage(inflictedDamage);
            }
        }

        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider nearbyObject in collidersToMove)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

            if (rb != null && !nearbyObject.CompareTag("Player"))
            {
                Vector3 direction = nearbyObject.transform.position - transform.position;
                float distance = direction.magnitude;
                float blastMagnitude = blastForce * (1 - distance / blastRadius);
                rb.AddExplosionForce(blastMagnitude, transform.position, blastRadius);
            }
        }

        Destroy(gameObject);
        hasExploded = true;
    }
}
