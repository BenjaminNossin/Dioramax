using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water_Exploding : MonoBehaviour
{

    [SerializeField] float time_stretchsquash;
    [SerializeField] Vector3 stretch_squash;

    // Start is called before the first frame update
    void Start()
    {
        //stretch&squash
        LeanTween.scale(gameObject, stretch_squash, time_stretchsquash).setEasePunch().setLoopCount(-1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
