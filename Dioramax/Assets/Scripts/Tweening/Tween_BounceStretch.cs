using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween_BounceStretch : MonoBehaviour
{
    [SerializeField] float up_max_position;
    [SerializeField] float time_bounce;
    [SerializeField] float time_stretch;
    [SerializeField] Vector3 stretch_squash;

    // Start is called before the first frame update
    void Start()
    {

        // ! remove looping

        //bounce
        LeanTween.moveY(gameObject, up_max_position, time_bounce).setEasePunch().setLoopCount(-1);

        //stretch&squash
        LeanTween.scale(gameObject, stretch_squash, time_stretch).setEasePunch().setLoopCount(-1);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
