using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor.SceneManagement;
using UnityEngine.InputSystem.LowLevel;
using System;
using UnityEngine.InputSystem.Utilities;


public class AttractModeScript : MonoBehaviour
{
    AsyncOperation asyncUnloadLevel;
    AsyncOperation asyncLoadLevel;
    [SerializeField] int InactiveTimer;
    ControllerTest testControl;
    string nextScene;
    string currentScene;
    bool switchScences;
    public static IObservable<InputControl> onAnyButton {  get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        switchScences = false;
    }

    // Update is called once per frame
    void Update()
    {
        InputSystem.onAnyButtonPress.CallOnce(ctrl => Debug.Log("Button { ctrl } was pressed"));
            currentScene = SceneManager.GetActiveScene().name;
            Debug.Log("Start");
            StartCoroutine(playerDelay());
        if (switchScences == true)
        {
            if (currentScene == "Title - Derrick")
            {
                Debug.Log("gamePlay");
                nextScene = "TTT5 - Derrick";
            }
            else if (currentScene == "TTT5 - Derrick")
            {
                Debug.Log("Menu");
                nextScene = "Title - Derrick";
            }
            StartCoroutine(LoadNewScene());
            switchScences = false;
        }
        
    }

    IEnumerator LoadNewScene()
    {
        asyncLoadLevel = EditorSceneManager.LoadSceneAsync(nextScene);
        while (!asyncLoadLevel.isDone)
        {
            Debug.Log("loading");
            yield return null;
        }
        yield return new WaitForSeconds(1);
        asyncUnloadLevel = EditorSceneManager.UnloadSceneAsync(currentScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        while (!asyncUnloadLevel.isDone)
        {
            yield return null;
        }
    }
    IEnumerator playerDelay()
    {
        int time = 0;
        while(time <= InactiveTimer)
        {
            yield return new WaitForSeconds(1);
            time++;
            Debug.Log(time.ToString());
        }
        switchScences = true;
        Debug.Log("Out");
    }
}
