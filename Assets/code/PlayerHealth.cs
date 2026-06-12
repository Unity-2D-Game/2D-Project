using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHp = 100f;                   // 최대 체력 (카드로 수정 가능)
    public float CurrentHp { get; private set; } // 현재 체력 (외부에서 읽기만 가능)
    public float knockbackForce; //플레이어가 뒤로 날아가는 힘 크기 (총알 충돌시)
    private shield sh;
    private Color originalColor; //현제 플레이어 색 저장
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    public event System.Action OnDeath;

    void Start()
    {
        CurrentHp = maxHp;
        sh = GetComponent<shield>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        rb = GetComponent<Rigidbody2D>();
    }

    IEnumerator flashdamage()
    {
        sr.color = Color.red; 
        yield return new WaitForSeconds(0.2f);
        sr.color = originalColor;
    }

    void OnTriggerEnter2D(Collider2D other) //총알이랑 충돌
    {
        if (other.CompareTag("Bullet"))
        {
            if (sh.IsBlocking)
                return; // 쉴드 켜져있으면 데미지 무시
            TakeDamage(10f);
            StartCoroutine(flashdamage());
            float dirX = (transform.position.x - other.transform.position.x) > 0 ? 1f : -1f;
            float knockbackUpwardForce = 2f;
            PlayerMovement pm = GetComponent<PlayerMovement>();
            pm.isKnockback = true;
            rb.linearVelocity = new Vector2(dirX * knockbackForce, knockbackUpwardForce);
            StartCoroutine(ResetKnockback(pm));
        }
    }

    IEnumerator ResetKnockback(PlayerMovement pm)
    {
        yield return new WaitForSeconds(0.3f);
        pm.isKnockback = false;
    }

    // 데미지를 받을 때 호출 (투사체 등 외부에서 호출)
    public void TakeDamage(float damage)
    {
        CurrentHp = Mathf.Clamp(CurrentHp - damage, 0, maxHp);
        if (CurrentHp <= 0) OnDeath?.Invoke();
    }
}