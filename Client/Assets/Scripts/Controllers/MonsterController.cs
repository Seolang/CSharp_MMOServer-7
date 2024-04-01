using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    protected override void Init()
    {
        base.Init();
        State = CreatureState.Idle;
        Dir = MoveDir.None;
    }

    protected override void UpdateController()
    {
        //GetDirectionInput();
        base.UpdateController();
    }

    // Ű���� �Է��� �޾� ���� ���¸� �����ϴ� �޼ҵ�
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

    public override void OnDamaged()
    {

        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f); // 0.5�� �� ��� ����Ʈ �Ҹ�

        Managers.Object.Remove(gameObject); // ������Ʈ �Ŵ������� ����
        Managers.Resource.Destroy(gameObject); // ���� ������Ʈ�� �Ҹ�
    }
}
