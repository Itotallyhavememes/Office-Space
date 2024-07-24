using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomizeSpawner : MonoBehaviour
{
    [SerializeField] enum itemType { ITEM, OBJECTIVE }
    [SerializeField] itemType type;

    [SerializeField] GameObject[] items;
    [SerializeField] float spawnTimer;
    [SerializeField] int spawningThreshold;
    bool isSpawning;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if ((type == itemType.ITEM && GameManager.instance.worldItemCount < spawningThreshold) ||
            (type == itemType.OBJECTIVE && GameManager.instance.worldDonutCount < spawningThreshold))
            if (!isSpawning)
                StartCoroutine(SpawnItem());
    }

    IEnumerator SpawnItem()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnTimer);
        isSpawning = false;

        Vector3 randPos = Random.insideUnitSphere * Random.Range(1,100);

        NavMeshHit hit;

        NavMesh.SamplePosition(randPos, out hit, 100, 1);

        Vector3 spawnPos = hit.position;
        spawnPos.y += 1;

        Instantiate(items[Random.Range(0, items.Length)], spawnPos, transform.rotation);

    }
}
