using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class UIManager : MonoBehaviour
{
    [SerializeField] private DebugMonitor debugMonitor;
    [SerializeField] private PlayerHUD playerHUD;
    [SerializeField] private Canvas EscapeMenu;


    public void ToggleDebugMonitor()
    {
        if (debugMonitor.gameObject.activeSelf)
        {
            debugMonitor.gameObject.SetActive(false);
        }
        else
        {
            debugMonitor.gameObject.SetActive(true);
        }
    }

    public void ToggleEscapeMenu()
    { /*
        if (EscapeMenu.gameObject.activeSelf)
        {
            EscapeMenu.gameObject.SetActive(false);
        }
        else
        {
            EscapeMenu.gameObject.SetActive(true);
        }*/
    }
    public void LoadMainMenu()
    {
        Debug.Log("main menu button pressed");
        SceneManager.LoadScene(0);
    }
}