using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomizeSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] items;
    [SerializeField] float spawnTimer;
    bool isSpawning;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isSpawning)
            StartCoroutine(SpawnItem());
    }

    IEnumerator SpawnItem()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnTimer);
        isSpawning = false;

        Vector3 randPos = Random.insideUnitSphere * 100;
        randPos.y = 1f;

        NavMeshHit hit;
        NavMesh.SamplePosition(randPos, out hit, 100, 1);
        Instantiate(items[Random.Range(0, items.Length)], hit.position, transform.rotation);
    }
}
