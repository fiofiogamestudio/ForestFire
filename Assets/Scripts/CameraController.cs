using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotateSpeed = 5.0f; // Rotate speed
    public float zoomSpeed = 10.0f; // Zoom speed

    private Vector3 center = new Vector3(512f, 0, 512f);

    private void Update()
    {
        if (Input.GetMouseButton(2))
        {
            // Get the new rotation
            float rotX = Input.GetAxis("Mouse X") * rotateSpeed * Mathf.Deg2Rad;
            float rotY = Input.GetAxis("Mouse Y") * rotateSpeed * Mathf.Deg2Rad;

            // Rotate the camera around the target position
            transform.RotateAround(center, Vector3.up, -rotX);
            transform.RotateAround(center, transform.right, rotY);
        }

        // Get the mouse wheel input
        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        // Move the camera along its forward vector
        transform.position += transform.forward * zoom;
    }
}
