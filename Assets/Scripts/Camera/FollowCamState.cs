using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCamState : ICameraState
{

    private Vector3 m_defaultPosition;
    private Quaternion m_defaultRotation;
    public float rotationSpeed = 50f;
    public float distance = 5f;

    public FollowCamState(CameraController controller)
    {
        m_defaultPosition = new Vector3(0, 5, 0);
        m_defaultRotation = Quaternion.Euler(90, 0, 0);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        controller.transform.position = m_defaultPosition;
        controller.transform.rotation = m_defaultRotation;
    }
    public void Handle(CameraController controller)
    {
        GameObject target = controller.GetTarget();

        // If the player clicks on a game object, the camera will follow it
        if (controller.GetPickObject().action.triggered)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                controller.SetTarget(hit.collider.gameObject);
                target = controller.GetTarget();
                controller.transform.position = new Vector3
                (
                    target.transform.position.x, 
                    controller.transform.position.y, 
                    target.transform.position.z
                );
            }
        }

        if (controller.GetAllowFollowRotation().action.IsPressed() && target != null)
        {
            Vector2 rotationInput = controller.GetRotation().action.ReadValue<Vector2>();

            Transform newTransform = controller.transform;

            newTransform.RotateAround(target.transform.position, Vector3.up, rotationInput.x * rotationSpeed * Time.deltaTime);
            newTransform.RotateAround(target.transform.position, controller.transform.right, rotationInput.y * rotationSpeed * Time.deltaTime);

            float rotationX = Mathf.Clamp(newTransform.rotation.eulerAngles.x, 10, 80);
            newTransform.rotation = Quaternion.Euler(rotationX, newTransform.rotation.eulerAngles.y, 0);
            
            newTransform.LookAt(target.transform);

            // Prevent the camera from being reversed
            rotationX = Mathf.Clamp(newTransform.rotation.eulerAngles.x, 10, 80);
            newTransform.rotation = Quaternion.Euler(rotationX, newTransform.rotation.eulerAngles.y, 0);

            float posY = Mathf.Clamp(newTransform.position.y, 1f, 4.5f);
            newTransform.position = new Vector3(newTransform.position.x, posY, newTransform.position.z);



            controller.transform.position = newTransform.position;
            controller.transform.rotation = newTransform.rotation;

    /*
            // Appliquer les rotations à la caméra de manière incrémentale
            controller.transform.rotation = yaw + controller.transform.rotation;
            controller.transform.rotation = pitch * controller.transform.rotation;

            // Maintenir la distance fixe
            Vector3 directionToTarget = (controller.transform.position - target.transform.position).normalized;
            controller.transform.position = target.transform.position - directionToTarget * distance;

            // Assurez-vous que la caméra regarde toujours la cible
            controller.transform.LookAt(target.transform); */
        }
    }
}