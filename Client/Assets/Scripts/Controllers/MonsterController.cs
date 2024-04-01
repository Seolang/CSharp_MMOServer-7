using System.Collections;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine _coPatrol;
    Vector3Int _destCellPos;

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
        }
    }

    protected override void Init()
    {
        base.Init();
        State = CreatureState.Idle;
        Dir = MoveDir.None;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if (_coPatrol == null)
        {
            _coPatrol = StartCoroutine("CoPatrol");
        }
    }

    protected override void MoveToNextPosition()
    {
        // 이동해야할 다음 위치를 한칸 씩 지정
        Vector3Int moveCellDir = _destCellPos - CellPos;
        if (moveCellDir.x > 0)
            Dir = MoveDir.Right;
        else if(moveCellDir.x < 0)
            Dir = MoveDir.Left;
        else if (moveCellDir.y > 0)
            Dir = MoveDir.Up;
        else if (moveCellDir.y < 0)
            Dir = MoveDir.Down;
        else
            Dir = MoveDir.None;


        Vector3Int destPos = CellPos; // 목표 좌표 = 현재 좌표

        // 방향에 맞게 목표 좌표를 한칸 이동
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

        // 가야할 좌표가 이동 가능하고, 다른 오브젝트가 없는지 체크
        if (Managers.Map.CanGo(destPos) && Managers.Object.Find(destPos) == null)
        {
            CellPos = destPos;
        }
        else {
            State = CreatureState.Idle;
        }
    }

    // 공격 받을 시 피격 처리 메소드
    public override void OnDamaged()
    {

        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f); // 0.5초 후 사망 이펙트 소멸

        Managers.Object.Remove(gameObject); // 오브젝트 매니저에서 삭제
        Managers.Resource.Destroy(gameObject); // 게임 오브젝트를 소멸
    }

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
}
