using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownScript : MonoBehaviour
{
    public void applyDropdownBots(int index)
    {
        GameManager.instance.SetBotCount(index);
    }
    
    public void applyDropdownRounds(int index)
    {
        GameManager.instance.SetRounds(index + 1); //Needs to add 1 for rounds since index 0 contains 1;
    }
   
}
