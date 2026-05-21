using UnityEngine;

public class Rope : MonoBehaviour
{
    // targetHinge = public으로 선언
    public HingeJoint2D targetHinge;
    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
    }

    // targetHinge 를 객체 (줄)에 직접 드래그 앤 드롭
    void LateUpdate() 
    {
        if (targetHinge == null || line == null) return;

        // 1. 실의 시작점
        Vector3 startPos = targetHinge.transform.TransformPoint(targetHinge.anchor);
        line.SetPosition(0, startPos);

        // 2. 실의 끝점: 천장의 고정점 (월드 좌표)
        Vector3 endPos = targetHinge.connectedAnchor;
        line.SetPosition(1, endPos);
    }
}