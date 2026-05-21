using System.Net.Mail;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    public int playerNumber = 1;    // 인스펙터에서 1P는 1, 2P는 2로 설정
    public float gunOffset = 0.7f;  // 플레이어에서 총까지 거리
    public float deadZone = 0.3f;   // 이 범위 안에서는 손 전환 안 함
    private PlayerMovement pm;

    void Start()
    {
        pm = GetComponentInParent<PlayerMovement>();
    }

    void Update()
    {
        float dir = pm.movement;
        if (playerNumber == 1)
        {
            //시점 변경
            if (dir <= -1) //왼쪽
            {
                transform.localPosition = new Vector3(-gunOffset, 0f, 0f);
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (dir >= 1) //오른쪽
            {
                transform.localPosition = new Vector3(gunOffset, 0f, 0f);
                transform.localScale = new Vector3(1f, 1f, 1f);
            }

            //1p는 임의로 H를 누르면 발사되는 걸로 구현
            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                Debug.Log("1p : 발사");
            }
        }
        else if (playerNumber == 2)
        {
            if (dir <= -1) //왼쪽
            {
                transform.localPosition = new Vector3(-gunOffset, 0f, 0f);
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (dir >= 1) //오른쪽
            {
                transform.localPosition = new Vector3(gunOffset, 0f, 0f);
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            
            //2p는 임의로 z를 누르면 발사되는 걸로 구현
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                Debug.Log("2p : 발사");
            }
        }
    }
}
