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
        // ī�޶� ��ġ ������Ʈ (ī�޶�� update ���� ȣ��Ǵ� lateUpdate���� �ַ� ����Ѵ�)
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // �Է°��� ���� �ϴ� �޼ҵ�
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

    // Ű���� �Է��� �޾� ������ �����ϴ� �޼ҵ�
    void GetDirectionInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            // deltaTime�� �����ִ� ������ ��� ���ɿ� ���� ������ ���̿� ���� �ӵ��� �޶����� ���� �����Ѵ�.
            // ��Ƽ�÷��� ���ӿ����� �Է¹޴� ��� �ٷ� �̵���Ű�� ���� ���� �� ���� �ʴ�.
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
        // �̵� ���·� ���� Ȯ��
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
