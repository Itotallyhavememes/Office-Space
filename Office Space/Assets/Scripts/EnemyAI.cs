using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class enemyAI : MonoBehaviour, IDamage, ITarget
{
    //Enum for distinguishing different enemy types for AI distinction
    [SerializeField] enum enemyType { norm, fast, tank, security};
    [SerializeField] enemyType type;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;
    [SerializeField] RigBuilder enemRig;
    [SerializeField] Color dmgColor;
    [SerializeField] Transform shootPos;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject targeting;
    [SerializeField] float shootRate;
    [SerializeField] int animTransitSpeed;
    public bool ikActive;

    [SerializeField] AudioSource DamageSound1;
    [SerializeField] AudioSource DamageSound2;
    [SerializeField] AudioSource DamageSound3;
    [SerializeField] AudioSource DamageSound4;
    [SerializeField] AudioSource DamageSound5;

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
    [SerializeField] float range;
    [SerializeField] float minRange;
    [SerializeField] LayerMask groundLayer;
    Vector3 enemyWalk;
    Vector3 destPoint;
    bool walkPoint;
    bool isPatrolling;
    [SerializeField] float patrolRate;

    public Transform currentPoint;
    [SerializeField] int romTimer;
    Vector3 PatrolPoint;
    Vector3 StartPoint;

    [SerializeField] List<Transform> Positions;
    int nextPosIndex;
    bool isPatrol;

    //TIM CODE
    [SerializeField] GameObject targetOBJ;
    ITarget target;
    [SerializeField] int roamDist;
    [SerializeField] int roamTimer;
    Vector3 startingPos;
    bool isRoaming;
    float stoppingDistOrig;
    Vector3 targetingOrig;
    bool canTarget;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        //tells game manager that we've made an enemy
        GameManager.instance.enemyCount++;
        isSprinting = false;
        target = null;
        randPos = Vector3.zero;
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
        StartPoint = transform.position;
        nextPosIndex = 0;
        //targetingOrig = gameObject.transform.position;
        //targeting.transform.position = targetingOrig;
    }

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;

        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * animTransitSpeed));
        
        //I found you and I'm coming for you
        if (targetInRange && canSeePlayer())
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
            }

            if (!isShooting)
            {
                StartCoroutine(shoot());
            }
            agent.stoppingDistance = stoppingDistOrig;
        }
        //You can't see me, but I'm nearby
        else if (targetInRange && !canSeePlayer())
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
                StartCoroutine(Roam());

        }
        else if (!targetInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
                StartCoroutine(Roam());
        }
        // JOHN PATROL CODE
        //if (playerInRage && !canSeePlayer())
        //{
        //    if (!isPatrol && agent.remainingDistance < .5f)
        //    {
        //        StartCoroutine(Patrol());
        //    }


        //}
        //else if (!playerInRage)
        //{
        //    if (!isPatrol && agent.remainingDistance < .5f)
        //    {
        //        StartCoroutine(Patrol());
        //    }

        //}
    }

    //private void OnAnimatorIK()
    //{
    //    if (canTarget && anim)
    //    {
    //        if (ikActive && targetOBJ)
    //        {
    //            //anim.SetLookAtWeight(1);
    //            //anim.SetLookAtPosition(new Vector3(targetOBJ.transform.position.x, targetOBJ.transform.position.y + 1));


    //            //Quaternion rot = Quaternion.LookRotation(TargetDIR);
    //            //transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    //        }
    //        else
    //        {
    //            anim.SetLookAtWeight(0);
    //        }
    //    }
    //}


    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(TargetDIR);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }


    IEnumerator Patrol()
    {
        isPatrol = true;


        yield return new WaitForSeconds(romTimer);

        agent.stoppingDistance = 0;
        GoOnPatrol();
        agent.SetDestination(PatrolPoint);
        isPatrol = false;
    }
    // PATROL POINT CAN NOT BE A CHALDEN OF ENEMY
    // PATROL POINT MUST BE DIG INTO POSITIONS IN EDIT 
    public void GoOnPatrol()
    {

        if (nextPosIndex < Positions.Count)
        {

            PatrolPoint = Positions[nextPosIndex].transform.position;
            ++nextPosIndex;
        }
        else if (nextPosIndex >= Positions.Count)
        {
            if (nextPosIndex > Positions.Count)
            {
                nextPosIndex = Positions.Count;
            }
            PatrolPoint = StartPoint;
            nextPosIndex = 0;
        }
    }
    IEnumerator Roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);
        agent.stoppingDistance = 0;
        //creates sphere that's the size of roamdist and selects a random position
        Vector3 randPos = Random.insideUnitSphere * roamDist;
        randPos += startingPos;
        //Prevents getting null reference when creating random point
        NavMeshHit hit;
        NavMesh.SamplePosition(randPos, out hit, roamDist, 1);
        //The "1" is in refernce to layer mask "1"
        agent.SetDestination(hit.position);
        isRoaming = false;
    }
    bool canSeePlayer()
    {
       
        TargetDIR = targetOBJ.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(TargetDIR, transform.forward);
        //Each frame, the enemy AI will be seeking out player's position through this line

        //Debug.Log(angleToPlayer);
        //Debug.DrawRay(transform.position, TargetDIR);
        RaycastHit hit;
        //This checks if there is a wall between enemy and player
        if (Physics.Raycast(transform.position, TargetDIR, out hit))
        {
            if ((hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy")) && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(targetOBJ.transform.position);
                canTarget = true;
                anim.SetBool("Aiming", true);
                if(targetOBJ != null)
                    targeting.transform.position = targetOBJ.transform.position;
                FaceTarget();
                enemRig.enabled = true;
                return true;
            }

        }
        agent.stoppingDistance = 0;
        canTarget = false;
        anim.SetBool("Aiming", false);
        enemRig.enabled = false;
        return false;
    }

    //if Player enters SPHERE/GENERAL RANGE
    public void OnTriggerEnter(Collider other)
    {
        target = other.GetComponent<ITarget>();
        if (target != null && other != this)
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
            agent.stoppingDistance = 0;
            //If targetOBJ leaves (for now) reset targeting transform position
            targeting.transform.position = targetingOrig;
        }
    }



    public void takeDamage(int amount)
    {

        HP -= amount;
        StartCoroutine(flashDamage());
        //Enemy reacts to getting hurt
        //Enemy now SNAPS to the direction of firing player
        Quaternion rot = Quaternion.LookRotation(TargetDIR);
        transform.rotation = rot;
        agent.SetDestination(targetOBJ.transform.position);
        //if (HP <= 7)
        //    dodgeThreat();

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

        if (HP <= 0)
        {
            //He's died, so decrement
            // GameManager.instance.updateGameGoal(-1);
            --GameManager.instance.enemyCount;
            Destroy(gameObject);
        }
    }

    void dodgeThreat()
    {
        //faceTarget();
        //enemyVel = new Vector3(Random.Range(-dodgeSpeed, dodgeSpeed), 0, Random.Range(-dodgeSpeed, dodgeSpeed));
        //agent.velocity = enemyVel;

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

    // method for enemy walk
    //IEnumerator Patrol()
    //{
    //    isPatrolling = true;
    //    if (!walkPoint)
    //    {
    //        SetRandowWaypoint();
    //    }
    //    if (walkPoint)
    //    {
    //        agent.SetDestination(destPoint);
    //    }
    //    if (Vector3.Distance(transform.position, destPoint) < minRange)
    //    {
    //        walkPoint = false;
    //    }
    //    yield return new WaitForSeconds(patrolRate);
    //    isPatrolling = false;
    //}

    //private void SetRandowWaypoint()
    //{
    //    //int randRage = Random.Range(-50, 50);
    //    float x = Random.Range(-range, range);
    //    float z = Random.Range(-range, range);

    //    destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
    //    if (Physics.Raycast(destPoint, Vector3.down, groundLayer))
    //    {
    //        walkPoint = true;
    //    }
    //}

    public GameObject declareOBJ(GameObject obj)
    {
        return this.gameObject;
    }
}
