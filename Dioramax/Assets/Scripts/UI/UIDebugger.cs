using UnityEngine;
using UnityEngine.SceneManagement; // DEBUG : this won't be here later on

public class UIDebugger : MonoBehaviour
{
    public bool dontDestroyOnLoad; 
    void Awake()
    {
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadScene(int index)
    {
        if (index > SceneManager.sceneCountInBuildSettings - 1 || index < 0) return; 
        SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
