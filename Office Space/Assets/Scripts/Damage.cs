using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] enum damageType { stream, stationary, projectile, item }
    [SerializeField] damageType type;

    [SerializeField] Rigidbody rb;
    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    public GameObject parent;
    public GameObject victim;
    public bool hasKilled;
    bool hasDamaged;

    //TEST VARIABLE FOR KILL COUNT
    [SerializeField] GameObject dmgSource;

    // Start is called before the first frame update
    void Start()
    {
        
        if (type == damageType.stream || type == damageType.projectile)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    //METHOD HERE:
    public GameObject tellMyParents(GameObject deadman)
    {
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (type != damageType.item)
        {
            if (other.isTrigger)
                return;

            IDamage dmg = other.GetComponent<IDamage>();
            //GameObject victim = other.GetComponent<GameObject>();
            
            if (dmg != null && !hasDamaged)
            {
                Debug.Log(parent.name.ToString() + " --> SHOT --> " + other.name.ToString());
                dmg.takeDamage(damageAmount);
                if (GameManager.instance.CallTheDead(other.name))
                {
                    //Debug.Log(other.name.ToString() + " DIED!");
                    hasKilled = true;
                    victim = other.gameObject;
                    GameManager.instance.statsTracker[parent.name].updateKills();
                    GameManager.instance.statsTracker[parent.name].updateKDR();
                    GameManager.instance.DisplayKillMessage(parent, other.gameObject);
                    Debug.Log(parent.name.ToString() + GameManager.instance.statsTracker[parent.name].getAllStats());
                }


            }

            if (type == damageType.stream || type == damageType.projectile)
            {
                hasDamaged = true;
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (!gameObject.CompareTag("Enemy"))
        Instantiate(GameManager.instance.playerScript.weaponList[GameManager.instance.playerScript.GetSelectedWeaponIndex()].hitEffect, transform.position, Quaternion.identity);

    }
}
