using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesBlink : MonoBehaviour
{
    [SerializeField] GameObject leftEye;
    [SerializeField] GameObject rightEye;
    
    [SerializeField] float blinkTime;
    [Range(1, 5)][SerializeField] float minTimeBetweenBlinks;
    [Range(1, 5)][SerializeField] float maxTimeBetweenBlinks;

bool isBlinking;
    void Update()
    {
        if (!isBlinking)
            StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        isBlinking = true;
        leftEye.SetActive(false);
        rightEye.SetActive(false);
        yield return new WaitForSeconds(blinkTime);
        leftEye.SetActive(true);
        rightEye.SetActive(true);

        yield return new WaitForSeconds(Random.Range(minTimeBetweenBlinks, maxTimeBetweenBlinks));
        isBlinking = false;
    }
}
