using UnityEngine;

[CreateAssetMenu(fileName = "New Tutorial Text Holder", menuName = "Dioravity/Tutorial/TextHolder")]
public class TutorialTextHolder : ScriptableObject
{
    [SerializeField] private TutorialPrompt[] tutorialPrompts = new TutorialPrompt[4];
    // [SerializeField] private TMP_Text tmpText; 

    /* public void ShowPrompt(int index)
    {
        tmpText.text = tutorialPrompts[index].textToDisplay; 
    } */
}

[System.Serializable]
public class TutorialPrompt
{
    [SerializeField] private string promptName; 
    [TextArea] public string textToDisplay; 
}
