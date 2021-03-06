using UnityEngine;
using TMPro;
using System.Collections; 

public class TutorialPromptsUI : MonoBehaviour
{
    public static TutorialPromptsUI Instance;

    [SerializeField] private AudioSource audiosource;
    [SerializeField] private TutorialTextHolder tutorialTextHolder;
    [SerializeField] private GameObject textHolderPanel;
    [SerializeField] private TMP_Text tmpText;

    public void Awake()
    {
        if (Instance)
        {
            Destroy(Instance); 
        }


        Instance = this;
        tutorialTextHolder.SetTmpReference(tmpText);
    }

    private void Start()
    {
        if (OverridenCinematic) return; 
        textHolderPanel.SetActive(false); 
    }

    public static bool OverridenCinematic; 
    public void ShowStartingPrompt()
    {
        StartCoroutine(ShowPanel(0, 0));
        Debug.Log("showing starting prompt");
    }

    public void ShowNextPrompt(int index, int subIndex)
    {
        HidePanel();
        StartCoroutine(ShowPanel(index, subIndex));
    }

    private readonly WaitForSeconds WFS = new(0f);
    private IEnumerator ShowPanel(int index, int subIndex)
    {
        yield return WFS;

        audiosource.Play();

        textHolderPanel.SetActive(true);
        tutorialTextHolder.ShowPrompt(index, subIndex);
    }

    // use a fadeout tween instead
    public void HidePanel()
    {
        textHolderPanel.SetActive(false);
    }
}
