using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn_Manege : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1) // un seul doigt sur l'écran
        {
            if (Input.GetTouch(0).phase = TouchPhase.Begin)
            {
                // Ton code ici lorsque le doigt touche l'écran durant la première frame


            }
        }
    }
}
