using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraController : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [SerializeField] float maxTime;
    [SerializeField] AudioClip detected;
    //[SerializeField] SphereCollider sphere;

    [SerializeField] CameraSphere camSphere;
    float totalTime;
    bool playerSpotted = false;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (playerSpotted)
        {

            totalTime += Time.deltaTime;
            if (totalTime > maxTime)
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
        if (totalTime <= 0)
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
        List<GameObject> enemies = camSphere.enemiesInRange;
        if (enemies.Count > 0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemyAI enemy = enemies[i]?.GetComponent<enemyAI>();
                if (enemy != null)
                {
                    if (!isRoaming)
                    {
                        GameManager.instance.PriorityPoint.Add(gameObject.transform);
                    }
                    else
                    {
                        GameManager.instance.PriorityPoint.Remove(gameObject.transform);
                    }
                }
            }
        }
    }
}