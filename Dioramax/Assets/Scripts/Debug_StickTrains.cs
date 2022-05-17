using UnityEngine;
using System.Collections.Generic; 

public class Debug_StickTrains : MonoBehaviour
{
    [SerializeField] private Transform parent;

    [Space, SerializeField] private Transform locoStickPoint;
    [SerializeField] private Transform wagonStickPoint;

    [Space, SerializeField] private bool freezeAllExceptSelected = true;
    public static List<FreezeStateController> AllTrainsFreezeController { get; private set; }

    public static bool IsOn;
    public static Transform Parent;

    [Space] public bool freezeAllOnDetach = true;  
    public static System.Action OnDetachChildren { get; set; }

    private void OnEnable()
    {
        TouchDetection.OnTrainDetection += UpdateAllFreezeStates; 
    }

    private void OnDisable()
    {
        TouchDetection.OnTrainDetection -= UpdateAllFreezeStates;
    }

    private void Awake()
    {
        AllTrainsFreezeController = new(); 
    }

    private void Start()
    {
        Parent = parent; 
    }

    public void StickOnOrOff()
    {
        IsOn = !IsOn; 
        if (IsOn)
        {
            wagonStickPoint.SetPositionAndRotation(locoStickPoint.position, Quaternion.Euler(locoStickPoint.rotation.eulerAngles));
            Parent.SetPositionAndRotation(locoStickPoint.position, Quaternion.Euler(locoStickPoint.rotation.eulerAngles));

            locoStickPoint.SetParent(Parent);
            wagonStickPoint.SetParent(Parent);
        }
        else
        {
            Parent.DetachChildren();

            if (freezeAllOnDetach)
            {
                OnDetachChildren();
            }
        }
    }

    private void UpdateAllFreezeStates(FreezeStateController detectedTrainFreezeController)
    {
        for (int i = 0; i < AllTrainsFreezeController.Count; i++)
        {
            if (AllTrainsFreezeController[i] != detectedTrainFreezeController)
            {
                AllTrainsFreezeController[i].SetFreezeState(freezeAllExceptSelected);
            }
        }
    }
}
