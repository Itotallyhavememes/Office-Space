using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;

public class enemyAI : MonoBehaviour, IDamage, ITarget
{
    public int enemyDonutCount;
    
    //Enum for distinguishing different enemy types for AI distinction
    [SerializeField] enum enemyType { norm, fast, tank, security };
    [SerializeField] enemyType type;

    [SerializeField] public NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;
    [SerializeField] RigBuilder enemRig;
    [SerializeField] Color dmgColor;
    [SerializeField] Transform shootPos;
    [Range(1, 20)][SerializeField] int HP;
    [Range(50, 100)][SerializeField] int faceTargetSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject targeting;
    [SerializeField] float shootRate;
    [Range(1, 10)][SerializeField] int animTransitSpeed;
    public bool ikActive;

    [Header("----- Sounds -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audDamage;
    [Range(0, 1)][SerializeField] float audDamageVol;

    [Header("----- Variables -----")]

    Color colorOrig;
    bool isShooting;
    bool targetInRange;
    Vector3 TargetDIR;
    float angleToTarget;
    //JOHN CODE
    bool isSprinting;
    [Range(5, 25)][SerializeField] int dodgeSpeed;
    Vector3 enemyVel;
    Vector3 randPos;
    [Range(15, 100)][SerializeField] float range;
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
    //public int enemyDonutCount;
    [SerializeField] int dodgeNumber;
    [SerializeField] int detCode;
    [SerializeField] GameObject targetOBJ;
    [SerializeField] int tarOBJhash;
    //[SerializeField] List<GameObject> targets;
    [Range(5, 12)][SerializeField] int jumpSpeed;
    bool isJumping;
    ITarget target;
    [Range(10, 20)][SerializeField] int roamDist;
    [Range(1, 3)][SerializeField] int roamTimer;
    Vector3 startingPos;
    bool isRoaming;
    float stoppingDistOrig;
    bool canTarget;
    bool isTargetDead;
    [SerializeField] int numOfTargets;
    //bool hasPriority;
 


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
        if(type != enemyType.security)
            GameManager.instance.AddToTracker(this.gameObject);
        isTargetDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * animTransitSpeed));

        //if (targets.Count == 1)
        //{
        //    if (targets[0] == null && !targetOBJ)
        //    {
        //        targets.Remove(targets[0].gameObject);
        //        if (targetInRange == true)
        //            targetInRange = false;
        //    }
        //    else
        //    {
        //        CleanUpList();
        //        targetOBJ = PrioritizeTarget();
        //    }
        //}
        if (numOfTargets > 0)
            targetInRange = true;
        else targetInRange = false;

        if (targetInRange)
        {
            //if (targets.Count > 1 && targetOBJ == null)
            //{
            //    CleanUpList();
            //    targetOBJ = PrioritizeTarget();
            //}
            for (int i = 0; i < GameManager.instance.deadTracker.Count; ++i)
            {
                if (tarOBJhash == GameManager.instance.deadTracker[i].GetHashCode())
                {
                    targetOBJ = null;
                    isTargetDead = true;
                    break;
                }
            }
            if (isTargetDead)
            {
                targetOBJ = PrioritizeTarget(targetOBJ);
                if (targetOBJ != null)
                    isTargetDead = false;
            }
            if (targetOBJ != null)
                Debug.Log(gameObject.name.ToString() + " says: My New Target-> " + targetOBJ.name.ToString());
            if (canSeePlayer())
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                    FaceTarget();
                if (!isShooting)
                    StartCoroutine(shoot());
                agent.stoppingDistance = stoppingDistOrig;
            }
            else
            {
                if (agent.remainingDistance < 0.05f)
                {
                    if (type != enemyType.security)
                    {
                        if (!isRoaming)
                        {
                            if (GameManager.instance.PriorityPoint == null)
                            {
                                StartCoroutine(Roam());
                                //StopCoroutine(GoToPOI());
                            }
                            else
                            {
                                StartCoroutine(GoToPOI());
                                //StopCoroutine(Roam());
                            }
                        }
                        //if (!hasPriority && GameManager.instance.PriorityPoint != null)
                        //    StartCoroutine(GoToPOI());
                        //if (!isRoaming && GameManager.instance.PriorityPoint == null)
                        //    StartCoroutine(Roam());

                        //if (!isRoaming)
                        //    StartCoroutine(Roam());
                    }
                    else if (type == enemyType.security && !isPatrol)
                        StartCoroutine(Patrol());
                }
            }
        }
        else if (!targetInRange)
        {
            if (type != enemyType.security)
            {
                if (!isRoaming)
                {
                    if (GameManager.instance.PriorityPoint == null)
                    {
                        StartCoroutine(Roam());
                        //StopCoroutine(GoToPOI());
                    }
                    else
                    {
                        StartCoroutine(GoToPOI());
                        //StopCoroutine(Roam());
                    }
                }
                //if (!hasPriority && GameManager.instance.PriorityPoint != null)
                //    StartCoroutine(GoToPOI());
                //if (!isRoaming && GameManager.instance.PriorityPoint == null)
                //    StartCoroutine(Roam());

                //if(!isRoaming)
                //    StartCoroutine (Roam());
            }
            else
            {
                if (!isPatrol && agent.remainingDistance < 0.05f)
                    StartCoroutine(Patrol());
            }
        }
    }

    void FaceTarget()
    {
        TargetDIR.y = 0;
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
        if (type != enemyType.security)
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
    }

    IEnumerator GoToPOI()
    {
        Debug.Log(gameObject.name.ToString() + "says: Heading For - " + GameManager.instance.PriorityPoint.name.ToString());
        //hasPriority = true;
        yield return new WaitForSeconds(0.1f);
        agent.stoppingDistance = 0;
        agent.SetDestination(GameManager.instance.PriorityPoint.position);
        //hasPriority = false;
    }

    //if potential target enters Sphere
    public void OnTriggerEnter(Collider other)
    {
        //if (other == other.GetComponentInChildren<CapsuleCollider>())
        if (numOfTargets < GameManager.instance.bodyTracker.Count)
        {
            target = other.GetComponent<ITarget>();
            if (target != null && other != this)
            {

                if (numOfTargets == 0)
                {
                    targetOBJ = other.gameObject;
                    ++numOfTargets;
                }
                else
                {
                    ++numOfTargets;
                    targetOBJ = PrioritizeTarget(targetOBJ);
                }
                //targetInRange = true;
                //TargetDIR = targetOBJ.transform.position - transform.position;
                //    targetOBJ = other.gameObject;
                //    targetInRange = true;
                //    TargetDIR = targetOBJ.transform.position - transform.position;
                //}
                ////System for holding multiple targets
                //if (type != enemyType.security)
                //{
                //bool canAdd = false;
                //if (targetInRange && targetOBJ)
                //{
                //    if (targets.Count == 0)
                //        canAdd = true;
                //    else if (targets.Count > 0)
                //    {
                //        CleanUpList();
                //        for (int i = 0; i < targets.Count; ++i)
                //        {
                //            if (targets[i].GetHashCode() == targetOBJ.GetHashCode())
                //            {
                //                canAdd = false;
                //                break;
                //            }
                //            else
                //            {

                //                canAdd = true;
                //            }
                //        }
                //    }
                //}
                ////Time to add
                //if (canAdd)
                //{
                //    targets.Add(targetOBJ);
                //    targetOBJ = PrioritizeTarget();
                //}
            }
        }
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    //targetOBJ = PrioritizeTarget(targetOBJ);
    //}
    //If target exits Sphere
    public void OnTriggerExit(Collider other)
    {
        //if (other == other.GetComponentInChildren<CapsuleCollider>() )
        if (numOfTargets > 0)
        {
            target = other.GetComponent<ITarget>();
            if (target != null && other != this)
            {

                --numOfTargets;
                targetOBJ = PrioritizeTarget(targetOBJ);
                //    RemoveFromList(other.gameObject);
                //    if (targets.Count == 0)
                //    {
                //        targetOBJ = null;
                //        target = null;
                //        targetInRange = false;
                //        agent.stoppingDistance = 0;
                //        TargetDIR = Vector3.zero;
                //    }
                //    else if (targets.Count > 0)
                //    {
                //        targetOBJ = PrioritizeTarget();
                //    }
            }
        }
    }

    //Goes through list and checks to see if anything is null
    //If so, delete it from the list
    //public void CleanUpList()
    //{
    //    for (int i = 0; i < targets.Count; ++i)
    //    {
    //        if (targets[i].gameObject == null)
    //            targets.Remove(targets[i].gameObject);
    //    }
    //}

    //takes in currTarget and checks to see if currTarget is closest in relation to GameManager's bodyTracker
    //if it is, return currTarget, if not: find the closest and return that as priority
    public GameObject PrioritizeTarget(GameObject currTarget)
    {
        
        GameObject targetHolder = null;
        float currDist = 0.0f;
        float compDist = 0.0f;
        if (currTarget != null)
        {
            targetHolder = currTarget;
        }
        else
        {
            for (int i = 0; i < GameManager.instance.bodyTracker.Count; i++)
            {
                if(GameManager.instance.bodyTracker[i].GetHashCode() != gameObject.GetHashCode())
                    targetHolder = GameManager.instance.bodyTracker[i];
            }
        }
        
        
        if (targetHolder)
        {
            currDist = Vector3.Distance(transform.position, targetHolder.transform.position);
            for (int i = 0; i < GameManager.instance.bodyTracker.Count; i++)
            {
                if (GameManager.instance.bodyTracker[i].GetHashCode() != gameObject.GetHashCode())
                {
                    compDist = Vector3.Distance(transform.position, GameManager.instance.bodyTracker[i].transform.position);
                    if (compDist < currDist)
                    {
                        targetHolder = GameManager.instance.bodyTracker[i];
                        target = targetHolder.GetComponent<ITarget>();
                        break;
                    }
                }
            }
            detCode = GetDetCode(targetHolder);
            tarOBJhash = targetHolder.GetHashCode();
        }

        return targetHolder;
    }

    int GetDetCode(GameObject target)
    {
        if (target != null)
        {
            if (target.CompareTag("Player"))
                return 1;
            else
                return 2;
        }
        return 0;
    }

    //public void RemoveFromList(GameObject targetObj)
    //{
    //    for (int i = 0; i < targets.Count; i++)
    //    {
    //        if (targetObj.GetHashCode() == targets[i].GetHashCode())
    //        {
    //            targets.Remove(targets[i]);
    //            //target = null;
    //            break;
    //        }
    //    }
    //}

    bool canSeePlayer()
    {
        if (targetOBJ)
        {

            //OLD CODE:
            targeting.transform.position = targetOBJ.transform.position;
            TargetDIR = targeting.transform.position - transform.position;
            angleToTarget = Vector3.Angle(TargetDIR, transform.forward);

            bool canSee = false;

            switch (detCode)
            {
                //For Players
                case 1:
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, TargetDIR, out hit))
                    {
                        if (hit.collider.CompareTag("Player"))
                            canSee = true;
                    }
                    break;
                //For Other Enemies
                case 2:
                    if (!Physics.Linecast(transform.position, targeting.transform.position))
                        canSee = true;
                    break;
                //Most cases when detCode is absent
                default:
                    canSee = false;
                    break;
            }

            if (angleToTarget <= viewAngle && canSee)
            {
                agent.stoppingDistance = stoppingDistOrig;
                agent.SetDestination(targetOBJ.transform.position);
                canTarget = true;
                anim.SetBool("Aiming", true);
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

    public void takeDamage(int amount)
    {
        if (type != enemyType.security)
        {
            HP -= amount;
            Debug.Log(gameObject.name.ToString() + " HP: " + HP.ToString());

            StartCoroutine(flashDamage());
            //if (targetOBJ)
            //{
            //    targetOBJ = PrioritizeTarget(targetOBJ);
            //    FaceTarget();
            //}

            //dodgeNumber determines how often this enemy dodges
            if (HP % dodgeNumber == 0)
                dodgeThreat();

            aud.PlayOneShot(audDamage[Random.Range(0, audDamage.Length)], audDamageVol);

            if (HP <= 0)
            {
                //He's died, so decrement
                --GameManager.instance.enemyCount;
                //GameManager.instance.DeclareSelfDead(this.gameObject, this.type.ToString());
                //Destroy(gameObject);

                gameObject.SetActive(false) ;
                if (gameObject.activeSelf == false)
                    Debug.Log(gameObject.name.ToString() + " : DEAD");
                for (int i = 0; i < GameManager.instance.bodyTracker.Count; ++i)
                {
                    if (gameObject.GetHashCode() == GameManager.instance.bodyTracker[i].GetHashCode())
                        GameManager.instance.bodyTracker.Remove(GameManager.instance.bodyTracker[i]);
                }
                GameManager.instance.deadTracker.Add(gameObject);
            }
        }
    }

    void dodgeThreat()
    {
        FaceTarget();
        enemyVel = new Vector3(Random.Range(-dodgeSpeed, dodgeSpeed), 0, Random.Range(-dodgeSpeed, dodgeSpeed));
        agent.velocity = enemyVel;
        //Jump
    }

    IEnumerator Jump()
    {
        if (isJumping)
        {
            yield return null;
            if (agent.baseOffset == -0.07)
                isJumping = false;
        }
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

    public bool declareDeath()
    {
        return true;
    }
}
