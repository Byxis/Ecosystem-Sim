using UnityEngine;

public class FreeCamState : ICameraState
{
    private CameraController m_camController;

    public FreeCamState(CameraController controller)
    {
        m_camController = controller;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (m_camController.gameObject.GetComponent<Rigidbody>())
        {
            m_camController.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            m_camController.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    public void Handle()
    {
        // Get the input values
        Vector3 movement = m_camController.GetMovement().action.ReadValue<Vector3>();
        float sprint = m_camController.GetSprint().action.ReadValue<float>();
        Vector2 rotation = m_camController.GetRotation().action.ReadValue<Vector2>();

        // Calculate the new rotation
        Transform newTransform = m_camController.transform;
        Quaternion rotationX = Quaternion.Euler(0, rotation.x * CameraController.ROTATION_SPEED, 0);
        Quaternion rotationY = Quaternion.Euler(-rotation.y * CameraController.ROTATION_SPEED, 0, 0);

        // Apply the new rotation and movement
        newTransform.rotation = rotationX * newTransform.rotation * rotationY;
        newTransform.position += newTransform.forward * movement.z * Time.deltaTime * (CameraController.MOVEMENT_SPEED * (1 + sprint));
        newTransform.position += newTransform.right * movement.x * Time.deltaTime * (CameraController.MOVEMENT_SPEED * (1 + sprint));
        newTransform.position += Vector3.up * movement.y * Time.deltaTime * (CameraController.MOVEMENT_SPEED * (1 + sprint));

        // If the corners are not set, just move the camera
        if (m_camController.GetMinCorner() == null || m_camController.GetMaxCorner() == null)
        {
            m_camController.transform.position = newTransform.position;
            m_camController.transform.rotation = newTransform.rotation;
            return;
        }

        // If the camera is in a bounded area, clamp the position
        float newX = Mathf.Clamp(newTransform.position.x, m_camController.GetMinCorner().x, m_camController.GetMaxCorner().x);
        float newY = Mathf.Clamp(newTransform.position.y, m_camController.GetMinCorner().y, m_camController.GetMaxCorner().y);
        float newZ = Mathf.Clamp(newTransform.position.z, m_camController.GetMinCorner().z, m_camController.GetMaxCorner().z);

        newTransform.position = new Vector3(newX, newY, newZ);

        // Apply the new transform
        m_camController.transform.position = newTransform.position;
        m_camController.transform.rotation = newTransform.rotation;
    }
}