using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchVFX : MonoBehaviour
{
    Touch touch;
    [SerializeField] ParticleSystem ParticleSystem;
    
    void Update()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // TP the VFX to the touch point
            touch = Input.GetTouch(0);
            Vector3 screenPos = new Vector3(touch.position.x, touch.position.y, 10);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            transform.position = worldPos;
            
            // Play the VFX
            ParticleSystem.Play();
        }

    }
}
