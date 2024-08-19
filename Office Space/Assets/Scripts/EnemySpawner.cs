using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //[SerializeField] float min;
    //[SerializeField] float max;
    //[SerializeField] float stopTime;
    //[SerializeField] GameObject[] enemyPrefabs;
    //private float time;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.AddToSpawnList(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //public void BotSpawner()
    //{
        //int rand = Random.Range(0, enemyPrefab.Length);
        //GameObject enemy = enemyPrefab[rand];
        //Instantiate(enemy, transform.position, Quaternion.identity);
        //enemy.transform.position = new Vector3(transform.position.x + transform.forward.x, transform.position.y, transform.position.z);
        //GameManager.instance.enemyCount++;
        

    //}

    //private void SetTimeSpawner()
    //{
    //    time = Random.Range(min, max);
    //}

    //private void DonutKing1Spawner() //Saved Donut King 1 enemy spawn logic
    //{
    //    if (GameManager.instance.enemyCount <= GameManager.instance.Thresh)
    //    {

    //        int rand = Random.Range(0, enemyPrefab.Length);
    //        GameObject enemy = enemyPrefab[rand];
    //        Instantiate(enemy, transform.position, Quaternion.identity);
    //        enemy.transform.position = new Vector3(transform.position.x + transform.forward.x, transform.position.y, transform.position.z);
    //        //GameManager.instance.enemyCount++;
    //        SetTimeSpawner();


    //    }
    //}


}
