using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SecurityCameraController : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [SerializeField] float maxTime;
    [SerializeField] AudioClip detected;
    [SerializeField] GameObject sphere;

    float totalTime;
    bool playerSpotted = false;


    // Update is called once per frame
    void Update()
    {
        if (playerSpotted)
        {

            totalTime += Time.deltaTime;
            if(totalTime > maxTime)
            {
                updateEnemies(false);
            }
        }
        if (!playerSpotted)
        {
            if (totalTime > 0)
            {
                totalTime--;//Reduces time in camera but not instantly so if you go out and right back in it compounds
            }
        }
        if(totalTime == 0)
        {
            updateEnemies(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSpotted = true;
            aud.PlayOneShot(detected);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSpotted = false;
        }
    }



    public void updateEnemies(bool isRoaming)
    {
        List<GameObject> enemies = sphere.GetComponent<CameraSphere>().enemiesInRange;
        for(int i = 0; i < enemies.Count; i++)
        {
            enemyAI enemy = enemies[i].GetComponent<enemyAI>();
            if (!isRoaming)
            {
                enemy.playerSpotted = true;
                NavMeshAgent agent = enemy.agent;
                agent.SetDestination(gameObject.transform.position);
            }
            else
            {
                enemy.playerSpotted = false;
                enemy.GoOnPatrol();
            }
        }
    }
}
