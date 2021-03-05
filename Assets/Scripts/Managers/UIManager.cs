using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] DebugMonitor debugMonitor;

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