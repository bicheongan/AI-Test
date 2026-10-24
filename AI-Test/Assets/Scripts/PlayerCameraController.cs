using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Distance")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float targetHeight = 1.5f;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 70f;

    private float yaw;
    private float pitch = 15f;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError(
                "PlayerCameraController: Target가 연결되지 않았습니다.",
                this
            );

            enabled = false;
            return;
        }

        yaw = target.eulerAngles.y;

        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "PlayerCameraController: GameManager가 없습니다.",
                this
            );

            enabled = false;
            return;
        }

        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

        HandleGameStateChanged(GameManager.Instance.CurrentState);
    }

    private void LateUpdate()
    {
        if (target == null || GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        ReadMouseInput();
        UpdateCameraPosition();
    }
    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Playing)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    private void ReadMouseInput()
    {
        if (Mouse.current == null)
        {
            return;
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * mouseSensitivity;
        pitch -= mouseDelta.y * mouseSensitivity;

        // 카메라가 지나치게 위나 아래를 향하지 않도록 제한합니다.
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void UpdateCameraPosition()
    {
        // 좌우 회전과 상하 회전을 합쳐 카메라 회전을 만듭니다.
        Quaternion cameraRotation = Quaternion.Euler(
            pitch,
            yaw,
            0f
        );

        // 플레이어보다 약간 높은 지점을 바라보도록 합니다.
        Vector3 targetPosition =
            target.position + Vector3.up * targetHeight;

        // 회전 방향의 반대쪽에 카메라를 배치합니다.
        Vector3 cameraPosition =
            targetPosition +
            cameraRotation * new Vector3(0f, 0f, -distance);

        transform.SetPositionAndRotation(
            cameraPosition,
            cameraRotation
        );
    }

    private void OnDisable()
    {
        // 스크립트가 비활성화되면 커서를 다시 표시합니다.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }

    }
}

