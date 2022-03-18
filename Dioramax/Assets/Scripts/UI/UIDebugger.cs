using UnityEngine;
using UnityEngine.SceneManagement; // DEBUG : this won't be here later on

public class UIDebugger : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(int index)
    {
        if (index > SceneManager.sceneCountInBuildSettings - 1 || index < 0) return; 
        SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
    }
}
