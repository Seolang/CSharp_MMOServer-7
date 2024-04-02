using UnityEngine;
using static Define;
using static UnityEngine.UI.CanvasScaler;

// ��� �����̴� ��ü�� ���� ��Ʈ�ѷ�
public class CreatureController : MonoBehaviour
{
    // �ӵ�
    [SerializeField]
    public float _speed = 5.0f;

    // ��ġ
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;

    // �ִϸ��̼�
    protected Animator _animator;

    // ��������Ʈ
    protected SpriteRenderer _sprite;

    // ����
    [SerializeField]
    protected CreatureState _state = CreatureState.Idle;
    public virtual CreatureState State
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

    // �̵� ����
    protected MoveDir _lastDir = MoveDir.Down;
    [SerializeField]
    protected MoveDir _dir = MoveDir.Down;
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

    public MoveDir GetDirFromVec(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDir.Right;
        else if (dir.x < 0)
            return MoveDir.Left;
        else if (dir.y > 0)
            return MoveDir.Up;
        else if (dir.y < 0)
            return MoveDir.Down;
        else
            return MoveDir.None;
    }


    // �ٶ󺸴� ���� �ٷ� ���� ��ġ�� ��ȯ�ϴ� �޼ҵ�
    public Vector3Int GetFrontCellPosition()
    {
        Vector3Int cellPos = CellPos;

        switch (_lastDir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
        }

        return cellPos;
    }

    // ���¿� ���� �ִϸ��̼��� �����ϴ� �޼ҵ�
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
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("ATTACK_BACK");
                    _sprite.flipX = false;
                    break;

                case MoveDir.Down:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;

                case MoveDir.Left:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = true;
                    break;

                case MoveDir.Right:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
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
        // ��ü�� �پ��ִ� �ִϸ����͸� ������
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();

        // �ʱ� ��ġ�� 0, 0 ���� ����
        // CellToWorld : �� ��ǥ�踦 ���� ��ǥ��� ��ȯ
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    protected virtual void UpdateController()
    {
        switch (State) // ���º��� ó��
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }


    protected virtual void UpdateIdle()
    {

    }


    protected virtual void UpdateMoving()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position; // �� ����

        // ���� ���� üũ
        float dist = moveDir.magnitude; // ���� �Ÿ�

        // ���� �̵� ���� Ž��
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPosition();
        }
        else // ��������Ʈ �̵�
        {
            // normalized : ��������
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
        }
    }

    protected virtual void MoveToNextPosition()
    {
        // �̵� ������ ������ ����
        if (_dir == MoveDir.None)
        {
            State = CreatureState.Idle;
            return;
        }

        // 1ĭ �� �����̸� �̵��� �� �ִ��� Ȯ��
        // ������ �����ϰ� �̵� ������ ������ ��, ���� ��ǥ�� �̵��Ѵ�
        Vector3Int destPos = CellPos; // ��ǥ ��ǥ

        // ���⿡ �°� ��ǥ ��ǥ�� ��ĭ �̵�
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

        // ������ ��ǥ�� �̵� �����ϰ�, �ٸ� ������Ʈ�� ������ üũ
        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.Find(destPos) == null)
            {
                CellPos = destPos;
            }
        }
    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

    }

    public virtual void OnDamaged()
    {
        Debug.Log($"{gameObject.name} hit!");
    }
}
