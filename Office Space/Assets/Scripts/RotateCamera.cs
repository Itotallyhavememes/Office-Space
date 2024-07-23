using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] float farLeft;
    [SerializeField] float farRight;
    [SerializeField] float turnSpeed;

    void Start()
    {
        StartCoroutine(CameraRotation());
    }

    IEnumerator CameraRotation()
    {
        while (true)
        {
            yield return RotateTo(Quaternion.Euler(0, farLeft, 0));
            yield return new WaitForSeconds(2f);
            yield return RotateTo(Quaternion.Euler(0, farRight, 0));
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator RotateTo(Quaternion rotation)
    {
        Quaternion start = transform.rotation;
        float time = 0f;
        float duration = Quaternion.Angle(start, rotation) / turnSpeed;

        while (time < duration)
        {
            transform.rotation = Quaternion.Slerp(start, rotation, (time / duration));
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = rotation;
    }
}
