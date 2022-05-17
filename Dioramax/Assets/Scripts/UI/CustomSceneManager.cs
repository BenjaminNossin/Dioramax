using UnityEngine;
using UnityEngine.SceneManagement; 

public class CustomSceneManager : MonoBehaviour
{
    [SerializeField] private bool isMainMenu;
    [SerializeField] private DioramaName dioramaToLoad = DioramaName.Tutorial; // will be modified via save system with SetDioramaToLoad

    [Header("--DEBUG--")]
    [SerializeField] private bool dontDestroyOnLoad;

    private void Awake()
    {
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
        
    public void LoadLastSavedDiorama()
    {
        if (isMainMenu)
        {
            SceneManager.LoadSceneAsync((int)dioramaToLoad + 1, LoadSceneMode.Single);
            // have a fade out screen. When it's done, allowSceneActivation = true; 
        }        
    }

    public void LoadScene(int index)
    {
        if (index > SceneManager.sceneCountInBuildSettings - 1 || index < 0) return; 
        SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
    }

    private int currentScene; 
    public void LoadNextScene()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        if (currentScene == 4) return; 

        SceneManager.LoadSceneAsync(currentScene + 1, LoadSceneMode.Single);
    }

    public void SetDioramaToLoad(DioramaName diorama)
    {
        dioramaToLoad = diorama; 
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
