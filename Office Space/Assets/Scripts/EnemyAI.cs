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
    [SerializeField] bool canSeeTarget;
    //public GameObject headPos;
    [SerializeField] GameObject DKLight;
    [SerializeField] bool amITheKing;
    [SerializeField] Vector3 DKRPoint;
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
    //bool isSprinting;
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
        //isSprinting = false;
        target = null;
        randPos = Vector3.zero;
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
        StartPoint = transform.position;
        nextPosIndex = 0;
        GameManager.instance.AddToTracker(this.gameObject);
        isTargetDead = false;
        agent.stoppingDistance = 0;
        destPriority = null;
        origHP = HP;
        DKLight.SetActive(false);
    }

    // Update is called once per frame
    //FOR ALL
    void Update()
    {

        if (!GameManager.instance.isThereDonutKing)
        {
            agent.SetDestination(GameManager.instance.donutDropItem.transform.position);
            agent.stoppingDistance = 0;
            //No Donut King, so fight each other
            targetOBJ = PrioritizeTarget(targetOBJ);

        }
        else if (GameManager.instance.isThereDonutKing)
        {
            if (GameManager.instance.isThereDonutKing.GetHashCode() != this.gameObject.GetHashCode())
            {
                agent.SetDestination(GameManager.instance.TheDonutKing.transform.position);
                agent.stoppingDistance = stoppingDistOrig;
                //if (agent.remainingDistance > agent.stoppingDistance)
                //    agent.stoppingDistance = 5;
                //There is Donut King, so targetOBJ = TheDonutKing;
                targetOBJ = GameManager.instance.TheDonutKing;
            }
            else
            {
                GoOnPatrol();
                targetOBJ = null;
                //targetOBJ = PrioritizeTarget(targetOBJ);
            }
        }
        if (targetOBJ != null)
            FaceTarget();
        if (canSeeTarget = canSeePlayer())
        {

            if (!isShooting)
                StartCoroutine(shoot());
        }

        float agentSpeed = agent.velocity.normalized.magnitude;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * animTransitSpeed));



        //if (type != enemyType.security)
        //{
        //    if (numOfTargets > 0)
        //        targetInRange = true;
        //    else targetInRange = false;
        //}
        ////LOGIC BREAKDOWN:
        ////Enemies should always Prioritize the Following:
        ////The Donut and The Donut King
        ////
        ////Logic changes based on situation:
        ////If The Donut is active, then The Donut King MUST be false
        ////-in this case, enemies attack each other as normal to get to The Donut
        ////Else if, The Donut is inactive, then The Donut King MUST be true
        ////-in this case, enemies ignore each other and only attack The Donut King
        ////Cycle repeats for enemies that are NOT The Donut King
        ////
        ////IF an Enemy BECOMES The Donut King
        ////-Main Priority: Choose a Spawn Location at random and run
        ////-If they BUMP into another enemy, then fight (Work on Complex DK Enemy AI Later)

        ////if (GameManager.instance.isThereDonutKing == false/* || !destPriority*/)
        ////{
        ////    destPriority = GameManager.instance.donutDropItem.transform;
        ////    PrioritizeTarget(targetOBJ);
        ////}
        ////else if (GameManager.instance.isThereDonutKing == true/* && destPriority*/)
        ////{
        ////    destPriority = GameManager.instance.TheDonutKing.transform;
        ////    targetOBJ = destPriority.gameObject;
        ////    GetDetCode(targetOBJ);
        ////}
        ////What to do if I'm the king
        //if (!amITheKing)
        //    destPriority = GetPriorityPoint();
        //else if (amITheKing)
        //    destPriority = null;


        //if (targetInRange)
        //{

        //    for (int i = 0; i < GameManager.instance.deadTracker.Count; ++i)
        //    {
        //        if (tarOBJhash == GameManager.instance.deadTracker[i].GetHashCode())
        //        {
        //            targetOBJ = null;
        //            isTargetDead = true;
        //            break;
        //        }
        //    }
        //    if (isTargetDead)
        //    {
        //        targetOBJ = PrioritizeTarget(targetOBJ);
        //        if (targetOBJ != null)
        //            isTargetDead = false;
        //    }
        //    if (targetOBJ != null)
        //        //Debug.Log(gameObject.name.ToString() + " says: My New Target-> " + targetOBJ.name.ToString());
        //        if (canSeePlayer())
        //        {
        //            //Debug.Log("I see you!");
        //            if (agent.remainingDistance <= agent.stoppingDistance)
        //                FaceTarget();
        //            if (!isShooting)
        //                StartCoroutine(shoot());
        //        }
        //        else
        //        {
        //            if (agent.remainingDistance < 0.05f)
        //            {

        //                if (type != enemyType.security)
        //                {
        //                    if (!amITheKing)
        //                    {
        //                        StartCoroutine(GoToPOI());
        //                        //if (GameManager.instance.PriorityPoint.Count > 0)
        //                        //{
        //                        //    if (destPriority != null)
        //                        //        StartCoroutine(GoToPOI());
        //                        //}
        //                        ////else
        //                        ////{
        //                        ////    StartCoroutine(Roam());
        //                        ////}
        //                    }
        //                    else if (amITheKing)
        //                        StartCoroutine(RunKingRun());
        //                }
        //                else if (type == enemyType.security && !isPatrol)
        //                    StartCoroutine(Patrol());
        //            }
        //        }
        //}
        //else if (!targetInRange/* && !destPriority*/)
        //{
        //    if (type != enemyType.security)
        //    {
        //        if (!amITheKing)
        //        {
        //            StartCoroutine(GoToPOI());
        //            //if (GameManager.instance.PriorityPoint.Count > 0)
        //            //{
        //            //    if (destPriority != null)
        //            //        StartCoroutine(GoToPOI());
        //            //}
        //            ////else
        //            ////{
        //            ////    StartCoroutine(Roam());
        //            ////}
        //        }
        //        else if (amITheKing)
        //            StartCoroutine(RunKingRun());
        //    }
        //    else
        //    {
        //        if (!isPatrol && agent.remainingDistance < 0.05f)
        //            StartCoroutine(Patrol());
        //    }
        //}
    }

    public void ToggleMyLight()
    {
        if (DKLight.activeSelf == false)
            DKLight.SetActive(true);
        else
            DKLight.SetActive(false);
    }

    public void ToggleAmIKing()
    {
        amITheKing = !amITheKing;
    }

    public bool getKingStatus() { return amITheKing; }

    //METHOD FOR DONUT KING ENEMIES
    IEnumerator RunKingRun()
    {
        isPatrol = true;
        yield return new WaitForSeconds(romTimer);
        agent.stoppingDistance = 0;
        RandomRunPoints();
        agent.SetDestination(DKRPoint);
        isPatrol = false;
    }

    public void RandomRunPoints()
    {
        //if (nextPosIndex < Positions.Count)
        //{

        //    PatrolPoint = Positions[nextPosIndex].transform.position;
        //    ++nextPosIndex;
        //}
        //else if (nextPosIndex >= Positions.Count)
        //{
        //    if (nextPosIndex > Positions.Count)
        //    {
        //        nextPosIndex = Positions.Count;
        //    }
        //    PatrolPoint = StartPoint;
        //    nextPosIndex = 0;
        //}
        DKRPoint = GameManager.instance.spawnPoints[Random.Range(0, GameManager.instance.spawnPoints.Count)].transform.position;
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
    //IEnumerator Roam()
    //{
    //    if (type != enemyType.security)
    //    {
    //        isRoaming = true;
    //        yield return new WaitForSeconds(roamTimer);
    //        agent.stoppingDistance = 0;
    //        //creates sphere that's the size of roamdist and selects a random position
    //        Vector3 randPos = Random.insideUnitSphere * roamDist;
    //        randPos += startingPos;
    //        //Prevents getting null reference when creating random point
    //        NavMeshHit hit;
    //        NavMesh.SamplePosition(randPos, out hit, roamDist, 1);
    //        //The "1" is in refernce to layer mask "1"
    //        agent.SetDestination(hit.position);
    //        isRoaming = false;
    //    }
    //}

    //FOR BOTH
    //IEnumerator GoToPOI()
    //{
    //    yield return new WaitForSeconds(0.05f);
    //    agent.stoppingDistance = 0;
    //    agent.SetDestination(destPriority.position);
    //}

    //if potential target enters Sphere
    //FOR BOTH
    //public void OnTriggerEnter(Collider other)
    //{
    //    target = other.GetComponent<ITarget>();
    //    if (this.type == enemyType.security)
    //    {
    //        if (target != null && other.CompareTag("Player"))
    //        {
    //            targetOBJ = other.gameObject;
    //            targetInRange = true;
    //            detCode = 1;
    //        }
    //    }
    //    else
    //    {
    //        if (numOfTargets < GameManager.instance.bodyTracker.Count)
    //        {
    //            if (target != null && other != this)
    //            {

    //                if (numOfTargets == 0)
    //                {
    //                    targetOBJ = other.gameObject;
    //                    ++numOfTargets;
    //                }
    //                else
    //                {
    //                    ++numOfTargets;
    //                    targetOBJ = PrioritizeTarget(targetOBJ);
    //                }
    //            }
    //        }
    //    }
    //}

    //If target exits Sphere
    //FOR BOTH
    //public void OnTriggerExit(Collider other)
    //{
    //    if (this.type == enemyType.security)
    //    {
    //        if (other.CompareTag("Player"))
    //        {
    //            targetOBJ = null;
    //            targetInRange = false;
    //            TargetDIR = Vector3.zero;
    //            detCode = 0;
    //        }
    //    }
    //    else
    //    {
    //        if (numOfTargets > 0)
    //        {
    //            target = other.GetComponent<ITarget>();
    //            if (target != null && other != this)
    //            {

    //                --numOfTargets;
    //                targetOBJ = PrioritizeTarget(targetOBJ);
    //            }
    //        }
    //    }
    //}


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
                if (GameManager.instance.bodyTracker[i].GetHashCode() != gameObject.GetHashCode())
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
            enemyAI targetComp = targetHolder.GetComponent<enemyAI>();
            //if(targetComp != null)
            //{
            //    targetHolder = targetComp.headPos;
            //}
            //detCode = GetDetCode(targetHolder);
            tarOBJhash = targetHolder.GetHashCode();
        }

        return targetHolder;
    }

    //public Transform GetPriorityPoint()
    //{
    //    //Transform closestPoint = null;
    //    //if (GameManager.instance.PriorityPoint.Count > 0)
    //    //{
    //    //    Transform enemyT = gameObject.transform;
    //    //    closestPoint = GameManager.instance.PriorityPoint[0];
    //    //    float compDist = Vector3.Distance(enemyT.position, closestPoint.position);
    //    //    float newComp = 0.0f;
    //    //   for(int i = 0; i < GameManager.instance.PriorityPoint.Count; ++i)
    //    //    {
    //    //        newComp = Vector3.Distance(enemyT.position, GameManager.instance.PriorityPoint[i].position);
    //    //        if (compDist > newComp)
    //    //        {
    //    //            compDist = newComp;
    //    //            closestPoint = GameManager.instance.PriorityPoint[i];
    //    //        }
    //    //    }
    //    //}
    //    //return closestPoint;
    //    Transform PriorityPoint = null;
    //    PriorityPoint = GameManager.instance.PriorityPoint[0];
    //    //if (!GameManager.instance.isThereDonutKing)
    //    //    PriorityPoint = GameManager.instance.donutDropItem.transform;
    //    //else if(GameManager.instance.isThereDonutKing)
    //    //    PriorityPoint = GameManager.instance.TheDonutKing.transform;

    //    return PriorityPoint;
    //}
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
        //Changed it to headPos, since transform is always at model's feet
        //enemyAI targetCmp = targetOBJ.GetComponent<enemyAI>();
        //if(targetCmp != null)
        //{
        //    targetOBJ = targetCmp.headPos;
        //}
        targeting.transform.position = targetOBJ.transform.position;
        TargetDIR = targeting.transform.position - transform.position;
        angleToTarget = Vector3.Angle(TargetDIR, transform.forward);
        //Each frame, the enemy AI will be seeking out player's position through this line

        //Debug.Log(angleToPlayer); <- this does take up quite a few frames, so take out if possible
        //Debug.DrawRay(headPos.position, PlayerDirection);
        Debug.DrawRay(transform.position, TargetDIR, Color.yellow);
        RaycastHit hit;
        //This checks if there is a wall between enemy and player
        if (Physics.Raycast(transform.position, TargetDIR, out hit))
        {
            if (hit.collider.name == targetOBJ.name && angleToTarget <= viewAngle)
            {
                return true;
            }
        }
        return false;
        //if (targetOBJ)
        //{

        //OLD CODE:
        //    targeting.transform.position = targetOBJ.transform.position;
        //    TargetDIR = targeting.transform.position - transform.position;
        //    angleToTarget = Vector3.Angle(TargetDIR, transform.forward);
        //    //if (GameManager.instance.isThereDonutKing)
        //    //    StopCoroutine(GoToPOI());
        //    bool canSee = false;

        //    switch (detCode)
        //    {
        //        //For Players
        //        case 1:
        //            RaycastHit hit;
        //            if (Physics.Raycast(transform.position, TargetDIR, out hit))
        //            {
        //                if (hit.collider.CompareTag("Player"))
        //                    canSee = true;
        //            }
        //            break;
        //        //For Other Enemies
        //        case 2:
        //            if (!Physics.Linecast(transform.position, targeting.transform.position))
        //                canSee = true;
        //            break;
        //        //Most cases when detCode is absent
        //        default:
        //            canSee = false;
        //            break;
        //    }

        //    if (angleToTarget <= viewAngle && canSee)
        //    {
        //        if(type!=enemyType.security)
        //            agent.stoppingDistance = stoppingDistOrig;
        //        agent.SetDestination(targetOBJ.transform.position);
        //        canTarget = true;
        //        anim.SetBool("Aiming", true);
        //        FaceTarget();
        //        enemRig.enabled = true;
        //        return true;
        //    }
        //}
        //agent.stoppingDistance = 0;
        //canTarget = false;
        //anim.SetBool("Aiming", false);
        //enemRig.enabled = false;
        //return false;
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
                //int indexx = 0;
                //He's died, so decrement
                --GameManager.instance.enemyCount;
                if (isShooting)
                {
                    isShooting = false;
                }
                //for (int i = 0; i < GameManager.instance.bodyTracker.Count; ++i)
                //{
                //    if (gameObject.GetHashCode() == GameManager.instance.bodyTracker[i].GetHashCode())
                //    {
                //        GameManager.instance.bodyTracker.Remove(GameManager.instance.bodyTracker[i]);
                //    }
                //}
                GameManager.instance.DeclareSelfDead(gameObject);
                ResetHP();


                //
                //while (GameManager.instance.statsTracker[name] > 0)

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
        {
            isShooting = true;

            Instantiate(bullet, shootPos.position, transform.rotation);
            Damage bulletDmg = bullet.GetComponent<Damage>();
            bulletDmg.parent = this.gameObject;
            //Debug.Log(bulletDmg.parent.name.ToString() + " HURT " + bulletDmg.victim.name.ToString());
            //if (bulletDmg.hasKilled)
            //{
            //    Debug.Log(bulletDmg.hasKilled.ToString());
            //    Debug.Log(bulletDmg.victim.ToString());
            //    GameManager.instance.statsTracker[this.gameObject.name].updateKills();
            //    GameManager.instance.statsTracker[this.gameObject.name].updateKDR();
            //    GameManager.instance.DisplayKillMessage(gameObject, bulletDmg.victim);
            //    Debug.Log(GameManager.instance.statsTracker[this.gameObject.name].getAllStats());
            //}
            //Debug.Log(bulletDmg.parent.ToString() + " --> BULLET!");
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
