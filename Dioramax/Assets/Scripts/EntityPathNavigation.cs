using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPathNavigation : MonoBehaviour
{
    [SerializeField] private PathController entityPathController;
    [SerializeField, Range(0.25f, 5f)] private float moveSpeed = 1f; // on the object, not the path
    [SerializeField] private bool loopPath; 

    public int DestPoint { get; private set; }
    private float remainingDistance;
    private Vector3 destinationDirection;

    void Start()
    {
        transform.position = entityPathController.Points[0].position;
        GotoNextPoint();
    }


    void Update()
    {
        if (Vector3.Distance(transform.position, entityPathController.Points[^1].position) < 0.03f &&
            DestPoint == entityPathController.Points.Length-1 && !loopPath) return; 

        remainingDistance = Vector3.Distance(transform.position, entityPathController.Points[DestPoint].position);
        if (remainingDistance < 0.03f)
        {
            GotoNextPoint();
        }
        else
        {
            transform.position += destinationDirection * Time.deltaTime * moveSpeed;
        }
    }

    // go if more than 90°
    private void GotoNextPoint()
    {
        if (entityPathController.Points.Length == 0)
            return;

        DestPoint = (DestPoint + 1) % entityPathController.Points.Length;
        destinationDirection = (entityPathController.Points[DestPoint].position - transform.position).normalized;
    }
}
