using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class FirstPersonCamera : MonoBehaviour, CharacterActions.ICameraActions
{
    [SerializeField] private float lookSensitivity = 0.1f;
    [SerializeField] private float minVerticalAngle = -80f;
    [SerializeField] private float maxVerticalAngle = 80f;
    [SerializeField] private Transform playerTransform;

    private Vector2 lookInput;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private CharacterActions inputActions;
    private CinemachineVirtualCamera virtualCamera;
    private bool isActive;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        inputActions = new CharacterActions();
        inputActions.Camera.SetCallbacks(this);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        virtualCamera.Priority = 0;
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

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        if (isActive)
        {
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
            playerTransform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
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

    public void SetActive(bool active)
    {
        if (active)
        {
            ThirdPersonCamera thirdPersonCamera = FindObjectOfType<ThirdPersonCamera>();
            if (thirdPersonCamera != null)
            {
                xRotation = thirdPersonCamera.GetCameraPitch();
                yRotation = thirdPersonCamera.GetCameraYaw();
            }
        }
        virtualCamera.Priority = active ? 10 : 0;
        isActive = active;
    }
}