using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float sprintSpeedBonus = 2f;

    [Header("Stamina")]
    [SerializeField, Min(1f)] private float maxStamina = 100f;
    [SerializeField, Min(0f)] private float sprintStaminaCost = 10f;
    [SerializeField, Min(0f)] private float staminaRecovery = 3f;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController characterController;
    private Vector3 verticalVelocity;

    [SerializeField] private float currentStamina;
    private bool isSprinting;

    // 몬스터 최고 속도 계산에 사용하는 기본 걷기 속도
    public float BaseMoveSpeed => moveSpeed;

    // 나중에 스태미나 UI에서 사용할 값
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public bool IsSprinting => isSprinting;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        currentStamina = maxStamina;

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

        if (sprintAction != null && sprintAction.action != null)
        {
            sprintAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.Disable();
        }

        if (sprintAction != null && sprintAction.action != null)
        {
            sprintAction.action.Disable();
        }

        isSprinting = false;
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.CurrentState != GameState.Playing)
        {
            isSprinting = false;
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

        // 대각선 이동 속도 제한
        moveDirection = Vector3.ClampMagnitude(
            moveDirection,
            1f
        );

        bool isMoving = moveDirection.sqrMagnitude > 0.001f;

        bool shiftPressed =
            sprintAction != null &&
            sprintAction.action != null &&
            sprintAction.action.IsPressed();

        // 실제로 이동하면서 Shift를 누를 때만 달리기
        isSprinting =
            shiftPressed &&
            isMoving &&
            currentStamina > 0f &&
            sprintSpeedBonus > 0f;

        if (isSprinting)
        {
            currentStamina -= sprintStaminaCost * Time.deltaTime;

            currentStamina = Mathf.Max(
                0f,
                currentStamina
            );
        }
        else
        {
            currentStamina += staminaRecovery * Time.deltaTime;

            currentStamina = Mathf.Min(
                maxStamina,
                currentStamina
            );
        }

        float currentMoveSpeed = moveSpeed;

        if (isSprinting)
        {
            currentMoveSpeed += sprintSpeedBonus;
        }

        // 기존 중력 처리 유지
        if (characterController.isGrounded &&
            verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y +=
            Physics.gravity.y * Time.deltaTime;

        Vector3 movement =
            moveDirection * currentMoveSpeed +
            verticalVelocity;

        characterController.Move(
            movement * Time.deltaTime
        );
    }
}
