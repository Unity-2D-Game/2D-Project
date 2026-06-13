using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float lifeTime = 2f;
    public float maxSpeed = 20f;

    [Header("Damage Settings")]
    public int damage = 20;

    [Header("Bounce Settings")]
    public int maxBounceCount = 1;

    private int bounceCount = 0;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetVelocity(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private void FixedUpdate()
    {
        // 최대 속도 제한
        rb.linearVelocity =
            Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);

        // 이동 방향으로 회전
        if (rb.linearVelocity != Vector2.zero)
        {
            transform.right = rb.linearVelocity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어 피격 처리
        PlayerHealth playerHealth =
            collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        // 벽/바닥/천장 등에 대한 튕김 처리
        bounceCount++;

        if (bounceCount > maxBounceCount)
        {
            Destroy(gameObject);
        }
    }
}