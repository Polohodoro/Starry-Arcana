using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // 캐릭터 이동 속도 (타일 당 초당 이동 거리)
    public Tilemap tilemap; // 타일맵 참조
    public Tilemap fogTilemap; // 암흑 타일맵 참조
    public TileBase wallTile; // 벽 타일
    public TileBase fogTile; // 암흑 타일
    public TileMapGenerator tileMapGenerator; // TileMapGenerator 참조
    public BoxUIController boxUIController; // BoxUIController 참조
    public float pauseDuration = 0.5f; // 타일 이동 후 멈추는 시간

    private Vector3Int targetTilePosition; // 목표 타일 위치
    private Vector3Int[] path; // 이동 경로
    private int currentPathIndex = 0; // 현재 경로 인덱스
    private float characterZ; // 캐릭터의 Z 좌표
    private bool isMoving = false; // 이동 중인지 여부

    private void Start()
    {
        targetTilePosition = tilemap.WorldToCell(transform.position); // 현재 위치를 타일 좌표로 변환하여 초기화
        characterZ = transform.position.z; // 캐릭터의 현재 Z 좌표를 저장
    }

    private void Update()
    {
        // 마우스 왼쪽 버튼이 눌렸는지 확인
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            // 마우스 클릭 위치를 월드 좌표로 변환하여 타일맵의 타일 위치로 가져옴
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int clickedTilePosition = tilemap.WorldToCell(mouseWorldPosition);

            // 클릭된 타일 위치로 이동 목표 설정
            targetTilePosition = clickedTilePosition;

            // 새로운 경로 계산
            path = CalculatePath(targetTilePosition);

            // 경로가 유효하고 암흑 타일이 없을 때 이동 시작
            if (path != null && path.Length > 0 && !PathHasFogTile(path))
            {
                currentPathIndex = 0;
                StartCoroutine(MoveAlongPath());
            }

            // 클릭된 위치에 상자가 있는지 확인
            CheckForBox(mouseWorldPosition);
        }
    }

    private IEnumerator MoveAlongPath()
    {
        isMoving = true;

        while (currentPathIndex < path.Length)
        {
            // 다음 타일 위치로 이동
            Vector3 targetWorldPosition = tilemap.GetCellCenterWorld(path[currentPathIndex]);
            // 이동할 때 Z 좌표를 유지
            Vector3 targetPosition = new Vector3(targetWorldPosition.x, targetWorldPosition.y, characterZ);
            while (transform.position != targetPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // 다음 타일에 도착했으면 다음 경로 인덱스로 이동
            currentPathIndex++;
            tileMapGenerator.UpdateFogOfWar(); // 캐릭터 위치 기준으로 시야 갱신

            // 한 타일 이동 후 잠깐 멈춤
            if (currentPathIndex < path.Length)
            {
                yield return new WaitForSeconds(pauseDuration);
            }
        }

        isMoving = false;
    }

    private void CheckForBox(Vector3 worldPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Box"))
            {
                // 상자가 있는 경우 UI 표시
                boxUIController.ShowBoxUI();
                break;
            }
        }
    }

    // 경로에 암흑 타일이 있는지 확인하는 메서드
    private bool PathHasFogTile(Vector3Int[] path)
    {
        foreach (Vector3Int tilePosition in path)
        {
            if (fogTilemap.GetTile(tilePosition) == fogTile)
            {
                return true;
            }
        }
        return false;
    }

    // A* 알고리즘을 사용하여 최단 경로 계산
    private Vector3Int[] CalculatePath(Vector3Int targetPosition)
    {
        Vector3Int startPosition = tilemap.WorldToCell(transform.position);

        // A* 알고리즘을 사용하여 최단 경로 계산
        List<Vector3Int> path = AStar(startPosition, targetPosition);

        // 시작점을 포함하여 경로를 반환
        path.Insert(0, startPosition);

        // 경로가 유효한지 확인
        if (PathHasFogTile(path.ToArray()))
        {
            return null; // 경로에 암흑 타일이 있는 경우 null 반환
        }

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
                // 벽 타일이나 암흑 타일인 경우 이웃에서 제외
                if (tilemap.GetTile(neighbor) == wallTile || fogTilemap.GetTile(neighbor) == fogTile)
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
