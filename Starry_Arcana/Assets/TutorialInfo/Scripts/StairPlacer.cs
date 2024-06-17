using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StairPlacer : MonoBehaviour
{
    public Tilemap tilemap; // 타일맵 참조
    public TileBase floorTile; // 바닥 타일
    public TileBase wallTile; // 벽 타일
    public GameObject stair; // 계단 오브젝트

    public void PlaceStairs(Vector3Int centerPosition)
    {
        // 중심 위치를 기준으로 3x3 영역에 타일 및 오브젝트 배치
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int position = new Vector3Int(centerPosition.x + x, centerPosition.y + y, 0);

                if (x == 0 && y == 0)
                {
                    // 중심 위치에 바닥 타일 배치 후 계단 오브젝트 배치
                    tilemap.SetTile(position, floorTile);
                    Vector3 worldPosition = tilemap.CellToWorld(position) + tilemap.cellSize / 2;
                    Instantiate(stair, new Vector3(worldPosition.x, worldPosition.y, 1f), Quaternion.identity); // Z 좌표를 1로 설정하여 계단이 암흑 타일 뒤에 배치되도록 함
                }
                else if ((x == -1 && y == -1) || (x == 0 && y == -1) || (x == 1 && y == -1))
                {
                    // 7, 8, 9번 위치에 바닥 타일 배치
                    tilemap.SetTile(position, floorTile);
                }
                else
                {
                    // 1, 2, 3, 4, 6번 위치에 벽 타일 배치
                    tilemap.SetTile(position, wallTile);
                }
            }
        }
    }

    public void PlaceStairsInCenter()
    {
        // 중심 위치로 사용할 타일맵의 중간 위치를 계산
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int mapCenter = new Vector3Int(bounds.xMin + bounds.size.x / 2, bounds.yMin + bounds.size.y / 2, 0);
        PlaceStairs(mapCenter);
    }
}
