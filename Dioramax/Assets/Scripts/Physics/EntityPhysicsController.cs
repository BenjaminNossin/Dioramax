using System.Collections.Generic;
using UnityEngine;

// reference to all physicable objects
public class EntityPhysicsController : MonoBehaviour
{
    public static EntityPhysicsController Instance; 

    [SerializeField, Range(5, 20)] private float gravityForce = 9.81f;
    private List<Rigidbody> entityRbList = new List<Rigidbody>();

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance); 
        }
        Instance = this; 
    }

    void Update()
    {
        UpdateEntities(); 
    }

    public void AddRbToList(Rigidbody rb)
    {
        entityRbList.Add(rb);
    }

    public void RemoveRbFromList(Rigidbody rb)
    {
        entityRbList.Remove(rb); 
    }

    private void UpdateEntities()
    {
        if (entityRbList.Count > 0)
        {
            foreach (Rigidbody rb in entityRbList)
            {
                try
                {
                    rb.AddForce(transform.InverseTransformDirection(Camera.main.transform.up) * -gravityForce, ForceMode.Acceleration);
                }
                catch { }
            } 
        }
    }
}
