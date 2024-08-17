using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleIcons : MonoBehaviour
{
    [SerializeField] Toggle p1Toggle;
    [SerializeField] Toggle p2Toggle;
    [SerializeField] Toggle p3Toggle;
    [SerializeField] Toggle p4Toggle;

    [SerializeField] Text p1TogText;
    [SerializeField] Text p2TogText;
    [SerializeField] Text p3TogText;
    [SerializeField] Text p4TogText;

    [SerializeField] Text b1TogText;
    [SerializeField] Text b2TogText;
    [SerializeField] Text b3TogText;
    [SerializeField] Text b4TogText;

    void onPlayerJoin()
    {
        int playerCount = PlayerManager.instance.players.Count;
        p1Toggle.colors.disabledColor.Equals(Color.red);
    }
}
