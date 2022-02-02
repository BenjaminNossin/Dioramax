using UnityEngine;

public class InputsController : MonoBehaviour
{
    [SerializeField] private LayerMask cubeMask;
    [SerializeField] private Camera camera;
    [SerializeField] private PlaceholderFeedback placeholderFeedback;

    private Vector3 origin;
    private Vector3 end;
    private Touch touch; 
    // Input.touches to track multiple fingers -> zoom
    
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            origin = transform.position; 
            end = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 30f));
            Debug.DrawRay(origin, (end-origin) * 100f, Color.red, 0.5f); 
            
            if (Physics.Raycast(origin, (end-origin), out RaycastHit hitInfo, 100f, cubeMask))
            {
                Debug.DrawRay(origin, (end-origin), Color.green, 0.5f); 
                placeholderFeedback.ChangeColor();
            }
        }
    }
}
