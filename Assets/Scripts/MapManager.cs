using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [Header("맵 생성 세팅")]
    [SerializeField] private GameObject[] mapPrefabs; // 맵 프리팹 배열 (여기에 Map_Stage_1 넣기)
    [SerializeField] private Transform mapSpawnPoint;  // 맵 스폰 위치

    [Header("현재 활성화된 맵 데이터")]
    public GameObject currentMap;
    public List<HangingObject> activeRopes = new List<HangingObject>();

    [Header("플레이어 본체 (스폰 연동용)")]
    public Transform player1; // 유니티에서 진짜 플레이어 1번 오브젝트 연결
    public Transform player2; // 유니티에서 진짜 플레이어 2번 오브젝트 연결

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // [테스트] 키보드 1번 누르면 0번 맵 생성 및 플레이어 스폰!
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadMap(0);
        }

        // [테스트] 키보드 3번 누르면 현재 맵 삭제
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ClearCurrentMap();
        }
    }

    /// <summary>
    /// 맵을 로드하고 플레이어들을 스폰 포인트로 순간이동 시키는 핵심 함수
    /// </summary>
    public void LoadMap(int mapIndex)
    {
        if (mapIndex < 0 || mapIndex >= mapPrefabs.Length)
        {
            Debug.LogError($"[MapManager] 유효하지 않은 맵 인덱스입니다: {mapIndex}");
            return;
        }

        ClearCurrentMap();

        // 1. 새 맵 프리팹 찍어내기
        currentMap = Instantiate(mapPrefabs[mapIndex], mapSpawnPoint.position, Quaternion.identity);
        currentMap.name = $"ActiveMap_Index_{mapIndex}";

        // 2. 자식 실(Rope) 자동 수집
        FindAllActiveRopes();

        // 3.  [스폰 연동] 생성된 맵 자식 구조 안에서 스폰 포인트 경로 찾기
        Transform p1Spawn = currentMap.transform.Find("Spawn_Positions/P1_Spawn");
        Transform p2Spawn = currentMap.transform.Find("Spawn_Positions/P2_Spawn");

        // 4. 찾은 스폰 위치로 플레이어 캐릭터들 순간이동 시키기
        if (p1Spawn != null && player1 != null)
        {
            player1.position = p1Spawn.position;
        }
        if (p2Spawn != null && player2 != null)
        {
            player2.position = p2Spawn.position;
        }

        Debug.Log($"[MapManager] 맵 {mapIndex} 로드 및 플레이어 스폰 완료!");
    }

    public void ClearCurrentMap()
    {
        if (currentMap != null)
        {
            Destroy(currentMap);
            currentMap = null;
        }
        activeRopes.Clear();
    }

    private void FindAllActiveRopes()
    {
        if (currentMap == null) return;

        HangingObject[] ropes = currentMap.GetComponentsInChildren<HangingObject>();

        if (ropes != null && ropes.Length > 0)
        {
            activeRopes.AddRange(ropes);
            Debug.Log($"[MapManager] 현재 맵에서 {ropes.Length}개의 매달린 실 오브젝트를 감지하고 등록했습니다.");
        }
    }
}