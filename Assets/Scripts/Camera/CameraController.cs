using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public const float MOVEMENT_SPEED = 2.5f;
    public const float ROTATION_SPEED = 0.1f;

    [SerializeField] private InputActionReference m_movement;
    [SerializeField] private InputActionReference m_rotation;
    [SerializeField] private InputActionReference m_sprint;
    [SerializeField] private InputActionReference m_switch;
    [SerializeField] private InputActionReference m_pickObject;
    [SerializeField] private InputActionReference m_allowFollowRotation;

    [SerializeField] private Vector3 m_minCorner;
    [SerializeField] private Vector3 m_maxCorner;
    private GameObject m_target;
    private Transform m_oldFreeCamTransform;
    private ICameraState m_cameraState;

    private void Start()
    {
        // Apply the default values
        m_cameraState = new FreeCamState();

        // Enable the input actions
        m_movement.action.Enable();
        m_switch.action.Enable();
        m_switch.action.performed += _ => ChangeCameraState();
    }


    private void Update()
    {
        m_cameraState.Handle(this);
    }

    public void ChangeCameraState()
    {
        if (m_cameraState is FreeCamState)
        {
            m_cameraState = new FollowCamState(this);
        }
        else if (m_cameraState is FollowCamState)
        {
            m_cameraState = new ControllableState();
        }
        else if (m_cameraState is ControllableState)
        {
            m_cameraState = new FreeCamState();
        }
    }

    public InputActionReference GetMovement()
    {
        return m_movement;
    }

    public InputActionReference GetSprint()
    {
        return m_sprint;
    }

    public InputActionReference GetRotation()
    {
        return m_rotation;
    }

    public InputActionReference GetPickObject()
    {
        return m_pickObject;
    }

    public InputActionReference GetAllowFollowRotation()
    {
        return m_allowFollowRotation;
    }

    public Vector3 GetMinCorner()
    {
        return m_minCorner;
    }

    public Vector3 GetMaxCorner()
    {
        return m_maxCorner;
    }

    public GameObject GetTarget()
    {
        return m_target;
    }

    public void SetTarget(GameObject target)
    {
        m_target = target;
    }
}
