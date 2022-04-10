using UnityEngine;
using UnityEditor; 

public class GDDLinker : MonoBehaviour
{
    [MenuItem("Dioravity/GDD")]
    static void OpenMainPage()
    {
        Application.OpenURL("https://sites.google.com/rubika-edu.com/gamedesigndocument/accueil");
    }
}
