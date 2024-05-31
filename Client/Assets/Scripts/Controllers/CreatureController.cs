using Google.Protobuf.Protocol;
using UnityEngine;

// 모든 움직이는 개체에 대한 컨트롤러
public class CreatureController : MonoBehaviour
{
    // 개체 ID
    public int Id { get; set; }

    // 속도
    [SerializeField]
    public float _speed = 5.0f;

    protected bool _updated = false;

    PositionInfo _positionInfo = new PositionInfo();
    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set 
        {
            if (_positionInfo.Equals(value))
                return;

            CellPos = new Vector3Int(value.PosX, value.PosY, 0);
            State = value.State;
            Dir = value.MoveDir;
        }
    }

    // 위치와 스프라이트 동기화
    public void SyncPos()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = destPos;
    }

    // 위치
    public Vector3Int CellPos 
    {
        get 
        {
            return new Vector3Int(PosInfo.PosX, PosInfo.PosY, 0);
        }
        set 
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y)
                return;

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            _updated = true;
        }
    }

    // 애니메이션
    protected Animator _animator;

    // 스프라이트
    protected SpriteRenderer _sprite;

    public virtual CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
            _updated = true;
            UpdateAnimation();
        }
    }

    public MoveDir Dir // 방향 인스턴스 (메소드 아님)
    {
        get { return PosInfo.MoveDir; }
        set
        {
            if (PosInfo.MoveDir == value) // value는 set으로 들어온 값
                return;

            PosInfo.MoveDir = value; // 마지막에 dir를 갱신하여 갱신 이전 방향에 맞추어 Idle 방향을 정할 수 있음

            UpdateAnimation();
            _updated = true;
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
        else
            return MoveDir.Down;
    }


    // 바라보는 방향 바로 앞의 위치를 반환하는 메소드
    public Vector3Int GetFrontCellPosition()
    {
        Vector3Int cellPos = CellPos;

        switch (Dir)
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

    // 상태에 따라 애니메이션을 조절하는 메소드
    protected virtual void UpdateAnimation()
    {
        if (State == CreatureState.Idle)
        {
            switch (Dir)
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
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
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
        else if (State == CreatureState.Skill)
        {
            switch (Dir)
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
        // 개체에 붙어있는 애니메이터를 가져옴
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();

        // 초기 위치를 0, 0 으로 설정
        // CellToWorld : 셀 좌표계를 월드 좌표계로 변환
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        State = CreatureState.Idle;
        Dir = MoveDir.Down;
        UpdateAnimation();
    }

    protected virtual void UpdateController()
    {
        switch (State) // 상태별로 처리
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
        Vector3 moveDir = destPos - transform.position; // 차 벡터

        // 도착 여부 체크
        float dist = moveDir.magnitude; // 벡터 거리

        // 다음 이동 지점 탐색
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPosition();
        }
        else // 스프라이트 이동
        {
            // normalized : 단위벡터
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
        }
    }

    protected virtual void MoveToNextPosition()
    {

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
