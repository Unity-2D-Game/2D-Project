using UnityEngine;

// 이 스크립트를 추가할 때 LineRenderer, HingeJoint2D, Rigidbody2D 컴포넌트가 자동으로 함께 추가되도록 강제함
[RequireComponent(typeof(LineRenderer))] // 실 이미지 (시각적으로 보이는)
[RequireComponent(typeof(HingeJoint2D))] // 오브젝트 고정 못 (+ 실 기능)
[RequireComponent(typeof(Rigidbody2D))]   // 부피와 무게 부여
public class HangingObject : MonoBehaviour
{
    // 컴포넌트들을 코드에서 제어하기 위해 담아둘 내부 변수 선언
    private LineRenderer lineRenderer;
    private HingeJoint2D hingeJoint2D;
    private Rigidbody2D rigidBody2D;

    [Header(" 연결 지점 (Anchor)")]
    [SerializeField] private Transform anchorTransform; // 실이 고정될 천장 위치 데이터를 저장

    // 게임 시작 시 최초 1회만 실행
    void Start()
    {
        // GetComponent로 값을 받아와서 변수에 각각 할당
        lineRenderer = GetComponent<LineRenderer>();
        hingeJoint2D = GetComponent<HingeJoint2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();

        // LineRenderer(줄 그리기) 초기 설정
        lineRenderer.positionCount = 2;  // 점 2개를 이어서 하나의 선을 만듦
        lineRenderer.startWidth = 0.05f; // 실의 시작점 두께 설정
        lineRenderer.endWidth = 0.05f;   // 실의 끝점 두께를 똑같이 맞춰 일정한 굵기로 만듦

    }

    // 매 프레임마다 실행 (초당 약 60회 이상 화면을 갱신할 때마다 호출)
    void Update()
    {
        // 천장 오브젝트가 정상적으로 존재할 때만 실을 그림 (방어코드)
        if (anchorTransform != null)
        {
            // 실의 0번째 점(시작점)의 위치를 지정된 천장의 실시간 위치로 지정
            lineRenderer.SetPosition(0, anchorTransform.position);
            // 실의 1번째 점(끝점)의 위치를 현재 흔들리는 자기 자신의 실시간 위치로 지정
            lineRenderer.SetPosition(1, transform.position);
        }
    }

    /// <summary>
    /// 외부(예: 총알, 폭발)에서 호출하여 오브젝트에 순간적인 충격을 주는 함수
    /// </summary>
    public void OnHit(Vector2 forceDirection, float forceMagnitude)
    {
        // 순수한 방향 데이터만 남기기 위해 벡터 정규화(Normalized) 진행
        // 지속적인 힘이 아닌, 탁! 치는 순간적인 타격감을 위해 ForceMode2D.Impulse 사용
        rigidBody2D.AddForce(forceDirection.normalized * forceMagnitude, ForceMode2D.Impulse);
    }

    // 이 오브젝트에 무언가 물체가 부딪혔을 때 유니티가 자동으로 실행해주는 함수
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 부딪힌 물체의 태그가 "Player"인지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"{collision.gameObject.name}와 충돌! 오브젝트가 흔들립니다.");
        }
    }
}