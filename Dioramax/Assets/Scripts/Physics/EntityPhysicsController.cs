using System.Collections.Generic;
using UnityEngine;

// reference to all physicable objects
public class EntityPhysicsController : MonoBehaviour
{
    [SerializeField] private Transform mainCamTransform; 
    public static EntityPhysicsController Instance; 

    [SerializeField, Range(0.05f, 2f)] private float gravityForceMultiplier = 1f;
    private List<Rigidbody> entityRbList = new List<Rigidbody>();

    private const float GRAVITY_FORCE = 9.81f; 

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance); 
        }
        Instance = this; 
    }

    void FixedUpdate()
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
        Debug.DrawRay(transform.position, mainCamTransform.up * -30, Color.red);

        if (entityRbList.Count > 0)
        {
            foreach (Rigidbody rb in entityRbList)
            {
                try
                {
                    rb.AddForce(mainCamTransform.up * -GRAVITY_FORCE * gravityForceMultiplier, ForceMode.Acceleration);
                }
                catch { }
            } 
        }
    }
}
