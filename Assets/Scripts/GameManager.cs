using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 어디서나 이 게임 매니저(심판)를 쉽게 부를 수 있도록 싱글톤 규칙을 적용함
    public static GameManager Instance { get; private set; }

    [Header("게임 상태 데이터 (규칙 및 점수)")]
    public int currentRound = 1;       // 현재 몇 라운드가 진행 중인지 저장하는 변수
    public int player1Score = 0;       // 1번 플레이어의 현재 매치 점수
    public int player2Score = 0;       // 2번 플레이어의 현재 매치 점수
    public bool isGameOver = false;    // 게임이 완전히 끝났는지 체크하는 스위치

    // 게임이 켜지거나 오브젝트가 메모리에 올라올 때 최우선 1회 자동 실행
    private void Awake()
    {
        // 메모리에 게임 매니저 자리가 비어있다면 (최초 실행 시점)
        if (Instance == null)
        {
            Instance = this;                      // 자기 자신을 전역 심판 대장으로 정식 임명함
            DontDestroyOnLoad(gameObject);        // 다음 판으로 방(Scene)이 넘어가도 이 시스템을 지우지 못하게 보호함
        }
        // 만약 방이 다시 로드되면서 똑같은 게임 매니저 분신이 또 생겨났다면
        else
        {
            Destroy(gameObject);                  // 시스템 혼선을 막기 위해 새로 생긴 가짜 분신 객체를 즉각 파괴함
        }
    }

    // 매 프레임마다 키보드 입력을 감시함 (테스트용 기능)
    private void Update()
    {
        // 키보드의 O(알파벳) 키를 누르면 1번 플레이어가 죽은 것으로 간주
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("[치트키 발동] O 키 입력 -> 1번 플레이어 강제 사망 처리");
            OnPlayerDied(1);
        }

        // 키보드의 P(알파벳) 키를 누르면 2번 플레이어가 죽은 것으로 간주
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("[치트키 발동] P 키 입력 -> 2번 플레이어 강제 사망 처리");
            OnPlayerDied(2); 
        }
    }

    /// <summary>
    /// 종민님 파트나 준호님 파트에서 플레이어 죽었ㅇ을 때 여기서 감지해서 승패 반영해줌
    /// </summary>
    public void OnPlayerDied(int deadPlayerNumber)
    {
        if (isGameOver) return;

        Debug.Log($"[GameManager] {deadPlayerNumber}번 플레이어 사망 !");

        if (deadPlayerNumber == 1)
        {
            player2Score++; // 1번이 죽었으니 2번이 1점 획득
        }
        else if (deadPlayerNumber == 2)
        {
            player1Score++; // 2번이 죽었으니 1번이 1점 획득
        }

        Debug.Log($"[UI 연동용 로그] 현재 스코어 -> P1: {player1Score} VS P2: {player2Score}");

        // 누군가 먼저 2점을 선취하여 최종 승리했는지 체크
        if (player1Score >= 2 || player2Score >= 2)
        {
            EndMatch(); // 최종 매치 종료 함수 가동
        }
        else
        {
            AdvanceToNextRound(); // 다음 라운드 진행 함수 가동
        }
    }

    /// <summary>
    /// 다음 라운드로 전환하며 배경/오브젝트 시스템과 연동하는 핵심 함수
    /// </summary>
    private void AdvanceToNextRound()
    {
        currentRound++; 
        Debug.Log($"[GameManager] 다음 라운드로 진입합니다. 현재 라운드: {currentRound}");

        // 싱글톤 연동
        if (MapManager.Instance != null)
        {
            // 라운드 숫자에 맞춰 옛날 맵을 지우고 새 맵을 깔도록 명령
            MapManager.Instance.LoadMap(currentRound - 1); 
        }
        else
        {
            Debug.LogError("[GameManager] 화면에 MapManager 가 존재하지 않습니다. 맵 전환이 불가능합니다.");
        }
    }

    /// <summary>
    /// 경기 한 판이 완전히 끝나서 최종 우승자가 나왔을 때 가동되는 함수
    /// </summary>
    private void EndMatch()
    {
        isGameOver = true; 
        int winnerNumber = (player1Score >= 2) ? 1 : 2; 
        Debug.Log($"[GameManager] 우승자는 {winnerNumber} 플레이어 !");
    }
}