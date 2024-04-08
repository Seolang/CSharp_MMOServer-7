using Google.Protobuf.Protocol;
using UnityEngine;

public class MyPlayerController : PlayerController
{
    protected override void Init()
    {
        base.Init();
    }

    private void LateUpdate()
    {
        // 카메라 위치 업데이트 (카메라는 update 이후 호출되는 lateUpdate에서 주로 사용한다)
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // 입력값을 갱신 하는 메소드
    protected override void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                GetDirectionInput();
                break;
            case CreatureState.Moving:
                GetDirectionInput();
                break;

        }
        base.UpdateController();
    }

    // 키보드 입력을 받아 방향을 변경하는 메소드
    void GetDirectionInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            // deltaTime을 곱해주는 이유는 기기 성능에 따른 프레임 차이에 의해 속도가 달라지는 것을 방지한다.
            // 멀티플레이 게임에서는 입력받는 대로 바로 이동시키는 것은 설계 상 좋지 않다.
            //transform.position += Vector3.up * Time.deltaTime * _speed;
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //transform.position += Vector3.down * Time.deltaTime * _speed;
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            //transform.position += Vector3.left * Time.deltaTime * _speed;
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //transform.position += Vector3.right * Time.deltaTime * _speed;
            Dir = MoveDir.Right;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }

    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if (Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            _coSkill = StartCoroutine("CoStartPunch");

        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            State = CreatureState.Skill;
            _coSkill = StartCoroutine("CoStartShootArrow");
        }
    }

    protected override void MoveToNextPosition()
    {
        CreatureState prevState = State;
        Vector3Int prevCellPos = CellPos;

        base.MoveToNextPosition();

        if (prevState != State || prevCellPos != CellPos)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
        }
    }
}
