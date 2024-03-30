using System;
using System.Collections;
using UnityEngine;
using static Define;

// �÷��̾ �Է� �ѹ��� �� 1ĭ�� �̵��ϴ� ��Ʈ�ѷ�
public class PlayerController : CreatureController
{
    Coroutine _coSkill;

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                GetDirectionInput();
                GetIdleInput();
                break;
            case CreatureState.Moving:
                GetDirectionInput();
                break;

        }
        base.UpdateController();
    }

    private void LateUpdate()
    {
        // ī�޶� ��ġ ������Ʈ (ī�޶�� update ���� ȣ��Ǵ� lateUpdate���� �ַ� ����Ѵ�)
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
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

    // Idle ���¿����� Ű �Է��� �޴� �޼ҵ�
    void GetIdleInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            _coSkill = StartCoroutine("CoStartPunch");

        }
    }

    // ��ġ ��ų �ڷ�ƾ �޼ҵ�
    IEnumerator CoStartPunch()
    {
        // �ǰ� ����
        GameObject go = Managers.Object.Find(GetFrontCellPosition());
        if (go != null)
        {
            Debug.Log(go.name);
        }


        // ��� �ð�
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;
    }
}
