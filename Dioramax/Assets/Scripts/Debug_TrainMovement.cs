using UnityEngine;

public class Debug_TrainMovement : MonoBehaviour
{
    [SerializeField] private FreezeStateController stateController;
    [SerializeField] private bool isGameplayTrain;
    private bool canBeTeleported;

    private void OnEnable()
    {
        Debug_Teleporter.OnTeleportCallback += Teleport;
    }

    private void OnDisable()
    {
        if (isGameplayTrain)
        {

        }

        Debug_Teleporter.OnTeleportCallback -= Teleport;
    }

    private void Start()
    {
        TrainsManager.AllTrainsFreezeController.Add(stateController); 
    }

    private void Update()
    {
        canBeTeleported = !stateController.Freezed; 
    }

    private void Teleport(Transform targetTransf)
    {
        if (canBeTeleported)
        {
            if (isGameplayTrain && TrainsManager.IsOn)
            {
                TrainsManager.ParentOnStickTrain.SetPositionAndRotation(targetTransf.position, Quaternion.Euler(targetTransf.rotation.eulerAngles));
            }
            else
            {
                transform.parent.SetPositionAndRotation(targetTransf.position, Quaternion.Euler(targetTransf.rotation.eulerAngles));
            }
        }
    }
}
