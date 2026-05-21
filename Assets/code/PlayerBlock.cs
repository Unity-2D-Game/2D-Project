using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBlock : MonoBehaviour
{
    [Header("블록 설정")]
    public float cooltime = 2f; //보호막 사용 주기    
    float durationTimer; //보호막 지속시간
    public float duration = 2f; //보호막 지속시간
    float currcolltime; //현제 쿨타임 계산
    public int playerNumber;

    public bool IsBlocking { get; private set; }
    SpriteRenderer shieldSprite;
    Collider2D shieldCollider;
    public GameObject shieldObject;

    void Start()
    {
        shieldSprite = shieldObject.GetComponent<SpriteRenderer>();
        shieldCollider = shieldObject.GetComponent<Collider2D>();
        shieldSprite.enabled = false;
        shieldCollider.enabled = false;
        currcolltime = cooltime;
    }
    
    void Update()
    {
        //각 플레이어 쉴드 클릭 감지
        bool player_1_Click = Keyboard.current.jKey.wasPressedThisFrame; //1p = j
        bool player_2_Click = Keyboard.current.zKey.wasPressedThisFrame; //2p = x

        if (playerNumber == 1) //1p 플레이어
        {
            //쿨타임 관리
            if (IsBlocking) //보호막이 작동 했다면
            {
                durationTimer += Time.deltaTime; //보호막 지속시간 돌리기
                if (durationTimer >= duration)
                {
                    IsBlocking = false;
                    shieldSprite.enabled = false;
                    shieldCollider.enabled = false;
                    durationTimer = 0f;
                } 
            }
            else if (!IsBlocking) //보호막이 작동하지 않았다면
            {
                currcolltime += Time.deltaTime;
            }
    
            if (player_1_Click && currcolltime >= cooltime)
            {
                Debug.Log("보호막 작동");
                shieldSprite.enabled = true;
                shieldCollider.enabled = true;
                currcolltime = 0f; //쿨타임 초기화
                IsBlocking = true;
            }
        } 
        else if (playerNumber == 2) //2p 플레이어
        {
            //쿨타임 관리
            if (IsBlocking) //보호막이 작동 했다면
            {
                durationTimer += Time.deltaTime; //보호막 지속시간 돌리기
                if (durationTimer >= duration)
                {
                    IsBlocking = false;
                    shieldSprite.enabled = false;
                    shieldCollider.enabled = false;
                    durationTimer = 0f;
                } 
            }
            else if (!IsBlocking) //보호막이 작동하지 않았다면
            {
                currcolltime += Time.deltaTime;
            }
    
            if (player_2_Click && currcolltime >= cooltime)
            {
                Debug.Log("보호막 작동");
                shieldSprite.enabled = true;
                shieldCollider.enabled = true;
                currcolltime = 0f; //쿨타임 초기화
                IsBlocking = true;
            }
        }


        
    }

}
