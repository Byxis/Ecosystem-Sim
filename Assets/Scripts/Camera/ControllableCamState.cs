using UnityEngine;

public class ControllableState : ICameraState
{
    private CameraController m_camController;
    public ControllableState(CameraController controller)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        m_camController = controller;
        if (!m_camController.gameObject.GetComponent<Rigidbody>())
        {
            m_camController.gameObject.AddComponent<Rigidbody>();
        }
        m_camController.gameObject.GetComponent<Rigidbody>().useGravity = true;
        m_camController.gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
    public void Handle()
    {
        //TODO: Implement the controllable camera state
    }
}
