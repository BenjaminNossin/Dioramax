using UnityEngine;
using DG.Tweening;
public class Turn_Moulin : MonoBehaviour
{
    [SerializeField] float rotation_degrees;
    [SerializeField] float time_rotation;

    // Start is called before the first frame update
    void Start()
    {
        //rotation

       // LeanTween.rotateAround(gameObject, Vector3.right, rotation_degrees, time_rotation).setEaseOutBack().setLoopCount(-1);
        transform.DOLocalRotate((Vector3.right * (rotation_degrees)), time_rotation, RotateMode.FastBeyond360).SetEase(Ease.OutBack).SetLoops(-1);
    }
}
