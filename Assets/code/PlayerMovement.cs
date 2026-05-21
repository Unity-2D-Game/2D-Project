using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    public float speed;              // 이동 속도
    public float jumpForce;          // 점프 힘
    public float wallSlideSpeed;     // 벽 슬라이딩 하강 속도
    public LayerMask wallLayer;      // 레이캐스트가 감지할 벽/바닥 레이어
    public int maxJumpCount = 2;     // 최대 점프 횟수 (카드로 수정 가능)
    public int playerNumber = 1;     // 1P : 1, 2P : 2
    public bool isKnockback = false;

    private Rigidbody2D rb;
    private Collider2D col;
    public float movement { get; private set; }  // 수평 이동 입력값 
    private bool isGrounded = false;
    private bool isWallSliding = false;
    private bool isTouchingWall = false;
    private bool horizontal = false; // 수평 방향키 입력 여부
    private int jumpCount = 0;       // 현재 점프 횟수

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        // 수평 이동 입력 (1P: AD , 2P: 좌우 방향키)
        movement = 0f;
        if (playerNumber == 1)
        {
            if (Keyboard.current.aKey.isPressed) { movement = -1f; horizontal = true; }
            else if (Keyboard.current.dKey.isPressed) { movement = 1f; horizontal = true; }
            else horizontal = false;
        }
        else
        {
            if (Keyboard.current.leftArrowKey.isPressed) { movement = -1f; horizontal = true; }
            else if (Keyboard.current.rightArrowKey.isPressed) { movement = 1f; horizontal = true; }
            else horizontal = false;
        }

        // 벽, 바닥 레이캐스트 감지
        float rayLengthX = col.bounds.extents.x + 0.1f;
        float rayLengthY = col.bounds.extents.y + 0.1f;
        bool isWallRight = Physics2D.Raycast(transform.position, Vector2.right, rayLengthX, wallLayer);
        bool isWallLeft = Physics2D.Raycast(transform.position, Vector2.left, rayLengthX, wallLayer);
        bool isWallDown = Physics2D.Raycast(transform.position, Vector2.down, rayLengthY, wallLayer);
        isTouchingWall = isWallRight || isWallLeft;

        // 바닥에 닿아있을 때 점프 횟수 초기화 (상승 중엔 초기화 안 함)
        if (isWallDown && rb.linearVelocity.y <= 0)
        {
            jumpCount = 0;
        }

        // 점프 입력 (1P: W , 2P: 위 방향키)
        bool jumpPressed = playerNumber == 1
        
            ? Keyboard.current.wKey.wasPressedThisFrame
            : Keyboard.current.upArrowKey.wasPressedThisFrame;

        if (jumpPressed && (isGrounded || isWallSliding || jumpCount < maxJumpCount))
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            isWallSliding = false;
            jumpCount++;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    void FixedUpdate()
    {
        if (isKnockback) return;
        rb.linearVelocity = new Vector2(speed * movement, rb.linearVelocity.y);

        // 벽 슬라이딩: 벽에 붙어있고, 공중이고, 하강 중이고, 벽 방향키 입력 시
        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0 && horizontal)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
            isWallSliding = true;
            jumpCount = maxJumpCount - 1; // 벽에서 점프 1회 보장
        }
    }
}
