using System.Collections.Generic;
using UnityEngine;

// reference to all physicable objects
public class EntityPhysicsController : MonoBehaviour
{
    [SerializeField, Range(5, 20)] private float gravityForce = 9.81f;
    private List<SimulateEntityPhysics> entityPhysicsList = new List<SimulateEntityPhysics>();
    private List<Rigidbody> entityRbList = new List<Rigidbody>();

    private void OnEnable()
    {
        SimulateEntityPhysics.OnInitializePhysicsEntity += AddToList;
    }

    private void OnDisable()
    {
        SimulateEntityPhysics.OnInitializePhysicsEntity -= AddToList;
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

    private void UpdateEntities()
    {
        foreach (Rigidbody rb in entityRbList)
        {
            try
            {
                rb.AddForce(transform.InverseTransformDirection(Camera.main.transform.up) * -gravityForce, ForceMode.Acceleration);
            }
            catch { } // PLACEHOLDER. Some rats are destroyed, so it returns null. Change this
        } 
    }
}
