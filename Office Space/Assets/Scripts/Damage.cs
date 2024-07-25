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

    bool hasDamaged;

    // Start is called before the first frame update
    void Start()
    {
        if (type == damageType.stream || type == damageType.projectile)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (type != damageType.item)
        {
            if (other.isTrigger)
                return;

            IDamage dmg = other.GetComponent<IDamage>();

            if (dmg != null && !hasDamaged)
                dmg.takeDamage(damageAmount);

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
