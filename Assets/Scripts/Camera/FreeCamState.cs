using UnityEngine;

public class FreeCamState : ICameraState
{
    private CameraController m_camController;
    private float m_currentVerticalRotation = 0f;
    private float m_currentHorizontalRotation = 0f;

    public FreeCamState(CameraController controller)
    {
        m_camController = controller;

        // Hide the cursor and lock it to the center of the screen
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Remove the rigidbody and box collider from the camera if they exist
        Rigidbody rb = m_camController.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Object.Destroy(rb);
        }
        BoxCollider collider = m_camController.gameObject.GetComponent<BoxCollider>();
        if (collider != null)
        {
            Object.Destroy(collider);
        }
    }

    public void Handle()
    {
        // Get and process input
        Vector3 movement = m_camController.GetMovement().action.ReadValue<Vector3>();
        Vector2 rotation = m_camController.GetRotation().action.ReadValue<Vector2>();
        float sprint = m_camController.GetSprint().action.ReadValue<float>();

        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        Transform newTransform = m_camController.transform;

        // Handle rotation
        m_currentHorizontalRotation += rotation.x * CameraController.ROTATION_SPEED;
        m_currentVerticalRotation -= rotation.y * CameraController.ROTATION_SPEED;
        m_currentVerticalRotation = Mathf.Clamp(m_currentVerticalRotation, -CameraController.MAX_VERTICAL_ANGLE, CameraController.MAX_VERTICAL_ANGLE);
        newTransform.rotation = Quaternion.Euler(m_currentVerticalRotation, m_currentHorizontalRotation, 0);

        // Handle movement
        Vector3 forward = newTransform.forward;
        Vector3 right = newTransform.right;
        
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        float currentSpeed = CameraController.MOVEMENT_SPEED * (1 + sprint * CameraController.SPRINT_MULTIPLIER);
        newTransform.position += forward * movement.z * Time.deltaTime * currentSpeed;
        newTransform.position += right * movement.x * Time.deltaTime * currentSpeed;
        newTransform.position += Vector3.up * movement.y * Time.deltaTime * currentSpeed;

        // Handle boundaries
        if (m_camController.GetMinCorner() != null && m_camController.GetMaxCorner() != null)
        {
            Vector3 minCorner = m_camController.GetMinCorner();
            Vector3 maxCorner = m_camController.GetMaxCorner();
            
            newTransform.position = new Vector3(
                Mathf.Clamp(newTransform.position.x, minCorner.x, maxCorner.x),
                Mathf.Clamp(newTransform.position.y, minCorner.y, maxCorner.y),
                Mathf.Clamp(newTransform.position.z, minCorner.z, maxCorner.z)
            );
        }

        // Apply final transform
        m_camController.transform.position = newTransform.position;
        m_camController.transform.rotation = newTransform.rotation;
    }
}