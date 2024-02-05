using UnityEngine;
using static Define;
using static UnityEngine.UI.CanvasScaler;

// ��� �����̴� ��ü�� ���� ��Ʈ�ѷ�
public class CreatureController : MonoBehaviour
{
    public float _speed = 5.0f;

    protected Vector3Int _cellPos = Vector3Int.zero;
    protected Animator _animator;
    protected SpriteRenderer _sprite;

    CreatureState _state = CreatureState.Idle;

    public CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;
            UpdateAnimation();
        }
    }

    MoveDir _lastDir = MoveDir.Down;
    MoveDir _dir = MoveDir.Down;

    public MoveDir Dir // ���� �ν��Ͻ� (�޼ҵ� �ƴ�)
    {
        get { return _dir; }
        set
        {
            if (_dir == value) // value�� set���� ���� ��
                return;

            _dir = value; // �������� dir�� �����Ͽ� ���� ���� ���⿡ ���߾� Idle ������ ���� �� ����
            if (value != MoveDir.None)
                _lastDir = value;

            UpdateAnimation();
        }
    }

    protected virtual void UpdateAnimation()
    {
        if (_state == CreatureState.Idle)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;

                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    break;

                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = true;
                    break;

                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (_state == CreatureState.Moving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;

                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;

                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = true;
                    break;

                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (_state == CreatureState.Skill)
        {
            //TODO
        }
        else
        {

        }
    }

    void Start()
    {
        Init();

    }

    void Update()
    {
        UpdateController();
    }

    protected virtual void Init()
    {
        // �ʱ� ��ġ�� 0, 0 ���� ����
        // CellToWorld : �� ��ǥ�踦 ���� ��ǥ��� ��ȯ
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        // ��ü�� �پ��ִ� �ִϸ����͸� ������
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void UpdateController()
    {
        UpdatePosition();
        UpdateIsMoving();
    }

    // Ŭ���̾�Ʈ���� �̵� ���¿� ���߾� sprite�� �̵���Ű�� �Լ�
    void UpdatePosition()
    {
        if (State != CreatureState.Moving)
            return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position; // �� ����

        // ���� ���� üũ
        float dist = moveDir.magnitude; // ���� �Ÿ�

        // ���������� �Ÿ��� ��Ÿ�ð��� �̵� ������ �Ÿ����� ���� ��� �����̵� ��Ų �� ���� �˸�
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;

            // ���������� �ִϸ��̼��� ���� ��Ʈ��
            _state = CreatureState.Idle;
            if (_dir == MoveDir.None)
            {
                UpdateAnimation();
            }
        }
        else
        {
            // normalized : ��������
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
        }
    }

    // 1ĭ �� �����̸� �̵��� �� �ִ��� Ȯ���ϴ� �Լ�
    void UpdateIsMoving()
    {
        if (State == CreatureState.Idle && _dir != MoveDir.None)
        {
            Vector3Int destPos = _cellPos; // ������ �� ������ ��ǥ

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

            if (Managers.Map.CanGo(destPos)) // ������ ��ǥ�� �̵� ��������?
            {
                _cellPos = destPos;
                State = CreatureState.Moving;
            }
        }
    }
}
