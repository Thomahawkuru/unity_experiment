using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
public class SceneLoader : MonoBehaviour
{
    [Header("Scenes")]
    public int NumberOfScenes = 6;

    public KeyCode nextScene = KeyCode.RightArrow;
    public KeyCode previousScene = KeyCode.LeftArrow;

    public void Update()
    {
        if (Input.GetKeyDown(nextScene) && IsNextScene())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (Input.GetKeyDown(previousScene) && IsPreviousScene())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }

    public bool IsNextScene()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 < NumberOfScenes) { return true; }
        else { return false; }
    }

    public bool IsPreviousScene()
    {
        if (SceneManager.GetActiveScene().buildIndex > 1) { return true; }
        else { return false; }
    }
}