using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playerScript.GetPlayerStatusBool())
        {
            //get input
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

            if (invertY)
                rotX += mouseY;
            else
                rotX -= mouseY;

            //clamp the rotX on the x-axis
            rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

            //rotate the camera on the x-axis
            transform.localRotation = Quaternion.Euler(rotX, 0, 0);

            //rotate the player on the y-axis
            transform.parent.Rotate(Vector3.up * mouseX);
        }

    }
}
