using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCamState : ICameraState
{

    private Vector3 m_defaultPosition;
    private Quaternion m_defaultRotation;
    private CameraController m_camController;
    private float m_rotationX = 0;
    private float m_rotationY = -90;
    public float rotationSpeed = 0.5f;
    public float distance = 5f;

    public FollowCamState(CameraController controller)
    {
        m_camController = controller;
        m_defaultPosition = new Vector3(0, 5, 0);
        m_defaultRotation = Quaternion.Euler(90, 0, 0);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        m_camController.transform.position = m_defaultPosition;
        m_camController.transform.rotation = m_defaultRotation;
        
        if (m_camController.gameObject.GetComponent<Rigidbody>())
        {
            m_camController.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
    }
    public void Handle()
    {
        GameObject target = m_camController.GetTarget();

        // If the player clicks on a game object, the camera will follow it
        if (m_camController.GetPickObject().action.triggered)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                m_camController.SetTarget(hit.collider.gameObject);
                target = m_camController.GetTarget();
                m_camController.transform.position = target.transform.position - m_camController.transform.forward * distance;
            }
        }

        if (m_camController.GetAllowFollowRotation().action.IsPressed() && target != null)
        {
            Vector2 rotationInput = m_camController.GetRotation().action.ReadValue<Vector2>();
            
            m_rotationX += rotationInput.x * rotationSpeed;
            m_rotationY += rotationInput.y * rotationSpeed;

            m_rotationY = Mathf.Clamp(m_rotationY, -90, -5);

            m_camController.transform.eulerAngles = new Vector3(-m_rotationY, m_rotationX, 0);
            m_camController.transform.position = target.transform.position - m_camController.transform.forward * distance;
        }
    }
}