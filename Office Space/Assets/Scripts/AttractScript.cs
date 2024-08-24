using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor.SceneManagement;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Controls;
using System.Threading;



public class AttractScript : MonoBehaviour
{
    AsyncOperation asyncUnloadLevel;
    AsyncOperation asyncLoadLevel;
    //[SerializeField] int InactiveTimer;
    ControllerTest testControl;
    string nextScene;
    string currentScene;
    bool switchScences;
    float time;
    //public static IObservable<InputControl> onAnyButton {  get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        switchScences = false;
        testControl = GetComponent<ControllerTest>();
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentScene = SceneManager.GetActiveScene().name;
        InactiveTimer();
        if (switchScences == true)
        {
            if (currentScene == "Title - Derrick")
            {
                nextScene = "TTT5 - Derrick";
            }
            else if (currentScene == "TTT5 - Derrick")
            {
                nextScene = "Title - Derrick";
            }

            Debug.Log(nextScene);
        }     
    }
    void resetTimer()
    {
        time = 0;
    }

    IEnumerator LoadNewScene()
    {
        asyncLoadLevel = EditorSceneManager.LoadSceneAsync(nextScene);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        asyncUnloadLevel = EditorSceneManager.UnloadSceneAsync(currentScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        while (!asyncUnloadLevel.isDone)
        {
            yield return null;
        }
        switchScences = false;
    }
    void InactiveTimer()
    {
        while (time < 10)
        {
            InputSystem.onAnyButtonPress.CallOnce(ctr => resetTimer());
            Debug.Log(time.ToString());
            StartCoroutine(timerDelay());
            
            time++;
        }
        if(time >= 10)
        {
            StartCoroutine(LoadNewScene());
        }
        switchScences = true;
    }

    IEnumerator timerDelay()
    {
        yield return new WaitForSeconds(1);
    }
    void Sprint(InputAction.CallbackContext context)
    {
        Debug.Log("evented");
    }
}
