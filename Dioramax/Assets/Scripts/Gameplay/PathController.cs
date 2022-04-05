using System;
using UnityEngine;

public class PathController : MonoBehaviour
{
    public Transform[] Points { get; private set; }

    public bool IsInactive { get; set; }

    private void OnDrawGizmos()
    {
        SetPoints(); // I should do this ONLY if there is a change of childCount

        if (Points.Length != 0)
        {
            IsInactive = false;
            for (var i = 0; i < Points.Length; i++)
            {
                if (i > 0)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(Points[i - 1].position, Points[i].position);
                }

                Gizmos.color = i == Points.Length - 1 ? Color.red : (i == 0 ? Color.white : Color.yellow);
                Gizmos.DrawWireSphere(Points[i].position, 0.25f);
            }
        }
    }

    private void Awake()
    {
        SetPoints();
    }

    private void SetPoints()
    {
        if (IsInactive) return;

        Points = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            Points[i] = transform.GetChild(i);
        }
    }
}
