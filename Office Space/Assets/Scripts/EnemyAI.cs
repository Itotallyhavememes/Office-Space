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

    [SerializeField] int viewAngle;
    Color origColor;

    bool isShooting;
    bool playerInRange;

    Vector3 playerDir;
    Vector3 enemyVel;
    // random walk for enemy
    [SerializeField] float range;
    [SerializeField] LayerMask groundLayer;
    Vector3 enemyWalk;
    Vector3 destPoint;
    bool walkPoint;
    GameObject players;
    //
    [SerializeField] int dodgeSpeed;
    [SerializeField] int chaseCooldown;
    bool isSprinting;
    Vector3 randPos;
    bool lostTraget;
    float angleToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        //  isSprinting = false;
        agent.GetComponent<NavMeshAgent>();
        players = GameObject.Find("Player");
        origColor = model.material.color;
        GameManager.instance.UpdateGameGoal(1);

    }

    // Update is called once per frame
    void Update()
    {
        // SetRandowWaypoint();
        if(! playerInRange)
        {
            Patrol();
        }
       
            if (playerInRange && canSeePlayer())
            {
            
            //ToggleSprint();
            if (agent.remainingDistance <= agent.stoppingDistance)
              {
               
                FaceTarget();
              }
                
              if (!isShooting)
               {
                StartCoroutine(shoot());
                 }
             }
             if (playerInRange && !canSeePlayer())
            {
            
                 if (agent.remainingDistance > agent.stoppingDistance)
                 {
              //  ToggleSprint();
                FaceTarget();
                playerDir = GameManager.instance.player.transform.position - transform.position;
                agent.SetDestination(GameManager.instance.player.transform.position);
            }
               
          
        }
       
           

        
    }
    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(GameManager.instance.player.transform.position);
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        bool trueDectect = false;
        if (other.CompareTag("Player"))
        {
            trueDectect = true;

        }
        if (trueDectect)
        {
            if (chaseCooldown > 0)
                chaseCooldown = 0;
            playerInRange = true;
            lostTraget = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            lostTraget = true;
            chaseCooldown = 100;
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
        
        playerDir = GameManager.instance.player.transform.position - transform.position;
       
        agent.SetDestination(GameManager.instance.player.transform.position);
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
        StartCoroutine(flashDamage());

        PlayDamageSound();
        if(HP <=7)
        {
            dodgeThreat();
        }

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

    // method for enemy walk
    void Patrol()
    {
        if(!walkPoint)
        {
            SetRandowWaypoint();
        }
        if (walkPoint)
        {
            agent.SetDestination(destPoint);
        }
        if (Vector3.Distance(transform.position, destPoint) < 8)
        {
            walkPoint = false;
        }
    }
    private void SetRandowWaypoint()
    {
        //int randRage = Random.Range(-50, 50);
        float x = Random.Range(-range, range);
        float z = Random.Range(-range, range);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        if(Physics.Raycast(destPoint,Vector3.down, groundLayer))
        {
            walkPoint = true;
        }
    }
    //void MoveEnemy()
    //{
    //    agent.SetDestination(randPos);
    //}
   
    void dodgeThreat()
    {
       enemyVel = new Vector3(Random.Range(-dodgeSpeed, dodgeSpeed), Random.Range(-dodgeSpeed, dodgeSpeed), Random.Range(-dodgeSpeed, dodgeSpeed));
        agent.velocity = enemyVel;
    }
    //IEnumerator startCooldown()
    //{
    //    do
    //    {
    //        yield return new WaitForSeconds(Time.deltaTime * 3000);
    //        if (chaseCooldown > 0)
    //            --chaseCooldown;
    //    } while (chaseCooldown > 0);
    //}
    
}
