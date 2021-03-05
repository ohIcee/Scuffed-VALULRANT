using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] DebugMonitor debugMonitor;
    [SerializeField] PlayerHUD playerHUD;

    public void toggleDebugMonitor()
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

    public void toggleEscapeMenu()
    {

    }
}