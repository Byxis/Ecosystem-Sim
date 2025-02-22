using UnityEngine;

public class FollowCamState : ICameraState
{
    private CameraController m_camController;
    private float m_rotationX;
    private float m_rotationY;
    private float m_distance = 5f;

    public FollowCamState(CameraController controller)
    {
        m_camController = controller;
        m_rotationX = 0;
        m_rotationY = -90;

        // Show the cursor and unlock it
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
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
        GameObject target = m_camController.GetTarget();

        // If the player clicks on a game object, the camera will follow it
        if (m_camController.GetPickObject().action.triggered)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If the ray hits a game object, the camera will follow it
            
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.GetComponent<Animal>() != null)
            {
                target = hit.collider.gameObject;
                m_camController.SetTarget(target);
                m_camController.transform.position = target.transform.position - m_camController.transform.forward * m_distance;
            }
        }
        
        float scroll = m_camController.GetScroll().action.ReadValue<float>();

        m_distance -= scroll * CameraController.SCROLL_SPEED;
        m_distance = Mathf.Clamp(m_distance, CameraController.MIN_DISTANCE, CameraController.MAX_DISTANCE);

        if(!target) 
        {
            if (m_camController.GetAllowFollowRotation().action.IsPressed())
            {
                Vector2 rotationInput = m_camController.GetRotation().action.ReadValue<Vector2>();
                Transform newTransform = m_camController.transform;

                newTransform.position -= new Vector3(rotationInput.x * CameraController.SWIPE_SPEED, 0, rotationInput.y * CameraController.SWIPE_SPEED);

                Vector3 minCorner = m_camController.GetMinCorner();
                Vector3 maxCorner = m_camController.GetMaxCorner();
                
                newTransform.position = new Vector3(
                    Mathf.Clamp(newTransform.position.x, minCorner.x, maxCorner.x),
                    Mathf.Clamp(newTransform.position.y, minCorner.y, maxCorner.y),
                    Mathf.Clamp(newTransform.position.z, minCorner.z, maxCorner.z)
                );

                m_camController.transform.position = newTransform.position;
            }
            return;
        }

        // If the player right clicks, the camera will rotate with the mouse
        if (m_camController.GetAllowFollowRotation().action.IsPressed())
        {
            Vector2 rotationInput = m_camController.GetRotation().action.ReadValue<Vector2>();
            
            m_rotationX += rotationInput.x * CameraController.ORBIT_ROTATION_SPEED;
            m_rotationY += rotationInput.y * CameraController.ORBIT_ROTATION_SPEED;

            m_rotationY = Mathf.Clamp(m_rotationY, -90, -5);

            m_camController.transform.eulerAngles = new Vector3(-m_rotationY, m_rotationX, 0);
        }

        // The position is adjusted to be at a distance from the target
        m_camController.transform.position = target.transform.position - m_camController.transform.forward * m_distance;
    }
}