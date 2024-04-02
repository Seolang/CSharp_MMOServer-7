using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine _coSkill;
    Coroutine _coPatrol;
    Coroutine _coSearch;

    [SerializeField]
    Vector3Int _destCellPos;

    [SerializeField]
    GameObject _target;

    [SerializeField]
    float _searchRange = 10.0f;
    [SerializeField]
    float _skillRange = 1.0f;

    [SerializeField]
    bool _rangedSkill = false;

    public override CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value) return;

            base.State = value;

            if (_coPatrol != null)
            {
                StopCoroutine(_coPatrol);
                _coPatrol = null;
            }

            if (_coSearch != null)
            {
                StopCoroutine(_coSearch);
                _coSearch = null;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        State = CreatureState.Idle;
        Dir = MoveDir.None;
        _speed = 3.0f;
        _rangedSkill = (Random.Range(0, 2) == 0 ? true : false);
        if (_rangedSkill)
            _skillRange = 10.0f;
        else
            _skillRange = 1.0f;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if (_coPatrol == null)
        {
            _coPatrol = StartCoroutine("CoPatrol");
        }

        if (_coSearch == null)
        {
            _coSearch = StartCoroutine("CoSearch");
        }
    }

    protected override void MoveToNextPosition()
    {
        Vector3Int destPos = _destCellPos;

        if (_target != null)
        {
            destPos = _target.GetComponent<CreatureController>().CellPos;

            Vector3Int dir = destPos - CellPos;
            if (dir.magnitude <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Skill;

                if (_rangedSkill)
                    _coSkill = StartCoroutine("CoStartShootArrow");
                else
                    _coSkill = StartCoroutine("CoStartPunch");
                return;
            }
        }

        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);

        if (path.Count < 2 || (_target != null &&path.Count > 20)) // 길을 못 찾은 경우 || 타겟이 너무 멀리 간 경우
        {
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];
        Vector3Int moveCellDir = nextPos - CellPos;

        Dir = GetDirFromVec(moveCellDir);

        // 가야할 좌표가 이동 가능하고, 다른 오브젝트가 없는지 체크
        if (Managers.Map.CanGo(nextPos) && Managers.Object.Find(nextPos) == null)
        {
            CellPos = nextPos;
        }
        else {
            State = CreatureState.Idle;
        }
    }

    // 공격 받을 시 피격 처리 메소드
    public override void OnDamaged()
    {
        base.OnDamaged();

        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f); // 0.5초 후 사망 이펙트 소멸

        Managers.Object.Remove(gameObject); // 오브젝트 매니저에서 삭제
        Managers.Resource.Destroy(gameObject); // 게임 오브젝트를 소멸
    }

    // 패트롤 코루틴 메소드
    IEnumerator CoPatrol()
    {
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);

        for (int i = 0; i < 10; i++)
        {
            int xRange = Random.Range(-5, 6);
            int yRange = Random.Range(-5, 6);
            Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            if (Managers.Map.CanGo(randPos) && Managers.Object.Find(randPos) == null)
            {
                _destCellPos = randPos;
                State = CreatureState.Moving;
                yield break; // 코루틴 종료
            }
        }

        State = CreatureState.Idle;
    }

    // 범위 내 플레이어 탐색 코루틴 메소드
    IEnumerator CoSearch()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (_target != null)
                continue;

            _target = Managers.Object.Find((go) =>
            {
                PlayerController pc = go.GetComponent<PlayerController>();
                if (pc == null) 
                    return false;

                Vector3Int dir = (pc.CellPos - CellPos);
                if (dir.magnitude > _searchRange)
                    return false;

                return true;
            });
        }
    }

    // 펀치 스킬 코루틴 메소드
    IEnumerator CoStartPunch()
    {
        // 피격 판정
        GameObject go = Managers.Object.Find(GetFrontCellPosition());
        if (go != null)
        {
            // 피격 상대의 데미지 메소드 실행
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
                cc.OnDamaged();
        }

        // 대기 시간
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Moving;
        _coSkill = null;
    }

    // 화살 스킬 코루틴 메소드
    IEnumerator CoStartShootArrow()
    {
        // 화살 생성
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        ac.Dir = _lastDir; // 화살이 나아갈 방향 설정
        ac.CellPos = CellPos; // 화살 초기 위치 설정

        // 대기 시간
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Moving;
        _coSkill = null;
    }
}
