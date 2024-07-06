using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] enum damageType { stream, stationary }
    [SerializeField] damageType type;

    [SerializeField] Rigidbody rb;
    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    bool hasDamaged;

    // Start is called before the first frame update
    void Start()
    {
        if (type == damageType.stream)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && !hasDamaged)
            dmg.takeDamage(damageAmount);

        if (type == damageType.stream)
        {
            hasDamaged = true;
            Destroy(gameObject);
        }
    }
}
