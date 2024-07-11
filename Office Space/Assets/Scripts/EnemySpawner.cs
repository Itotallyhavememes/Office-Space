using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemyPrefab;
    [SerializeField] float min;
    [SerializeField] float max;
    [SerializeField] float stopTime;
    //[SerializeField] GameObject[] enemyPrefabs;
    private float time;
  

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.enemyCount <= GameManager.instance.Thresh)
        {
            
                int rand = Random.Range(0, enemyPrefab.Length);
                GameObject enemy = enemyPrefab[rand];
                Instantiate(enemy, transform.position, Quaternion.identity);
                enemy.transform.position = new Vector3(transform.position.x + transform.forward.x, transform.position.y, transform.position.z);
                //GameManager.instance.enemyCount++;
                SetTimeSpawner();
            
            
        }
    }

    private void SetTimeSpawner()
    {
        time = Random.Range(min, max);
    }
}
