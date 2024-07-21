using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SecPatrol : MonoBehaviour
{
    public Transform currentPoints ;
    [SerializeField] int PatrolTimer;
     public GameObject SecPoint;
    [SerializeField] NavMeshAgent agent;
    bool isPatrol; 


    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public IEnumerator Patrol()
    {
        isPatrol = true;
     yield return new WaitForSeconds(PatrolTimer);

        Vector3 point = currentPoints.position - transform.position;
        if(Vector3.Distance(transform.position, currentPoints.position) < 0.5f && currentPoints == SecPoint.transform)
        {

        }




    }

}
