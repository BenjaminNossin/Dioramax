using UnityEngine;

public class FollowTrain : MonoBehaviour
{
    private Transform transfToFollow = null;  
    private void OnEnable()
    {
        EntityPathNavigation.OnActivation += SetTransformToFollow;
    }

    private void OnDisable()
    {
        EntityPathNavigation.OnActivation += SetTransformToFollow;
    }

    private void Update()
    {
        if (transfToFollow || LevelManager.GameState == GameState.Playing)
        {
            transform.position = transfToFollow.position;
            //transform.LookAt(transfToFollow); 
        }
    }

    private void SetTransformToFollow(Transform _transfToFollow)
    {
        transfToFollow = _transfToFollow;
    }
}
