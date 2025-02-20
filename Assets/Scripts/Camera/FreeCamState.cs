using UnityEngine;

public class FreeCamState : ICameraState
{
    public FreeCamState()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Handle(CameraController controller)
    {
        // Get the input values
        Vector3 movement = controller.GetMovement().action.ReadValue<Vector3>();
        float sprint = controller.GetSprint().action.ReadValue<float>();
        Vector2 rotation = controller.GetRotation().action.ReadValue<Vector2>();

        // Calculate the new rotation
        Transform newTransform = controller.transform;
        Quaternion rotationX = Quaternion.Euler(0, rotation.x * CameraController.ROTATION_SPEED, 0);
        Quaternion rotationY = Quaternion.Euler(-rotation.y * CameraController.ROTATION_SPEED, 0, 0);

        // Apply the new rotation and movement
        newTransform.rotation = rotationX * newTransform.rotation * rotationY;
        newTransform.position += newTransform.forward * movement.z * Time.deltaTime * (CameraController.MOVEMENT_SPEED * (1 + sprint));
        newTransform.position += newTransform.right * movement.x * Time.deltaTime * (CameraController.MOVEMENT_SPEED * (1 + sprint));
        newTransform.position += Vector3.up * movement.y * Time.deltaTime * (CameraController.MOVEMENT_SPEED * (1 + sprint));

        // If the corners are not set, just move the camera
        if (controller.GetMinCorner() == null || controller.GetMaxCorner() == null)
        {
            controller.transform.position = newTransform.position;
            controller.transform.rotation = newTransform.rotation;
            return;
        }

        // If the camera is in a bounded area, clamp the position
        float newX = Mathf.Clamp(newTransform.position.x, controller.GetMinCorner().x, controller.GetMaxCorner().x);
        float newY = Mathf.Clamp(newTransform.position.y, controller.GetMinCorner().y, controller.GetMaxCorner().y);
        float newZ = Mathf.Clamp(newTransform.position.z, controller.GetMinCorner().z, controller.GetMaxCorner().z);

        newTransform.position = new Vector3(newX, newY, newZ);

        // Apply the new transform
        controller.transform.position = newTransform.position;
        controller.transform.rotation = newTransform.rotation;
    }
}