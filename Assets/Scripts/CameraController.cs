using UnityEngine;
using UnityEngine.InputSystem;

enum CameraState
{
    Freecam,
    Follow,
    Controllable
}

public class CameraController : MonoBehaviour
{
    public static float MOVEMENT_SPEED = 2.5f;
    public static float ROTATION_SPEED = 0.1f;

    [SerializeField] private Vector3 m_minCorner;
    [SerializeField] private Vector3 m_maxCorner;
    [SerializeField] private InputActionReference m_movement;
    [SerializeField] private InputActionReference m_rotation;
    [SerializeField] private InputActionReference m_sprint;
    [SerializeField] private InputActionReference m_switch;
    private CameraState m_state;

    private void Start()
    {
        m_state = CameraState.Freecam;
        m_movement.action.Enable();
        m_switch.action.Enable();
        m_switch.action.performed += _ => ChangeCameraState();
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        switch (m_state)
        {
            case CameraState.Freecam:
                FreeCam();
                break;
            case CameraState.Follow:
                break;
            case CameraState.Controllable:
                break;
        }
    }

    public void ChangeCameraState()
    {
        switch (m_state)
        {
            case CameraState.Freecam:
                m_state = CameraState.Follow;
                break;
            case CameraState.Follow:
                m_state = CameraState.Controllable;
                break;
            case CameraState.Controllable:
                m_state = CameraState.Freecam;
                break;
        }
    }

    private void FreeCam()
    {
        // Movement and rotation
        Vector3 movement = m_movement.action.ReadValue<Vector3>();
        float sprint = m_sprint.action.ReadValue<float>();
        Vector2 rotation = m_rotation.action.ReadValue<Vector2>();

        Transform newTransform = transform;
        Quaternion rotationX = Quaternion.Euler(0, rotation.x * ROTATION_SPEED, 0);
        Quaternion rotationY = Quaternion.Euler(-rotation.y * ROTATION_SPEED, 0, 0);

        newTransform.rotation = rotationX * newTransform.rotation * rotationY;
        newTransform.position += transform.forward * movement.z * Time.deltaTime * (MOVEMENT_SPEED * (1 + sprint));
        newTransform.position += transform.right * movement.x * Time.deltaTime * (MOVEMENT_SPEED * (1 + sprint));
        newTransform.position += Vector3.up * movement.y * Time.deltaTime * (MOVEMENT_SPEED * (1 + sprint));
        
        if (m_minCorner == null || m_maxCorner == null)
        {
            transform.position = newTransform.position;
            transform.rotation = newTransform.rotation;
            return;
        }

        float newX = Mathf.Clamp(newTransform.position.x, m_minCorner.x, m_maxCorner.x);
        float newY = Mathf.Clamp(newTransform.position.y, m_minCorner.y, m_maxCorner.y);
        float newZ = Mathf.Clamp(newTransform.position.z, m_minCorner.z, m_maxCorner.z);

        newTransform.position = new Vector3(newX, newY, newZ);

        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
    }
}
