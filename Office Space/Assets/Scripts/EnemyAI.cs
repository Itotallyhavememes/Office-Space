//using Autodesk.Fbx;
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
    //[SerializeField] int donutDropDistance;
    //[SerializeField] GameObject donutDropItem;
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
    public Transform destPriority;
    bool hasPriority;
    //bool hasPriority;

    int origHP;
 


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
        agent.stoppingDistance = 0;
        destPriority = null;
        origHP = HP;
    }

    // Update is called once per frame
    //FOR ALL
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * animTransitSpeed));

        if (type != enemyType.security)
        {
            if (numOfTargets > 0)
                targetInRange = true;
            else targetInRange = false;
        }
            if (GameManager.instance.PriorityPoint.Count > 0 || !destPriority)
                destPriority = GetPriorityPoint();
            if (GameManager.instance.PriorityPoint.Count == 0 && destPriority)
                destPriority = null;

        
       

        if (targetInRange)
        {

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
                //Debug.Log(gameObject.name.ToString() + " says: My New Target-> " + targetOBJ.name.ToString());
            if (canSeePlayer())
            {
                //Debug.Log("I see you!");
                if (agent.remainingDistance <= agent.stoppingDistance)
                    FaceTarget();
                if (!isShooting)
                    StartCoroutine(shoot());
            }
            else
            {
                if (agent.remainingDistance < 0.05f)
                {
                    if (type != enemyType.security)
                    {
                        if (!isRoaming)
                        {
                            if (GameManager.instance.PriorityPoint.Count > 0)
                            {
                                if(destPriority != null)
                                    StartCoroutine(GoToPOI());
                            }
                            else
                            {
                                StartCoroutine(Roam());
                            }
                        }
                    }
                    else if (type == enemyType.security && !isPatrol)
                        StartCoroutine(Patrol());
                }
           }
        }
        else if (!targetInRange/* && !destPriority*/)
        {
            if (type != enemyType.security)
            {
                if (!isRoaming)
                {
                    if (GameManager.instance.PriorityPoint.Count > 0)
                    {
                        if (destPriority != null)
                            StartCoroutine(GoToPOI());
                    }
                    else
                    {
                        StartCoroutine(Roam());
                    }
                }
            }
            else
            {
                if (!isPatrol && agent.remainingDistance < 0.05f)
                    StartCoroutine(Patrol());
            }
        }
    }

    //FOR BOTH
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
    //ONLY FOR SECURITY
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

    //ONLY FOR !SECURITY
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

    //FOR BOTH
    IEnumerator GoToPOI()
    {
        //Debug.Log(gameObject.name.ToString() + "says: Heading For - " + GameManager.instance.PriorityPoint.name.ToString());
        //hasPriority = true;
        yield return new WaitForSeconds(0.1f);
        agent.stoppingDistance = 0;
        agent.SetDestination(destPriority.position);
        //hasPriority = false;
    }

    //if potential target enters Sphere
    //FOR BOTH
    public void OnTriggerEnter(Collider other)
    {
        target = other.GetComponent<ITarget>();
        if (this.type == enemyType.security)
        {
            if (target != null && other.CompareTag("Player"))
            {
                targetOBJ = other.gameObject;
                targetInRange = true;
                detCode = 1;
            }
        }
        else
        {
            if (numOfTargets < GameManager.instance.bodyTracker.Count)
            {
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
                }
            }
        }
    }

    //If target exits Sphere
    //FOR BOTH
    public void OnTriggerExit(Collider other)
    {
        if (this.type == enemyType.security)
        {
            if (other.CompareTag("Player"))
            {
                targetOBJ = null;
                targetInRange = false;
                TargetDIR = Vector3.zero;
                detCode = 0;
            }
        }
        else
        {
            if (numOfTargets > 0)
            {
                target = other.GetComponent<ITarget>();
                if (target != null && other != this)
                {

                    --numOfTargets;
                    targetOBJ = PrioritizeTarget(targetOBJ);
                }
            }
        }
    }


    //takes in currTarget and checks to see if currTarget is closest in relation to GameManager's bodyTracker
    //if it is, return currTarget, if not: find the closest and return that as priority
    //ONLY FOR !SECURITY
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

    public Transform GetPriorityPoint()
    {
        Transform closestPoint = null;
        if (GameManager.instance.PriorityPoint.Count > 0)
        {
            Transform enemyT = gameObject.transform;
            closestPoint = GameManager.instance.PriorityPoint[0];
            float compDist = Vector3.Distance(enemyT.position, closestPoint.position);
            float newComp = 0.0f;
           for(int i = 0; i < GameManager.instance.PriorityPoint.Count; ++i)
            {
                newComp = Vector3.Distance(enemyT.position, GameManager.instance.PriorityPoint[i].position);
                if (compDist > newComp)
                {
                    compDist = newComp;
                    closestPoint = GameManager.instance.PriorityPoint[i];
                }
            }
        }
        return closestPoint;
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
                if(type!=enemyType.security)
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
            //Debug.Log(gameObject.name.ToString() + " HP: " + HP.ToString());

            if (HP > 0) //Stops them from respawning with their flash color on
                StartCoroutine(flashDamage());
            //dodgeNumber determines how often this enemy dodges
            if (HP % dodgeNumber == 0)
                dodgeThreat();

            aud.PlayOneShot(audDamage[Random.Range(0, audDamage.Length)], audDamageVol);

            if (HP <= 0)
            {
                //int indexx = 0;
                //He's died, so decrement
                --GameManager.instance.enemyCount;
                if (isShooting)
                {
                    isShooting = false;
                }
                for (int i = 0; i < GameManager.instance.bodyTracker.Count; ++i)
                {
                    if (gameObject.GetHashCode() == GameManager.instance.bodyTracker[i].GetHashCode())
                    {
                        GameManager.instance.bodyTracker.Remove(GameManager.instance.bodyTracker[i]);
                    }
                }
                GameManager.instance.DeclareSelfDead(gameObject);
                ResetHP();


                //
                //while (GameManager.instance.statsTracker[name] > 0)
                if (GameManager.instance.statsTracker[name].getDKStatus() == true)
                {
                    //GROUPED AS GAMEMANAGER METHOD
                    ////creates sphere that's the size of roamDist and selects a random position
                    //Vector3 randDropPos = Random.insideUnitSphere * donutDropDistance;
                    //randDropPos.y = donutDropItem.transform.position.y;
                    ////Prevents getting null reference when creating random point
                    //NavMeshHit hit;
                    ////The "1" is in refernce to layer mask "1"
                    //NavMesh.SamplePosition(randDropPos, out hit, donutDropDistance, 1);
                    //Instantiate(donutDropItem, transform.position + randDropPos, donutDropItem.transform.rotation);
                    ////GameManager.instance.UpdateDonutCount(gameObject, -1);

                    ////Sets isDonutKing for current object to false, since initially was true
                    //GameManager.instance.statsTracker[name].updateDKStatus();

                    GameManager.instance.dropTheDonut(this.gameObject);
                }
                //GameManager.instance.DeclareSelfDead(gameObject);//
                gameObject.SetActive(false);
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

        if (type != enemyType.security)

        if(type!= enemyType.security)

        {
            isShooting = true;

            Instantiate(bullet, shootPos.position, transform.rotation);
            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }

    public GameObject declareOBJ(GameObject obj)
    {
        return this.gameObject;
    }

    public bool declareDeath()
    {
        return true;
    }

    public void ResetHP()
    {
        HP = origHP;
    }

    public void HealHP(int amount)
    {
        HP += amount;
        if (HP > origHP)
            HP = origHP;
    }
}
