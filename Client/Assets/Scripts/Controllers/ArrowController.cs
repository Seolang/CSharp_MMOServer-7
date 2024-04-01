using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : CreatureController
{
    protected override void Init()
    {
        _speed = 10f;

        // ȭ�� ��������Ʈ ���� ����
        switch (_lastDir)
        {
            case MoveDir.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case MoveDir.Down:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case MoveDir.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case MoveDir.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }

        base.Init();
    }

    protected override void UpdateAnimation()
    {
        // Empty
    }

    protected override void UpdateIdle()
    {
        if (_dir != MoveDir.None)
        {
            Vector3Int destPos = CellPos; // ������ �� ������ ��ǥ

            switch (_dir)
            {
                case MoveDir.Up:
                    destPos += Vector3Int.up;
                    break;

                case MoveDir.Down:
                    destPos += Vector3Int.down;
                    break;

                case MoveDir.Left:
                    destPos += Vector3Int.left;
                    break;

                case MoveDir.Right:
                    destPos += Vector3Int.right;
                    break;
            }

            State = CreatureState.Moving;

            // ������ ��ǥ�� �̵� �����ϰ�, �ٸ� ������Ʈ�� ������ üũ
            if (Managers.Map.CanGo(destPos))
            {
                GameObject go = Managers.Object.Find(destPos);
                if (go == null)
                {
                    CellPos = destPos;
                }
                else // � ��ü�� �ε��� ���
                {
                    Debug.Log(go.name);
                    // ȭ�쿡 ���� ��ü�� ������ �޼ҵ带 ����
                    CreatureController cc = go.GetComponent<CreatureController>();
                    if (cc != null)
                        cc.OnDamaged();

                    // ȭ�� ����
                    Managers.Resource.Destroy(gameObject);
                }
            }
            else // ���� ���� ��ġ�� ���
            {
                Managers.Resource.Destroy(gameObject);
            }
        }
    }
}
