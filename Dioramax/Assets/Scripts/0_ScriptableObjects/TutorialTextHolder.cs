using UnityEngine;
using System.Collections.Generic;
using TMPro;

[CreateAssetMenu(fileName = "New Tutorial Text Holder", menuName = "Dioravity/Tutorial/TextHolder")]
public class TutorialTextHolder : ScriptableObject
{
    [SerializeField] private TutorialPrompt[] tutorialPrompts = new TutorialPrompt[4];
    private TMP_Text tmpText; 

    public void SetTmpReference(TMP_Text reference)
    {
        tmpText = reference;
    }

    // add a little tween
    public void ShowPrompt(int index, int subIndex)
    {
        tmpText.text = tutorialPrompts[index].section[subIndex].textToDisplay; 
    }
}

[System.Serializable]
public class TutorialPrompt
{
    [SerializeField] private string promptName;
    public List<TutorialPromptSection> section; 
}


[System.Serializable]
public class TutorialPromptSection
{
    public int index; 
    [TextArea] public string textToDisplay;
}
