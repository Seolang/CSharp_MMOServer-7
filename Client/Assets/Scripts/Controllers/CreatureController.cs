using UnityEngine;
using static Define;
using static UnityEngine.UI.CanvasScaler;

// 모든 움직이는 개체에 대한 컨트롤러
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

    public MoveDir Dir // 방향 인스턴스 (메소드 아님)
    {
        get { return _dir; }
        set
        {
            if (_dir == value) // value는 set으로 들어온 값
                return;

            _dir = value; // 마지막에 dir를 갱신하여 갱신 이전 방향에 맞추어 Idle 방향을 정할 수 있음
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
        // 초기 위치를 0, 0 으로 설정
        // CellToWorld : 셀 좌표계를 월드 좌표계로 변환
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        // 개체에 붙어있는 애니메이터를 가져옴
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void UpdateController()
    {
        UpdatePosition();
        UpdateIsMoving();
    }

    // 클라이언트에서 이동 상태에 맞추어 sprite를 이동시키는 함수
    void UpdatePosition()
    {
        if (State != CreatureState.Moving)
            return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position; // 차 벡터

        // 도착 여부 체크
        float dist = moveDir.magnitude; // 벡터 거리

        // 도착지까지 거리가 델타시간에 이동 가능한 거리보다 작을 경우 순간이동 시킨 후 도착 알림
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;

            // 예외적으로 애니메이션을 직접 컨트롤
            _state = CreatureState.Idle;
            if (_dir == MoveDir.None)
            {
                UpdateAnimation();
            }
        }
        else
        {
            // normalized : 단위벡터
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
        }
    }

    // 1칸 씩 움직이며 이동할 수 있는지 확인하는 함수
    void UpdateIsMoving()
    {
        if (State == CreatureState.Idle && _dir != MoveDir.None)
        {
            Vector3Int destPos = _cellPos; // 앞으로 갈 예정인 좌표

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

            if (Managers.Map.CanGo(destPos)) // 가야할 좌표가 이동 가능한지?
            {
                _cellPos = destPos;
                State = CreatureState.Moving;
            }
        }
    }
}
