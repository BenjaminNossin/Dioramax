using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeObjectStateOnCollision : MonoBehaviour
{
    [SerializeField] private bool onlyChangeOnce;
    [SerializeField, Range(1, 5)] private int hitsBeforeStateChange = 1;
    [SerializeField, Range(0f, 5f)] private float minSpeedToValidateHit;
    [SerializeField] private LayerMask collisionMask;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider; 
    private int hitCount;
    private bool isActive; 

    public static System.Action OnHit;
    private EntityPathNavigation collisionEntity; 

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>(); 
    }

    /* private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2f, other.gameObject.layer) == collisionMask)
        {
            if (!collisionEntity)
            {
                collisionEntity = other.transform.GetComponent<EntityPathNavigation>();
            }

            if (collisionEntity.MoveSpeed < minSpeedToValidateHit)
            {
                // collisionEntity.GoToPreviousPoint();
            }
            else
            {
                hitCount++;

                if (hitCount < hitsBeforeStateChange)
                {
                    // collisionEntity.GoToPreviousPoint();
                    StartCoroutine(nameof(BlinkOnce));
                }
                else
                {
                    isActive = !isActive;
                    meshRenderer.material.color = isActive ? Color.blue : Color.red;
                    hitCount = 0; // reset 

                    boxCollider.enabled = false;
                    meshRenderer.enabled = false;

                    if (!onlyChangeOnce)
                    {
                        StartCoroutine(nameof(ResetPhysicsAndVisuals));
                    }
                }
            }
        }
    } */

    private IEnumerator BlinkOnce()
    {
        meshRenderer.material.color = Color.blue;
        yield return new WaitForSeconds(0.2f);

        meshRenderer.material.color = Color.red; 
    }

    private IEnumerator ResetPhysicsAndVisuals()
    {
        yield return new WaitForSeconds(1f);

        meshRenderer.material.color = Color.red;
        boxCollider.enabled = true;
        meshRenderer.enabled = true;
    }
}
