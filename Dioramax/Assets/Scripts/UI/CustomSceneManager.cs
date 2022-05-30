using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;

public class CustomSceneManager : MonoBehaviour
{
    [SerializeField] private bool isMainMenu;
    [SerializeField] private bool isLevelSelection;
    [SerializeField] private DioramaName dioramaToLoad = DioramaName.Tutorial; // will be modified via save system with SetDioramaToLoad

    [Header("--DEBUG--")]
    [SerializeField] private bool dontDestroyOnLoad;
    private bool isSharing;

    public string fullPath;
    private AsyncOperation asyncOp;

    private void Awake()
    {
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        UIFadeOut.Instance.DoFadeOut();      
    }

    public void LoadLastSavedDiorama()
    {
        if (isMainMenu)
        {
            asyncOp = SceneManager.LoadSceneAsync((int)dioramaToLoad + 1, LoadSceneMode.Single);
            StartCoroutine(AllowSceneActivation());
            UIFadeIn.Instance.DoFadeIn();
        }
    }

    public void LoadScene(int index)
    {
        if (index > SceneManager.sceneCountInBuildSettings - 1 || index < 0) return;
        asyncOp = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        StartCoroutine(AllowSceneActivation());
        UIFadeIn.Instance.DoFadeIn();
    }

    public void ReloadScene()
    {
        asyncOp = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        StartCoroutine(AllowSceneActivation());
        UIFadeIn.Instance.DoFadeIn();
    }

    private int currentScene;
    public void LoadNextScene()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        if (currentScene == 4) return;

        asyncOp = SceneManager.LoadSceneAsync(currentScene + 1, LoadSceneMode.Single);
        StartCoroutine(AllowSceneActivation());
        UIFadeIn.Instance.DoFadeIn();
    }

    public void Share()
    {
        // StartCoroutine(TakeScreenshotAndShare());
        StartCoroutine(CaptureScreenshot());
    }

    public void SetDioramaToLoad(DioramaName diorama)
    {
        dioramaToLoad = diorama;
    }

    private WaitForSeconds WFS = new(1f);
    private IEnumerator TakeScreenshotAndShare()
    {
        isSharing = true;
        yield return new WaitForEndOfFrame();

        Texture2D ss = new(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(string.Concat(Application.temporaryCachePath, "shared img.png"));
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        Destroy(ss);

        new NativeShare().AddFile(filePath)
            .SetSubject("Subject goes here").SetText("Hello world!").SetUrl("https://github.com/yasirkula/UnityNativeShare")
            .SetCallback((result, shareTarget) => GameLogger.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();

        yield return WFS;
        isSharing = false;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    private string oldName, newName;
    private int screenShotnNbr; 
    private IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();

        /*oldName = $"{DateTime.Now}";
        GameLogger.Log($"old : { oldName }");

        newName = oldName.Replace("/", ".");
        GameLogger.Log($"new : {newName}"); */

        fullPath = Application.persistentDataPath + "\\" + "_" + screenShotnNbr + ".jpg";
        GameLogger.Log($"full path : {fullPath}");
        screenShotnNbr++; 

        // android path : Galaxy A6\Phone\Android\data\com.DefaultCompany.Dioravity

        Texture2D screenImage = new(Screen.width, Screen.height);
        //Get Image from screen
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        //Convert to png
        byte[] imageBytesJPG = screenImage.EncodeToJPG();

        //Save image to file
        File.WriteAllBytes(fullPath, imageBytesJPG);
    }

    private readonly WaitForSeconds fadeWFS = new(0.8f);
    private IEnumerator  AllowSceneActivation()
    {
        asyncOp.allowSceneActivation = false;

        yield return fadeWFS;
        asyncOp.allowSceneActivation = true;        
    }
}


