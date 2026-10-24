using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController characterController;
    private Vector3 verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.Disable();
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }
        if (moveAction == null || moveAction.action == null)
        {
            return;
        }

        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Vector3 cameraForward = Vector3.forward;
        Vector3 cameraRight = Vector3.right;

        if (cameraTransform != null)
        {
            cameraForward = Vector3.ProjectOnPlane(
                cameraTransform.forward,
                Vector3.up
            ).normalized;

            cameraRight = Vector3.ProjectOnPlane(
                cameraTransform.right,
                Vector3.up
            ).normalized;
        }

        Vector3 moveDirection =
            cameraForward * input.y +
            cameraRight * input.x;

        // 대각선 이동 속도가 더 빨라지지 않도록 제한합니다.
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        // 간단한 중력 처리
        if (characterController.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += Physics.gravity.y * Time.deltaTime;

        Vector3 movement =
            moveDirection * moveSpeed +
            verticalVelocity;

        characterController.Move(movement * Time.deltaTime);
    }
}

