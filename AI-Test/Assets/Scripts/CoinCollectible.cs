using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] private LayerMask playerLayer;

    private bool collected;

    private void OnEnable()
    {
        // 풀에서 다시 활성화될 때 수집 상태 초기화
        collected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 이미 수집한 코인이면 무시
        if (collected)
        {
            return;
        }

        // 충돌한 오브젝트의 Layer가
        // playerLayer에 포함되어 있는지 확인
        int otherLayer = other.gameObject.layer;

        bool isPlayer =
            (playerLayer.value & (1 << otherLayer)) != 0;

        if (!isPlayer)
        {
            return;
        }

        // 게임 진행 중에만 코인 수집 가능
        if (GameManager.Instance == null ||
            GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        collected = true;

        // 코인 개수 증가
        GameManager.Instance.AddCoin(1);

        Debug.Log("코인 수집 완료! 총 코인: "
            + GameManager.Instance.CoinCount);

        // 파괴하지 않고 풀에 반환
        gameObject.SetActive(false);
    }
}