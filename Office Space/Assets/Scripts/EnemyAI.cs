using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Color colorDamage;
    [SerializeField] Transform shootPos;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] AudioSource DamageSound1;
    [SerializeField] AudioSource DamageSound2;
    [SerializeField] AudioSource DamageSound3;
    [SerializeField] AudioSource DamageSound4;
    [SerializeField] AudioSource DamageSound5;

    Color origColor;

    bool isShooting;
    bool playerInRange;

    Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
        origColor = model.material.color;
        GameManager.instance.UpdateGameGoal(1);

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            playerDir = GameManager.instance.player.transform.position - transform.position;

            agent.SetDestination(GameManager.instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
            }

            if (!isShooting)
            {
                StartCoroutine(shoot());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(flashDamage());

        PlayDamageSound();

        if (HP <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.UpdateGameGoal(-1);
        Destroy(gameObject);
    }

    IEnumerator flashDamage()
    {
        model.material.color = colorDamage;
        yield return new WaitForSeconds(0.1f);
        model.material.color = origColor;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void PlayDamageSound()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                DamageSound1.Play();
                break;
            case 1:
                DamageSound2.Play();
                break;
            case 2:
                DamageSound3.Play();
                break;
            case 3:
                DamageSound4.Play();
                break;
            case 4:
                DamageSound5.Play();
                break;
        }
    }
}
