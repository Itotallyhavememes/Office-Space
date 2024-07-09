using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBall : MonoBehaviour
{
    //[SerializeField] float force;
    [SerializeField] Rigidbody paperBallBody;
    [SerializeField] int rotationSpeed;
    // Start is called before the first frame update
    //void Start()
    //{       

    //    paperBallBody.AddForce(Camera.main.transform.forward * force, ForceMode.Impulse);
    //    Destroy(gameObject, 2f);
    //}

    private void Update()
    {
        paperBallBody.transform.localRotation = Quaternion.Euler(0, rotationSpeed, 0);
    }
}
