using UnityEngine;

public class SelectDioramaOnMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] dioramas;
    [SerializeField] private bool isMainMenu = true; 
    private int selector; 

    void OnEnable()
    {
        if (isMainMenu)
        {
            selector = Random.Range(0, 2);
            dioramas[selector].SetActive(true);
        }
    }
}
