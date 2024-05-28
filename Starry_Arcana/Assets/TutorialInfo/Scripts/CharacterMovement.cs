using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // 캐릭터 이동 속도 (타일 당 초당 이동 거리)
    public Tilemap tilemap; // 타일맵 참조
    public TileBase wallTile; // 벽 타일

    private Vector3Int targetTilePosition; // 목표 타일 위치
    private Vector3Int[] path; // 이동 경로
    private int currentPathIndex = 0; // 현재 경로 인덱스

    private void Start()
    {
        targetTilePosition = tilemap.WorldToCell(transform.position); // 현재 위치를 타일 좌표로 변환하여 초기화
    }

    private void Update()
    {
        // 마우스 왼쪽 버튼이 눌렸는지 확인
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 클릭 위치를 월드 좌표로 변환하여 타일맵의 타일 위치로 가져옴
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int clickedTilePosition = tilemap.WorldToCell(mouseWorldPosition);

            // 클릭된 타일 위치로 이동 목표 설정
            targetTilePosition = clickedTilePosition;

            // 새로운 경로 계산
            path = CalculatePath(targetTilePosition);
            currentPathIndex = 0;
        }

        // 경로가 설정되었으면 이동
        if (path != null && path.Length > 0 && currentPathIndex < path.Length)
        {
            // 다음 타일 위치로 이동
            Vector3 targetWorldPosition = tilemap.GetCellCenterWorld(path[currentPathIndex]);
            transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, moveSpeed * Time.deltaTime);

            // 다음 타일에 도착했으면 다음 경로 인덱스로 이동
            if (transform.position == targetWorldPosition)
            {
                currentPathIndex++;
            }
        }
    }

    // A* 알고리즘을 사용하여 최단 경로 계산
    private Vector3Int[] CalculatePath(Vector3Int targetPosition)
    {
        Vector3Int startPosition = tilemap.WorldToCell(transform.position);

        // A* 알고리즘을 사용하여 최단 경로 계산
        List<Vector3Int> path = AStar(startPosition, targetPosition);

        // 시작점을 포함하여 경로를 반환
        path.Insert(0, startPosition);

        // List를 배열로 변환하여 반환
        return path.ToArray();
    }

    private List<Vector3Int> AStar(Vector3Int start, Vector3Int goal)
    {
        List<Vector3Int> openSet = new List<Vector3Int>();
        openSet.Add(start);

        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float>();
        gScore[start] = 0;

        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float>();
        fScore[start] = HeuristicCostEstimate(start, goal);

        while (openSet.Count > 0)
        {
            Vector3Int current = GetLowestFScore(openSet, fScore);

            if (current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);

            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                // 벽 타일인 경우 이웃에서 제외
                if (tilemap.GetTile(neighbor) == wallTile)
                {
                    continue;
                }

                float tentativeGScore = gScore[current] + Vector3Int.Distance(current, neighbor); // 비용을 대각선 거리로 계산

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // No path found
        return new List<Vector3Int>();
    }

    private float HeuristicCostEstimate(Vector3Int start, Vector3Int goal)
    {
        return Vector3Int.Distance(start, goal);
    }

    private Vector3Int GetLowestFScore(List<Vector3Int> openSet, Dictionary<Vector3Int, float> fScore)
    {
        Vector3Int lowest = openSet[0];
        foreach (Vector3Int node in openSet)
        {
            if (fScore.ContainsKey(node) && fScore[node] < fScore[lowest])
            {
                lowest = node;
            }
        }
        return lowest;
    }

    private List<Vector3Int> GetNeighbors(Vector3Int position)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>
        {
            position + Vector3Int.up,
            position + Vector3Int.down,
            position + Vector3Int.left,
            position + Vector3Int.right,
            position + new Vector3Int(1, 1, 0), // top-right
            position + new Vector3Int(1, -1, 0), // bottom-right
            position + new Vector3Int(-1, 1, 0), // top-left
            position + new Vector3Int(-1, -1, 0) // bottom-left
        };

        // 유효한 이웃만 반환
        neighbors.RemoveAll(neighbor => !tilemap.HasTile(neighbor));
        
        return neighbors;
    }

    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        path.Add(current);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse(); // Reverse the path to get it from start to goal
        return path;
    }
}
