using System.Collections.Generic;
using UnityEngine;

// reference to all physicable objects
public class EntityPhysicsController : MonoBehaviour
{
    [SerializeField, Range(5, 20)] private float gravityForce = 9.81f; 
    private List<SimulateEntityPhysics> entityPhysicsList = new List<SimulateEntityPhysics>();
    private List<Rigidbody> entityRbList = new List<Rigidbody>();

    private Vector3 gravityDirection = new Vector3(0f, -1f, 0f);  

    private void OnEnable()
    {
        SimulateEntityPhysics.OnInitializePhysicsEntity += AddToList;
        SimulateEntityPhysics.OnCameraZRotationUpdate += SetNewGravityValue; 
    }

    private void OnDisable()
    {
        SimulateEntityPhysics.OnInitializePhysicsEntity -= AddToList;
        SimulateEntityPhysics.OnCameraZRotationUpdate -= SetNewGravityValue;
    }

    void Update()
    {
        UpdateEntities(); 
    }

    private void AddToList(SimulateEntityPhysics entityToAdd, Rigidbody rb)
    {
        entityPhysicsList.Add(entityToAdd);
        entityRbList.Add(rb);
    }

    private void SetNewGravityValue(Vector3 newGravityDirection)
    {
        gravityDirection = new Vector3(Mathf.Sin(90f), -Mathf.Cos(90f));
    }

    private void UpdateEntities()
    {
        // angle -> direction
        // CHECK 2D Rotation Matrix
        gravityDirection = new Vector3(Mathf.Cos(90f), Mathf.Sin(90f));
        Debug.Log("gravity direction is " + gravityDirection);

        foreach (Rigidbody rb in entityRbList)
        {
            // always 9,81
            // gravityForce
            rb.AddForce(gravityDirection.normalized * gravityForce, ForceMode.Acceleration); 
        }
    }
}
