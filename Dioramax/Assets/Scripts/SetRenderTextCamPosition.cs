using UnityEngine;

public class SetRenderTextCamPosition : MonoBehaviour
{
    [SerializeField] private Transform mainCam; 

    public void ResetPosition()
    {
        transform.localPosition = mainCam.localPosition;
    }
}
