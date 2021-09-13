using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartExperiment : MonoBehaviour
{   
    public int subjectID = 0;

    [HideInInspector]
    public static string subjectDataFolder;
    public static bool IsStarted = false;

    private void Start()
    {
        // Varjo.VarjoPlugin.ResetPose(true, Varjo.VarjoPlugin.ResetRotation.ALL);
        Debug.Log($"Subject Name = {subjectID.ToString()}...");

        //Set input of data log folder:
        subjectDataFolder = string.Join("/", System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop).Replace("\\", "/"), "Data", subjectID + "_" + System.DateTime.Now.ToString("MM-dd_HH-mm"));
        Debug.Log($"Creating {subjectDataFolder}...");
        System.IO.Directory.CreateDirectory(subjectDataFolder);
        IsStarted = true;
    }
}
