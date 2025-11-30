using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public string mainSceneName = "MainScene"; 

    public void GoBack()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}