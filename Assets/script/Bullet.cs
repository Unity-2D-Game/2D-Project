using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float gravity = 4f;
    public float lifeTime = 2f;
    public float maxSpeed = 30f;
    public float drag = 1.5f;

    private Rigidbody2D rb;
    private Vector2 velocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    public void SetVelocity(Vector2 direction, float speed)
    {
        velocity = direction.normalized * speed;
    }

    void FixedUpdate()
    {
        //중력을 적용해서 총알이 아래로 떨어지도록 합니다.
        velocity += Vector2.down * gravity * Time.fixedDeltaTime;

        //공기 저항을 적용해서 총알이 점점 느려지도록 합니다.
        velocity *= (1 - drag * Time.fixedDeltaTime);

        //총알의 속도가 최대 속도를 넘지 않도록 합니다.
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

        //
        rb.linearVelocity = velocity;

        //총알이 이동하는 방향으로 회전하도록 합니다.
        if (velocity != Vector2.zero)
        {
            transform.right = velocity;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);

    }
}