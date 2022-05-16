using UnityEngine;
using UnityEngine.SceneManagement; 

public class CustomSceneManager : MonoBehaviour
{
    public static CustomSceneManager Instance; 

    [SerializeField] private bool isMainMenu;
    [SerializeField] private DioramaName dioramaToLoad = DioramaName.Tutorial; // will be modified via save system with SetDioramaToLoad

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
        }

        Instance = this; 
    }

    private void Update()
    {
        // use a specific button if needed
        if (isMainMenu && Input.GetTouch(0).phase == TouchPhase.Began)
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
