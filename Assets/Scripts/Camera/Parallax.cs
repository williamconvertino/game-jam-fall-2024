using UnityEngine;

public class Parallax : MonoBehaviour
{
    // Variables for setting the parallax effect speed.
    [SerializeField] private float parallaxEffectMultiplier = 0.5f;
    private Transform cameraTransform;
    private Vector3 previousCameraPosition;

    private void Start()
    {
        // Get the camera's transform and its initial position.
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
    }

    private void Update()
    {
        // Calculate the movement of the camera between frames.
        Vector3 deltaMovement = cameraTransform.position - previousCameraPosition;
        
        // Move the background based on the camera's movement and parallax multiplier.
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier, deltaMovement.y * parallaxEffectMultiplier, 0);
        
        // Update the previous camera position for the next frame.
        previousCameraPosition = cameraTransform.position;
    }
}