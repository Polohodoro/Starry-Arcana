using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapGenerator : MonoBehaviour
{
    public Tilemap tilemap; // 타일맵 참조
    public TileBase floorTile; // 바닥 타일
    public TileBase wallTile; // 벽 타일

    // generateMap 스크립트에 접근하기 위한 참조
    public GenerateMap mapGenerator;

    void Start()
    {
        GenerateTilemap();
    }

    void GenerateTilemap()
    {
        // generateMap 스크립트를 통해 맵 생성
        bool[,] map = mapGenerator.CreateMap();

        // 생성된 맵을 기반으로 타일맵에 타일 배치
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                // 맵의 값이 참(true)인 경우 타일을 배치
                if (map[x, y])
                {
                    // 타일맵 좌표에 타일 배치
                    tilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
                else
                {
                    // 타일맵 좌표에 타일 배치
                    tilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                }
            }
        }
    }
}
