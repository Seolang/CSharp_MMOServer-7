
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    List<GameObject> _objects = new List<GameObject>();

    public void Add(GameObject go)
    {
        _objects.Add(go);
    }

    public void Remove(GameObject go)
    {
        _objects.Remove(go);
    }

    public GameObject Find(Vector3Int cellPos)
    {
        foreach (GameObject obj in _objects)
        {
            // ���� ������Ʈ���� CreatureController ������Ʈ�� �����´�
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null) // CreatureComponent�� ��� �ϴ��� �Ѿ
                continue;

            if (cc.CellPos == cellPos) // �ش� ���� ������Ʈ�� ��ǥ�� ã�� ��ġ��� ��ü ��ȯ
                return obj;
        }

        return null;
    }

    public void Clear()
    {
        _objects.Clear();
    }
}
