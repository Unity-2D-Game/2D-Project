using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapManager : MonoBehaviour
{
    // 어디서나 이 대장 매니저를 쉽게 부를 수 있도록 싱글톤 규칙을 적용함
    // *싱글톤(Singleton)*: 게임 전체에서 단 하나의 인스턴스만 존재하도록 보장하는 디자인 패턴

    public static MapManager Instance { get; private set; }

    [Header("맵 생성 세팅")]
    [SerializeField] private GameObject[] mapPrefabs;   //  원본 맵 파일(프리팹)들을 담아두는 보관함 배열
    [SerializeField] private Transform mapSpawnPoint;    // 새 맵이 찍힐 게임 월드상의 정중앙 기준점 좌표

    [Header("현재 활성화된 맵 데이터")]
    public GameObject currentMap;                        // 현재 게임 화면에 켜져서 실시간 연산 중인 진짜 필드 맵
    public List<HangingObject> activeRopes = new List<HangingObject>(); // 맵 하위에서 흔들리는 실 기믹들을 모아둘 주머니

    [Header("플레이어 본체 (스폰 연동용)")]
    public Transform player1;                            // 위치를 강제로 옮길 1번 플레이어
    public Transform player2;                            // 위치를 강제로 옮길 2번 플레이어

    // 게임이 켜지거나 오브젝트가 메모리에 올라올 때 최우선 1회 자동 실행
    private void Awake()
    {
        // 메모리에 대장 매니저 자리가 비어있다면 (최초 실행 시점)
        if (Instance == null)
        {
            Instance = this;                      // 자기 자신을 초대 인스턴스로 설정
            DontDestroyOnLoad(gameObject);        // 다음 판으로 씬이 넘어가도 이 대장을 지우지 못하게 보호함
        }
        // 만약 방이 다시 로드되면서 똑같은 대장 매니저가 또 생겨났다면
        else
        {
            Destroy(gameObject);                  // 없애기
        }
    }

    // 매 프레임마다 실시간 무한 루프 돌며 입력 상태를 감시하는 업데이트 함수
    void Update()
    {
    if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadMap(0); // 1번째 칸 (Map1)
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadMap(1); // 2번째 칸 (Map2)
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadMap(2); // 3번째 칸 (Map3)
        }
    }

    /// <summary>
    /// 숫자를 받아서 기존 맵을 부수고, 지정을 맵을 새로 복사해온 뒤 캐릭터를 순간이동 시키는 함수
    /// </summary>
    public void LoadMap(int mapIndex)
    {
        // 예외 방어 코드: 만약 전달받은 숫자가 음수이거나, 등록한 프리팹 개수를 벗어나는 값일 경우
        if (mapIndex < 0 || mapIndex >= mapPrefabs.Length)
        {
            Debug.LogError($"[MapManager] 등록되지 않은 잘못된 맵 입니다: {mapIndex}");
            return; // 에러 로그 띄우고 더 이상 아래 코드가 실행되지 않게 함수를 강제 종료함
        }

        // 새 맵을 로드하기 전에 원래 있던 이전 판 맵 자산 지우기 (오버랩 방지용)
        ClearCurrentMap();

        // 원본 맵 프리팹을 꺼내 정해진 기준점 좌표에 회전 없이 실물 게임 오브젝트로 전환
        currentMap = Instantiate(mapPrefabs[mapIndex], mapSpawnPoint.position, Quaternion.identity);
        currentMap.name = $"ActiveMap_Index_{mapIndex}";

        // 새 맵이 태어났으므로 내부의 실 오브젝트들을 수집하는 함수 호출
        FindAllActiveRopes();

        // 새로 생성된 맵 내부 폴더 구조를 이름 경로로 찾아 플레이어들이 태어날 좌표 설정
        Transform p1Spawn = currentMap.transform.Find("Spawn_Positions/P1_Spawn");
        Transform p2Spawn = currentMap.transform.Find("Spawn_Positions/P2_Spawn");

        // 방어 코드: 1번 스폰 위치 좌표를 찾았고, 연결해둔 1번 플레이어 몸체가 비어있지 않다면
        if (p1Spawn != null && player1 != null)
        {
            player1.position = p1Spawn.position; // 다른 곳에 있던 플레이어 1번의 실제 좌표를 새 맵의 1번 스폰 위치 좌표로 순간이동
        }
        
        // 2번 스폰 위치 좌표도 정상 매핑 완료되었다면
        if (p2Spawn != null && player2 != null)
        {
            player2.position = p2Spawn.position; // 플레이어 2번도 자기 자리에 순간이동
        }

        Debug.Log($"[MapManager] 스테이지 {mapIndex} 로드 및 플레이어 리스폰 위치 세팅 완료");
    }

    /// <summary>
    /// 화면에 떠 있는 현재 스테이지를 완전히 없애고 메모리 잔상을 청소하는 함수
    /// </summary>
    public void ClearCurrentMap()
    {
        // 현재 들고 있는 맵 변수가 비어있지 않고 실물이 채워져 있다면 청소 작업 시작
        if (currentMap != null)
        {
            Destroy(currentMap); // 유니티 가비지 컬렉터에게 명령하여 현재 스테이지 하부 모든 박스와 기믹들을 통째로 파괴
            currentMap = null;   // 주소 잔상이 남아 에러를 유도하는 유령 참조를 막기 위해 변수를 null로 초기화
        }
        activeRopes.Clear();     // 매달린 실 스크립트들을 모아두던 바구니 초기화
    }

    /// <summary>
    /// 새로 태어난 스테이지 부모 밑에 숨겨진 자식 실(Rope) 물리 오브젝트들을 자동으로 전부 스캔하는 함수
    /// </summary>
    private void FindAllActiveRopes()
    {
        if (currentMap == null) return; // 화면에 맵 자체가 안 켜져 있다면 하부 조사가 불가능하므로 즉시 함수 취소 및 탈출

        // 부모 맵 밑에 매달려 있는 자식 상자나 마름모들에 부착된 HangingObject 스크립트들을 전수조사하여 배열에 쓸어 담음
        HangingObject[] ropes = currentMap.GetComponentsInChildren<HangingObject>();
        
        // 자식 실들이 단 하나라도 정상적으로 발견되었다면
        if (ropes != null && ropes.Length > 0)
        {
            activeRopes.AddRange(ropes); // 임시로 모은 배열 데이터들을 매니저가 상시 관리하는 동적 리스트 바구니에 이식 (다른 클래스들과 연동하기 위함)
        }
    }
}