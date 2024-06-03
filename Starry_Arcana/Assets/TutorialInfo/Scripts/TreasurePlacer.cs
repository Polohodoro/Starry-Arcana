using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreasurePlacer : MonoBehaviour
{
    public GameObject treasurePrefab; // 보물 프리팹 참조
    public GenerateMap mapGenerator;

    public void PlaceTreasures(int numberOfTreasures)
    {
        bool[,] map = mapGenerator.cellmap;
        int treasureHiddenLimit = 5;

        List<Vector3Int> possibleLocations = new List<Vector3Int>();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (!map[x, y])
                {
                    int nbs = mapGenerator.CountAliveNeighbours(map, x, y);
                    if (nbs >= treasureHiddenLimit)
                    {
                        possibleLocations.Add(new Vector3Int(x, y, 0));
                    }
                }
            }
        }

        // 랜덤하게 보물 위치 선택
        List<Vector3Int> selectedLocations = possibleLocations.OrderBy(loc => Random.value).Take(numberOfTreasures).ToList();

        foreach (var loc in selectedLocations)
        {
            PlaceTreasureObject(loc.x, loc.y);
        }
    }

    void PlaceTreasureObject(int x, int y)
    {
        // 보물 프리팹을 지정된 위치에 인스턴스화
        Vector3 position = new Vector3(x + 0.5f, y + 0.5f, 0);
        Instantiate(treasurePrefab, position, Quaternion.identity);
    }
}