using UnityEngine;

public class ControllableState : ICameraState
{
    public ControllableState()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Handle(CameraController controller)
    {
        //TODO: Implement the controllable camera state
    }
}
