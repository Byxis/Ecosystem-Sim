using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public const float MOVEMENT_SPEED = 4f;
    public const float ROTATION_SPEED = 0.1f;
    public const float SPRINT_MULTIPLIER = 2f;
    public const float MAX_VERTICAL_ANGLE = 89f;


    [Header("Input Actions")]

    [SerializeField] private InputActionReference m_movement;
    [SerializeField] private InputActionReference m_rotation;
    [SerializeField] private InputActionReference m_sprint;
    [SerializeField] private InputActionReference m_switch;
    [SerializeField] private InputActionReference m_pickObject;
    [SerializeField] private InputActionReference m_allowFollowRotation;
    

    [Header("Camera Boundaries")]

    [SerializeField] private Vector3 m_minCorner;
    [SerializeField] private Vector3 m_maxCorner;

    
    // Default camera positions and rotations

    private Vector3 m_defaultGlobalPosition = new Vector3(0, 4, -8);
    private Quaternion m_defaultGlobalRotation = Quaternion.Euler(30, 0, 0);

    private Vector3 m_defaultFollowCamPosition = new Vector3(0, 10, 0);
    private Quaternion m_defaultFollowCamRotation = Quaternion.Euler(90, 0, 0);

    private Vector3 m_defaultFreeCamPosition = new Vector3(0, 2, 0);
    private Quaternion m_defaultFreeCamRotation = Quaternion.Euler(0, 0, 0);

    private Vector3 m_defaultControllableCamPosition = new Vector3(0, 1, 0);
    private Quaternion m_defaultControllableCamRotation = Quaternion.Euler(0, 0, 0);


    private GameObject m_target;
    private ICameraState m_cameraState;

    private void Start()
    {
        // Apply the default values
        m_cameraState = new GlobalCamState(this);
        transform.position = m_defaultGlobalPosition;
        transform.rotation = m_defaultGlobalRotation;

        // Enable the input actions
        m_movement.action.Enable();
        m_switch.action.Enable();
        m_switch.action.performed += _ => ChangeCameraState();
    }


    private void Update()
    {
        if(m_cameraState != null)
            m_cameraState.Handle();
    }

    public void ChangeCameraState()
    {
        // Global Camera -> Follow Camera -> Free Camera -> Controllable Camera -> Global Camera
        if (m_cameraState is GlobalCamState)
        {
            m_cameraState = new FollowCamState(this);
            transform.position = m_defaultFollowCamPosition;
            transform.rotation = m_defaultFollowCamRotation;
        }
        else if (m_cameraState is FollowCamState)
        {
            m_cameraState = new FreeCamState(this);
            transform.position = m_defaultFreeCamPosition;
            transform.rotation = m_defaultFreeCamRotation;
        }
        else if (m_cameraState is FreeCamState)
        {
            m_cameraState = new ControllableState(this);
            transform.position = m_defaultControllableCamPosition;
            transform.rotation = m_defaultControllableCamRotation;
        }
        else if (m_cameraState is ControllableState)
        {
            m_cameraState = new GlobalCamState(this);
            transform.position = m_defaultGlobalPosition;
            transform.rotation = m_defaultGlobalRotation;
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
