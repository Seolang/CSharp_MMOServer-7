using Google.Protobuf.Protocol;
using UnityEngine;

public class ArrowController : CreatureController
{
    protected override void Init()
    {
        // 화살 스프라이트 방향 설정
        switch (Dir)
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

        State = CreatureState.Moving;
        _speed = 15.0f;

        base.Init();
    }

    protected override void UpdateAnimation()
    {
        // Empty
    }

    protected override void MoveToNextPosition()
    {

    }
}
