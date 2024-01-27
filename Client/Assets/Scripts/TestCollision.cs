using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestCollision : MonoBehaviour
{

    public Tilemap _tilemap;
    public TileBase _tile;

    void Start()
    {
        _tilemap.SetTile(new Vector3Int(0, 0, 0), _tile);
    }

    void Update()
    {
        List<Vector3Int> blocked = new();

        // cellBounds : 타일맵의 경계선
        // allPositionsWithin : 타일맵의 모든 영역
        foreach(Vector3Int pos in _tilemap.cellBounds.allPositionsWithin)
        {
            // 타일맵 내부의 모든 셀의 위치를 탐색하며 해당 위치에 타일이 존재하는지 확인
            TileBase tile = _tilemap.GetTile(pos);
            if (tile != null)
                blocked.Add(pos);
        }
    }
}
