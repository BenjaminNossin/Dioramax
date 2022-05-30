using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_PathUpdater : MonoBehaviour
{
    [SerializeField] private LayerMask fireMask;
    [SerializeField] private PathNode previousNode;
    [SerializeField] private PathNode nextNode;

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == fireMask)
        {
            // destroy anim
            GameLogger.Log("playing destroy anim");
            StartCoroutine(BridgePath());
        }
    }

    private readonly WaitForSeconds WFS = new(1f); // DEBUG. Call this from end of destroy animation
    private IEnumerator BridgePath()
    {
        yield return WFS;
        previousNode.SetNextNode(nextNode); 
    }
}
