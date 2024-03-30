
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
            // 게임 오브젝트에서 CreatureController 컴포넌트를 가져온다
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null) // CreatureComponent가 없어도 일단은 넘어감
                continue;

            if (cc.CellPos == cellPos) // 해당 게임 오브젝트의 좌표가 찾는 위치라면 객체 반환
                return obj;
        }

        return null;
    }

    public void Clear()
    {
        _objects.Clear();
    }
}
