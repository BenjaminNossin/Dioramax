using UnityEngine;

public class Water_Exploding : StoppableTween
{

    [SerializeField] float time_stretchsquash;
    [SerializeField] Vector3 stretch_squash;

    void Start()
    {
        //stretch&squash
        LeanTween.scale(gameObject, stretch_squash, time_stretchsquash).setEasePunch();
    }
}
