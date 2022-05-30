using UnityEngine;
using System.Collections.Generic; 

public class TrainsManager : MonoBehaviour
{
    [SerializeField] private Transform parentOnStickTrain;

    [Space, SerializeField] private Transform locoStickPoint;
    [SerializeField] private Transform wagonStickPoint;

    [Space, SerializeField] private bool freezeAllExceptSelected = true;
    public static List<FreezeStateController> AllTrainsFreezeController { get; private set; }
    [SerializeField] private FreezeStateController[] linkableFreezeControllers;  

    public static bool IsOn;
    public static Transform ParentOnStickTrain;

    [Space] public bool freezeAllOnDetach = true;  
    public static System.Action OnDetachChildren { get; set; }

    [SerializeField] private GameObject locomotive; 

    private void OnEnable()
    {
        TouchDetection.OnTrainDetection += UpdateAllFreezeStates;
        StopTrainOnTriggerEnter.OnFinishingObstacle1 += ActivateLocomotive;

    }

    private void OnDisable()
    {
        TouchDetection.OnTrainDetection -= UpdateAllFreezeStates;
        StopTrainOnTriggerEnter.OnFinishingObstacle1 -= ActivateLocomotive;

    }

    private void ActivateLocomotive()
    {
        locomotive.SetActive(true);
    }

    private void Awake()
    {
        AllTrainsFreezeController = new(); 
    }

    private void Start()
    {
        ParentOnStickTrain = parentOnStickTrain;
    }

    public void StickOnOrOff()
    {
        IsOn = !IsOn; 
        if (IsOn)
        {
            wagonStickPoint.SetPositionAndRotation(locoStickPoint.position, Quaternion.Euler(locoStickPoint.rotation.eulerAngles));
            ParentOnStickTrain.SetPositionAndRotation(locoStickPoint.position, Quaternion.Euler(locoStickPoint.rotation.eulerAngles));

            locoStickPoint.SetParent(ParentOnStickTrain);
            wagonStickPoint.SetParent(ParentOnStickTrain);
        }
        else
        {
            ParentOnStickTrain.DetachChildren();

            if (freezeAllOnDetach)
            {
                OnDetachChildren();
            }
        }

        for (int i = 0; i < linkableFreezeControllers.Length; i++)
        {
            linkableFreezeControllers[i].IsLinked = IsOn;
        }
    }

    private void UpdateAllFreezeStates(FreezeStateController detectedTrainFreezeController)
    {
        for (int i = 0; i < AllTrainsFreezeController.Count; i++)
        {
            if (AllTrainsFreezeController[i] != detectedTrainFreezeController && !AllTrainsFreezeController[i].IsLinked)
            {
                AllTrainsFreezeController[i].SetFreezeState(freezeAllExceptSelected);
            }
        }
    }
}
