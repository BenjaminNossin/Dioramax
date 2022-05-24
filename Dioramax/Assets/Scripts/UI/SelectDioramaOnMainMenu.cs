using UnityEngine;

public class SelectDioramaOnMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] dioramas;
    private int selector; 

    void Start()
    {
        selector = Random.Range(0, 2);
        dioramas[selector].SetActive(true);  
    }
}
