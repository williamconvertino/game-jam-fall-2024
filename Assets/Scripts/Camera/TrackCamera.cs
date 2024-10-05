using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Ship target;        // Target to follow
    public bool trackPosition = true;
    public bool trackRotation = false; // Whether to lock the camera's rotation
    public float zoomScale = 1.0f;
    private float zoomFactor;    // How zoomed in the camera is (higher is more zoomed out)

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("No camera component found on this GameObject.");
        }
        
    }
    
    float GetZoomFactor(int shipSize)
    {
        if (shipSize < 9) return 10.0f;
        if (shipSize < 16) return 12.0f;
        return 15.0f;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            zoomFactor = GetZoomFactor(target.ShipSize) * zoomScale;
            
            Vector3 newPosition = transform.position;

            // Follow the target's position based on the lock settings
            if (trackPosition)
            {
                newPosition.x = target.transform.position.x;
                newPosition.y = target.transform.position.y;
            }

            // Assign the new position to the camera
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

            // Lock or update rotation
            if (trackRotation)
            {
                transform.rotation = target.transform.rotation;
            }

            // Zoom handling
            cam.orthographicSize = zoomFactor;
        }
        else
        {
            Debug.LogWarning("No target set for the camera to follow.");
        }
    }
}