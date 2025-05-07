using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ThirdPersonCamera : MonoBehaviour, CharacterActions.ICameraActions
{
    [SerializeField] private float lookSensitivity = 0.1f;
    [SerializeField] private float minVerticalAngle = -80f;
    [SerializeField] private float maxVerticalAngle = 80f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);

    private CinemachineVirtualCamera virtualCamera;
    private Vector2 lookInput;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private CharacterActions inputActions;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        inputActions = new CharacterActions();
        inputActions.Camera.SetCallbacks(this);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        inputActions.Camera.Enable();
    }

    private void OnDisable()
    {
        inputActions.Camera.Disable();
    }

    private void LateUpdate()
    {
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        if (playerTransform != null)
        {
            transform.position = playerTransform.position + transform.rotation * offset;
            playerTransform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public float GetCameraYaw()
    {
        return yRotation;
    }

    public float GetCameraPitch()
    {
        return xRotation;
    }

    public void SetCameraControlsEnabled(bool enabled)
    {
        if (enabled)
        {
            inputActions.Camera.Enable();
        }
        else
        {
            inputActions.Camera.Disable();
            lookInput = Vector2.zero;
        }
    }
}