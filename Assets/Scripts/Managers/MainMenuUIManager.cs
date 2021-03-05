using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
       // SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
