using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownScript : MonoBehaviour
{
    //[SerializeField] Dropdown botDropdown;
    public void applyDropdownBots(int index)
    {
        PlayerManager.instance.ToggleActiveBots(index);
    }

    public void applyDropdownRounds(int index)
    {
        GameManager.instance.SetRounds(index + 1); //Needs to add 1 for rounds since index 0 contains 1;
    }

}
