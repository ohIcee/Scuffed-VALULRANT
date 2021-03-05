using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private DebugMonitor debugMonitor;
    [SerializeField] private PlayerHUD playerHUD;

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
    {

    }
}