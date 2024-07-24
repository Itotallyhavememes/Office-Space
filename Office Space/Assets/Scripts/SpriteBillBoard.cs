using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillBoard : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles);
    }
}
