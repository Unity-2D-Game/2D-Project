using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("넉백 관리")]
    public bool isKnockback = false;
    
    [Header("플레이어 이동 관리")]
    public float movement { get; private set; }  // 수평 이동 입력값 
    public float speed;              // 이동 속도
    public int playerNumber = 1;     // 1P : 1, 2P : 2

    [Header("벽슬라이딩 관리")]
    private bool isTouchingWall = false;
    private bool isWallSliding = false; //이게 필요한가?
    public float wallSlideSpeed;     // 벽 슬라이딩 하강 속도

    [Header("점프 관리")]
    private int jumpCount = 0;       // 현재 점프 횟수
    private bool isGrounded = false;
    public float jumpForce;          // 점프 힘
    public int maxJumpCount = 2;     // 최대 점프 횟수 (카드로 수정 가능)

    [Header("레이어 관리")]
    [SerializeField] public LayerMask GroundLayer;
    [SerializeField] public LayerMask WallLayer;

    [Header("컴포넌트 관리")]
    private RaycastHit2D hit_Ground;
    private Rigidbody2D rb;
    private Collider2D col;

    [Header("경사면 설정")]
    private float SlopeCheck;
    private float finalSpeed;
    private float Temp_gravity;
    [SerializeField] private float climbSpeedMultiplier = 0.7f; 
    [SerializeField] private float descendSpeedMultiplier = 0.8f; // 내리막 속도 (80%)

    // 상태 머신
    public enum PlayerState  {Normal, Dead, Slope}
    public PlayerState curstate = PlayerState.Normal; // 평상시 상태 표기

    // 이벤트
    private PlayerHealth health;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        health = GetComponent<PlayerHealth>();
        health.OnDeath += Die;

        Temp_gravity = rb.gravityScale; //기존 중력값 저장
    }

    void Update()
    {
        // 수평 이동 입력 (1P: AD , 2P: 좌우 방향키)
        movement = 0f;
        if (playerNumber == 1)
        {
            if (Keyboard.current.aKey.isPressed) { movement = -1f;}
            else if (Keyboard.current.dKey.isPressed) { movement = 1f;}
        }
        else
        {
            if (Keyboard.current.leftArrowKey.isPressed) { movement = -1f;}
            else if (Keyboard.current.rightArrowKey.isPressed) { movement = 1f;}
        }

        
        // 점프 입력 (1P: W , 2P: 위 방향키)
        bool jumpPressed = playerNumber == 1
            ? Keyboard.current.kKey.wasPressedThisFrame
            : Keyboard.current.numpad2Key.wasPressedThisFrame;

        if (jumpPressed && (isGrounded || isWallSliding || jumpCount < maxJumpCount))
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            isWallSliding = false;
            jumpCount++;
        }
    }

    void FixedUpdate()
    {
        if (isKnockback) return;

        if (curstate == PlayerState.Slope && isGrounded)
        {
            Slopemode();
        }
        else
        {
            rb.linearVelocity = new Vector2(movement * speed, rb.linearVelocity.y);
        }

        bool stillOnSlope = curstate == PlayerState.Slope && isGrounded && movement == 0f;
        rb.gravityScale = stillOnSlope ? 0f : Temp_gravity;

        // 벽, 바닥 레이캐스트 감지
        float rayLengthX = col.bounds.extents.x + 0.1f;
        float rayLengthY = col.bounds.extents.y + 0.1f;
        //[리팩토링] 플레이어 방향 레이케스트 발사
        Vector2 RayCastDir = movement == 1f ? Vector2.right : Vector2.left;
        //isTouchingWall = Physics2D.Raycast(transform.position, RayCastDir, rayLengthX, WallLayer);

        if (movement != 0f) isTouchingWall = Physics2D.Raycast(transform.position, RayCastDir, rayLengthX, WallLayer);
        else isTouchingWall = false;

        //[리팩토링] 바닥 감지 변환
        Vector2 Boxsize = new Vector2(col.bounds.size.x, 0.1f);
        hit_Ground = Physics2D.BoxCast(transform.position, Boxsize, 0f, Vector2.down, 0.5f, GroundLayer);


        // 벽 슬라이딩: 벽에 붙어있고, 공중이고, 하강 중이고, 벽 방향키 입력 시
        if (isTouchingWall && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
            isWallSliding = true;
            jumpCount = maxJumpCount - 1; // 벽에서 점프 1회 보장
            Debug.Log("슬라이딩");
        }
        else isWallSliding = false;

        if (hit_Ground.collider != null && rb.linearVelocity.y <= 0)
        {
            Debug.Log("바닥 감지");
            isGrounded = true;
            jumpCount = 0;
        }
        else isGrounded = false;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<Block>(out var block))
        {
            switch (block.blockType)
            {
                case Block.BlockType.diamond_block:   curstate = PlayerState.Slope;   break;
            }
        }
    }

    void Slopemode() // 경사면 이동 처리
    {
        if (movement == 0f)
        {
            rb.linearVelocity = Vector2.zero; // 경사 정지 시 미끄럼 방지
            Debug.Log("정지 상태");
            return;
        }

        //경사면 판단
        SlopeCheck = movement * hit_Ground.normal.x;

        if (SlopeCheck < -0.01f)      finalSpeed = speed * climbSpeedMultiplier;   // 오르막
        else if (SlopeCheck > 0.01f)  finalSpeed = speed * descendSpeedMultiplier; // 내리막

        Vector2 slopeDir = new Vector2(hit_Ground.normal.y, -hit_Ground.normal.x);
        rb.linearVelocity = slopeDir * finalSpeed * movement;
    }

    void OnDestroy()
    {
        health.OnDeath -= Die;
    }

    void Die()
    {
        Debug.Log("플레이어 사망");
        //플레이어 사망 로직 넣기
    }
}