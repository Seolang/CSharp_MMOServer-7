using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : CreatureController
{
    protected override void Init()
    {
        _speed = 10f;

        // 화살 스프라이트 방향 설정
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
            Vector3Int destPos = CellPos; // 앞으로 갈 예정인 좌표

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

            // 가야할 좌표가 이동 가능하고, 다른 오브젝트가 없는지 체크
            if (Managers.Map.CanGo(destPos))
            {
                GameObject go = Managers.Object.Find(destPos);
                if (go == null)
                {
                    CellPos = destPos;
                }
                else // 어떤 물체에 부딪힌 경우
                {
                    Debug.Log(go.name);
                    // 화살에 맞은 물체의 데미지 메소드를 실행
                    CreatureController cc = go.GetComponent<CreatureController>();
                    if (cc != null)
                        cc.OnDamaged();

                    // 화살 제거
                    Managers.Resource.Destroy(gameObject);
                }
            }
            else // 갈수 없는 위치인 경우
            {
                Managers.Resource.Destroy(gameObject);
            }
        }
    }
}
