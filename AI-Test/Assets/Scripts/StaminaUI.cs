using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Image staminaBarFill;
    [SerializeField] private TMP_Text staminaText;

    private void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        if (player == null)
        {
            Debug.LogError(
                "StaminaUI: PlayerController를 찾을 수 없습니다.",
                this
            );

            enabled = false;
            return;
        }

        if (staminaBarFill == null || staminaText == null)
        {
            Debug.LogError(
                "StaminaUI: Inspector 연결을 확인하세요.",
                this
            );

            enabled = false;
            return;
        }

        UpdateStaminaUI();
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        UpdateStaminaUI();
    }

    private void UpdateStaminaUI()
    {
        float current = player.CurrentStamina;
        float maximum = player.MaxStamina;

        float ratio = maximum > 0f
            ? current / maximum
            : 0f;

        staminaBarFill.fillAmount = Mathf.Clamp01(ratio);

        staminaText.text =
            $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(maximum)}";
    }
}

