using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
public class ExperimentManager : MonoBehaviour
{
    public int subjectID = 0;

    [Header("Scenes")]
    public string Startup = "Startup";
    public string Calibration = "Calibration";
    public string Scene1 = "Scene 1";
    public string Scene2 = "Scene 2";
    public string TheEnd = "The End";

    [Header("Input")]
    public KeyCode nextScene = KeyCode.RightArrow;
    public KeyCode previousScene = KeyCode.LeftArrow;

    [HideInInspector]
    public static string subjectDataFolder;
    public static bool loaded = false;
    public static bool IsStarted = false;
    public static string[] sceneArray;
    public static int sceneIndex;

    private void Start()
    {
        sceneIndex = 0;
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("Manager"));

        // Varjo.VarjoPlugin.ResetPose(true, Varjo.VarjoPlugin.ResetRotation.ALL);
        if (subjectID % 2 == 0)
        {
            string[] array = { Startup, Calibration, Scene2, TheEnd, Calibration, Scene1, TheEnd };
            sceneArray = array;
        }
        else
        {            
            string[] array = { Startup, Calibration, Scene1, TheEnd, Calibration, Scene2, TheEnd };
            sceneArray = array;
        }

        //Set input of data log folder:
        subjectDataFolder = "C:/Users/thoma/Google Drive/Thesis/Experiment/DataCrunch/Data/" + subjectID + "_" + System.DateTime.Now.ToString("MM-dd_HH-mm");
        //Debug.Log($"Creating {subjectDataFolder}...");
        System.IO.Directory.CreateDirectory(subjectDataFolder);

        loaded = false;
        IsStarted = true;
    }


    private void Update()
    {
        if (Input.GetKeyDown(nextScene) && IsNextScene() && loaded == false)
        {
            //Debug.Log("Next Scene selected");
            LoadYourScene(GetNextScene());
        }
        else if(Input.GetKeyDown(previousScene) && IsPreviousScene() && loaded == false)
        {
            //Debug.Log("Next Scene selected");
            LoadYourScene(GetPreviousScene());
        }
        else { loaded = false; }
    }

    private void LoadYourScene(string sceneName) 
    {
        //Debug.Log($"Loading next scene: {sceneName}...");
        SceneManager.LoadScene(sceneName);
        loaded = true;
    }

    private string GetNextScene()
    {
        string nextScene = sceneArray[sceneIndex];
        return nextScene;
    }

    private string GetPreviousScene()
    {        
        string nextScene = sceneArray[sceneIndex];        
        return nextScene;
    }
    private bool IsNextScene()
    {
        if (sceneIndex < sceneArray.Length-1) { sceneIndex++; return true; }
        else { return false; }
    }

    private bool IsPreviousScene()
    {
        if (sceneIndex > 1) { sceneIndex--; return true; }
        else { return false; }
    }
}