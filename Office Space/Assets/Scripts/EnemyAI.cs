using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage, ITarget
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Color dmgColor;
    [SerializeField] Transform shootPos;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Color colorOrig;
    bool isShooting;
    bool targetInRange;
    Vector3 TargetDIR;
    float angleToPlayer;
    //JOHN CODE
    bool isSprinting;
    [SerializeField] int dodgeSpeed;
    Vector3 enemyVel;
    Vector3 randPos;
    //TIM CODE
    [SerializeField] GameObject targetOBJ;
    ITarget target;
    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        //tells game manager that we've made an enemy
        //GameManager.instance.updateGameGoal(1);
        isSprinting = false;
        target = null;
        randPos = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //I found you and I'm coming for you
        if (targetInRange && canSeePlayer())
        {
            //ToggleSprint();
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }

            if (!isShooting)
            {
                StartCoroutine(shoot());
            }

        }
        //You can't see me, but I'm nearby
        else if (targetInRange && !canSeePlayer())
        {
            //I just saw you, but I lost sight of you, but I'm going to try and find you
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                //ToggleSprint();
                faceTarget();
                TargetDIR = targetOBJ.transform.position - transform.position;
                agent.SetDestination(targetOBJ.transform.position);
            }
        }
        else
        {
            randPos = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
            agent.SetDestination(randPos);

        }
    }

    void ToggleSprint()
    {
        if (!isSprinting)
        {
            agent.speed *= 10000;
            agent.angularSpeed *= 10000;
            faceTargetSpeed *= 10000;
        }
        else
        {
            agent.speed /= 10000;
            agent.angularSpeed /= 10000;
            faceTargetSpeed /= 10000;
        }
        isSprinting = !isSprinting;
    }

    bool canSeePlayer()
    {
        TargetDIR = targetOBJ.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(TargetDIR, transform.forward);
        //Each frame, the enemy AI will be seeking out player's position through this line

        Debug.Log(angleToPlayer);
        Debug.DrawRay(transform.position, TargetDIR);
        RaycastHit hit;
        //This checks if there is a wall between enemy and player
        if (Physics.Raycast(transform.position, TargetDIR, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(targetOBJ.transform.position);
                return true;
            }

        }
        return false;
    }

    //if Player enters SPHERE/GENERAL RANGE
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("OBJ IN RANGE");
        target = other.GetComponent<ITarget>();
        if (target != null)
        {
            targetOBJ = other.gameObject;
            targetInRange = true;
        }
    }

    //if Player exits bubble
    public void OnTriggerExit(Collider other)
    {
        if (targetOBJ == other.gameObject)
        {
            targetOBJ = null;
            targetInRange = false;
        }
    }

    void faceTarget()
    {

        Quaternion rot = Quaternion.LookRotation(TargetDIR);
        //transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
        transform.rotation = rot;
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashDamage());
        //Enemy reacts to getting hurt
        //Enemy now SNAPS to the direction of firing player
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (HP <= 7)
            dodgeThreat();
        faceTarget();
        if (HP <= 0)
        {
            //He's died, so decrement
            // GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    void dodgeThreat()
    {
        enemyVel = new Vector3(Random.Range(-dodgeSpeed, dodgeSpeed), 0, Random.Range(-dodgeSpeed, dodgeSpeed));
        agent.velocity = enemyVel;
    }

    IEnumerator flashDamage()
    {
        model.material.color = dmgColor;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public GameObject declareOBJ(GameObject obj)
    {
        return this.gameObject;
    }
}
