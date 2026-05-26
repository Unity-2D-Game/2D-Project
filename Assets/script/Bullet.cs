using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float lifeTime = 2f;
    public float maxSpeed = 30f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetVelocity(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    void FixedUpdate()
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}