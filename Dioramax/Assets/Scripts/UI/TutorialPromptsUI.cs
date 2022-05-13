using UnityEngine;
using TMPro;
using System.Collections; 

public class TutorialPromptsUI : MonoBehaviour
{
    public static TutorialPromptsUI Instance; 

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
    }

    private void Start()
    {
        textHolderPanel.SetActive(false); 
        tutorialTextHolder.SetTmpReference(tmpText);
    }

    public void ShowStartingPrompt()
    {
        Debug.Log("showing starting prompt"); 
        textHolderPanel.SetActive(true); 
        tutorialTextHolder.ShowPrompt(0, 0);

        StartCoroutine(HidePanel()); 
    }

    public void ShowPrompt(int index, int subIndex)
    {
        textHolderPanel.SetActive(true);
        tutorialTextHolder.ShowPrompt(index, subIndex);

        StartCoroutine(HidePanel());
    }

    public void OverrideHidePanelDelay()
    {
        textHolderPanel.SetActive(false);
        StopCoroutine(HidePanel()); 
    }

    // use a fadeout tween instead
    private readonly WaitForSeconds WFS = new(5f);
    private IEnumerator HidePanel()
    {
        yield return WFS;
        textHolderPanel.SetActive(false);
    }
}
