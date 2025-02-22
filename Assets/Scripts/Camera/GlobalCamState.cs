using UnityEngine;

public class GlobalCamState : ICameraState
{
    public GlobalCamState(CameraController controller)
    {
        // Hide the cursor and lock it to the center of the screen
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Remove the rigidbody and box collider from the camera if they exist
        Rigidbody rb = controller.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Object.Destroy(rb);
        }
        BoxCollider collider = controller.gameObject.GetComponent<BoxCollider>();
        if (collider != null)
        {
            Object.Destroy(collider);
        }
    }

    public void Handle()
    {
        // Nothing to do here, the camera is static
        return;
    }
}
