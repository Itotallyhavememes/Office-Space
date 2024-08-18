using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public GameObject Instruction;
    public GameObject AnimeObject;

  
   


    bool Action;
    bool close;

    // Start is called before the first frame update
    void Start()
    {
        close = true;
      
        Instruction.SetActive(false);

    }
    IEnumerator CloseDoor()
    {
        close = false;
        yield return new WaitForSeconds(3f);
        Debug.Log("door is Close");
        AnimeObject.GetComponent<Animator>().Play("Close");
        Instruction.SetActive(false);
        close = true;
       

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Enter");
            Instruction.SetActive(true);
            Action = true;
            
        }
    }


 
 
    // Update is called once per frame
    void Update()
    {
        if( Input.GetButtonDown("Interact"))
        {
            Debug.Log("F is pess");
            if(Action )
            {
                Debug.Log("door is open");
                Instruction.SetActive(false );
                AnimeObject.GetComponent<Animator>().Play("Open");
              StartCoroutine(CloseDoor());
                Action = false;
               
            }
           
        }
        
    }
}
