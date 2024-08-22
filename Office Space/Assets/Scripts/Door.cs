using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    //public GameObject Instruction;
    public GameObject AnimeObject;
   
    [SerializeField] bool /*isSomeoneThere,*/ isOutward, isInward; //Indicates whether someone is STILL inside there
    [SerializeField] GameObject OuterPoint;
    [SerializeField] GameObject InnerPoint;
    [SerializeField] bool shouldClose; //Test Variable for DEBUG
    [SerializeField] List<GameObject> inMyDoorway;
    //[SerializeField] GameObject LastInDoor;//Entity who intially triggered my doorway
    [Header("----- Sounds -----")]
    [SerializeField] AudioClip audDoorOpen;
    [Range(0f, 1f)][SerializeField] float audDoorOpenVol;
    [SerializeField] AudioClip audDoorClose;
    [Range (0f, 1f)] [SerializeField] float audDoorCloseVol;
    bool Action;
    bool close;

    // Start is called before the first frame update
    void Start()
    {

        close = true;
        //isSomeoneThere = false;
        inMyDoorway = new List<GameObject>();
        //Helps for ResetDoor()
        //Debug.Log(DoorShutPositionY);
        //Instruction.SetActive(false);

    }
    IEnumerator CloseDoor()
    {
        //close = false;
        yield return new WaitForSeconds(3f);
        Debug.Log("AD: door is Close");
        AnimeObject.GetComponent<Animator>().Play("Close");
        //Instruction.SetActive(false);
        //close = true;
       

    }

    public void ResetDoors()
    {
        Debug.Log("AD: RESETTING DOORS!");
        AnimeObject.GetComponent<Animator>().Play("ResetDoor");
        if (!close)
            close = true;
        //if(isSomeoneThere)
        //    isSomeoneThere = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        //if(other.CompareTag("Player") && close)
        //{
        //    Debug.Log("Enter");
        //    //Instruction.SetActive(true);
        //    Action = true;
        //}

        //CHECKS TO SEE IF OBJECT COLLIDING WITH BOX COLLIDER IS AN ENTITY (PLAYER/ENEMY)
        if (other == other.GetComponentInChildren<CapsuleCollider>())
        {
            float PointToOuter = 0.0f;
            float PointToInner = 0.0f;
            //CHECKS TO SEE IF THAT PLAYER/ENEMY IS ALIVE (IMPORTANT BECAUSE IF DEAD: DOOR CLOSES)
            GameObject compare = GameManager.instance.ReturnEntity(other.gameObject);
            
            if (compare != null)
            {
                //Grabbed other's transform position in respects TO: PointToOuter (point outside Door) AND PointToInner (point inside Door)
                //From HERE:
                //Check to see which one is FURTHEST from other.transform.position (gameobject)
                //IF Outer is Furthest - Play Open Animation (Opens OUTWARD)
                //ELSE Inner is Furthest - Plat Open1 Animation (Opens INWARD)
                //NOTE FOR FUTURE: REMEMBER to create Close (From Outer) AND Close1 (From Inner)
                //NEED to create BOOLS for Outer vs Inner

                //CHECK TO SEE IF WE CAN EVEN BEGIN TO CONSIDER OPENING:
                //Regardless, Add them to the List
                AddToDoorList(compare);

                //BEGIN ANIMATING:
                //This next section provides logic for when applicable person enters and we begin to ANIMATE:
                if (inMyDoorway.Count == 1 && close)
                {
                    PointToOuter = Vector3.Distance(OuterPoint.transform.position, other.gameObject.transform.position);
                    PointToInner = Vector3.Distance(InnerPoint.transform.position, other.gameObject.transform.position);
                    if (PointToOuter > PointToInner /*&& other.gameObject != LastInDoor*/)
                    {
                        GameManager.instance.playerScript.Munch(audDoorOpen, audDoorOpenVol);
                        AnimeObject.GetComponent<Animator>().Play("Open");
                        isOutward = true;
                    }
                    else
                    {
                        GameManager.instance.playerScript.Munch(audDoorOpen, audDoorOpenVol);
                        AnimeObject.GetComponent<Animator>().Play("InsideOpenDoor");
                        isInward = true;
                    }
                    //BOOLS ARE NECESSARY, to know WHICH Close to Play when OnTriggerExit (If isOutward, play Close (from outward)
                    //ELSE: OnTriggerExit -> Play Close (From Inward);
                    //AnimeObject.GetComponent<Animator>().Play("Open");
                    close = false;
                }
            }
        }
    }
      

    private void OnTriggerExit(Collider other)
    {
        //Needs to ALSO check if someone else is STILL there (OnTriggerEnter SHOULD handle this - need to test)

        //Only way isSomeoneThere = false, should be if NO ONE is left inside Box Collider
        //However: currently, this only cares about whoever leaves, doesn't care about who is still in there
        
        //This makes sure that if anyone tries to re-enter the door immediately while it's playing the close animation, that the door doesn't open again immediately
        //This can be remedited with LastInDoor AND extending the door's collider HORIZONTALLY
        //if (LastInDoor == null)
        //{
        //    LastInDoor = other.gameObject;
            
        //}
        //else if(LastInDoor == other.gameObject)
        //{
        //    StartCoroutine(ForgetLastInDoor());
        //    //isSomeoneThere = false;
        //}
        RemoveFromDoorList(other.gameObject);
        if (inMyDoorway.Count == 0 && !close)
        {
            shouldClose = true; //Debug Test
            Debug.Log("AD: NO ONE HERE!");
            if (isOutward)
            {
                Debug.Log("AD: CLOSING!");
                //Play Close(from Outward)
                isOutward = false;
                //StartCoroutine(CloseDoor()); //Currently meant to close FromOutward
                GameManager.instance.playerScript.Munch(audDoorClose, audDoorCloseVol);
                AnimeObject.GetComponent<Animator>().Play("Close");
            }
            else if (isInward)
            {
                Debug.Log("AD: CLOSING!");
                //Play Close(From Inward)
                GameManager.instance.playerScript.Munch(audDoorClose, audDoorCloseVol);
                AnimeObject.GetComponent<Animator>().Play("CloseInsideDoor");
                isInward = false;
            }
            close = true;
            shouldClose = false; //Debug Test
        }
    }

    IEnumerator ForgetLastInDoor()
    {
        //Minor bug, where this doesn't register for full 3 seconds - TIM
        //UPDATE: creating an additional CASE where LastInDoor is NOT null does not trigger door opening
        //Player can NOW walk back and forth IN FRONT OF DOOR (only from Inside atm) to not trigger door opening animation (outward)
        yield return new WaitForSeconds(0.5f);
        //LastInDoor = null;
    }

    void AddToDoorList(GameObject doorUser)
    {
        bool canAdd = true;

        if (inMyDoorway.Count > 0)
        {
            foreach (var person in inMyDoorway)
            {
                if (person.gameObject == doorUser)
                {
                    canAdd = false;
                    break;
                }
            }
        }
        if(canAdd)
            inMyDoorway.Add(doorUser);
    }

    public void RemoveFromDoorList(GameObject doorUser)
    {
        foreach (var person in inMyDoorway)
        {
            if(person.gameObject == doorUser)
            {
                inMyDoorway.Remove(person);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //CHECK YOUR DOORWAY TO MAKE SURE YOU DON'T HAVE THE DEAD GUY:
        //LOGIC FOR CLOSE:
        //CHECKS 2 THINGS - isSomeoneThere == false && isOutward or isInward
        //if (!isSomeoneThere && !close)
        //{
        //    Debug.Log("AD: NO ONE HERE!");
        //    if (isOutward)
        //    {
        //        Debug.Log("AD: CLOSING!");
        //        //Play Close(from Outward)
        //        isOutward = false;
        //        //StartCoroutine(CloseDoor()); //Currently meant to close FromOutward
        //        GameManager.instance.playerScript.Munch(audDoorClose, audDoorCloseVol);
        //        AnimeObject.GetComponent<Animator>().Play("Close");
        //    }
        //    else if (isInward)
        //    {
        //        Debug.Log("AD: CLOSING!");
        //        //Play Close(From Inward)
        //        GameManager.instance.playerScript.Munch(audDoorClose, audDoorCloseVol);
        //        AnimeObject.GetComponent<Animator>().Play("CloseInsideDoor");
        //        isInward = false;
        //    }
        //    close = true;
        //}


        //if( Input.GetButtonDown("Interact"))
        //{
        //    Debug.Log("F is pess");
        //    if(Action )
        //    {
        //        Debug.Log("door is open");
        //        //Instruction.SetActive(false );
        //        close = false;
        //        AnimeObject.GetComponent<Animator>().Play("Open");
        //      StartCoroutine(CloseDoor());
        //        Action = false;
               
        //    }
           
        //}
        
    }
}
