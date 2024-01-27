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

        // cellBounds : Ÿ�ϸ��� ��輱
        // allPositionsWithin : Ÿ�ϸ��� ��� ����
        foreach(Vector3Int pos in _tilemap.cellBounds.allPositionsWithin)
        {
            // Ÿ�ϸ� ������ ��� ���� ��ġ�� Ž���ϸ� �ش� ��ġ�� Ÿ���� �����ϴ��� Ȯ��
            TileBase tile = _tilemap.GetTile(pos);
            if (tile != null)
                blocked.Add(pos);
        }
    }
}
