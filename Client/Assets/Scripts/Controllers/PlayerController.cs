using System;
using UnityEngine;
using static Define;

// 플레이어가 입력 한번에 셀 1칸씩 이동하는 컨트롤러
public class PlayerController : MonoBehaviour
{
    public Grid _grid;
    public float _speed = 5.0f;


    Vector3Int _cellPos = Vector3Int.zero;
    MoveDir _dir = MoveDir.None;
    bool _isMoving = false;

    void Start()
    {
        // 초기 위치를 0, 0 으로 설정
        // CellToWorld : 셀 좌표계를 월드 좌표계로 변환
        Vector3 pos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    void Update()
    {
        GetDirectionInput();
        UpdatePosition();
        UpdateIsMoving();
    }

    // 키보드 입력을 받아 방향 상태를 변경하는 메소드
    void GetDirectionInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            // deltaTime을 곱해주는 이유는 기기 성능에 따른 프레임 차이에 의해 속도가 달라지는 것을 방지한다.
            // 멀티플레이 게임에서는 입력받는 대로 바로 이동시키는 것은 설계 상 좋지 않다.
            //transform.position += Vector3.up * Time.deltaTime * _speed;
            _dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //transform.position += Vector3.down * Time.deltaTime * _speed;
            _dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            //transform.position += Vector3.left * Time.deltaTime * _speed;
            _dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //transform.position += Vector3.right * Time.deltaTime * _speed;
            _dir = MoveDir.Right;
        }
        else
        {
            _dir = MoveDir.None;
        }
    }

    // 클라이언트에서 이동 상태에 맞추어 sprite를 움직이는 함수
    void UpdatePosition()
    {
        if (_isMoving == false)
            return;

        Vector3 destPos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position; // 차 벡터

        // 도착 여부 체크
        float dist = moveDir.magnitude; // 벡터 거리

        // 도착지까지 거리가 델타시간에 이동 가능한 거리보다 작을 경우 순간이동 시킨 후 도착 알림
        if (dist < _speed * Time.deltaTime) 
        {
            transform.position = destPos;
            _isMoving = false;
        }
        else
        {
            // normalized : 단위벡터
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            _isMoving = true;
        }
    }

    // 1칸 씩 움직이도록 이동 타이밍 조정 함수
    void UpdateIsMoving()
    {
        if (_isMoving == true)
            return;

        switch (_dir)
        {
            case MoveDir.Up:
                _cellPos += Vector3Int.up;
                _isMoving = true;
                break;

            case MoveDir.Down:
                _cellPos += Vector3Int.down;
                _isMoving = true;
                break;

            case MoveDir.Left:
                _cellPos += Vector3Int.left;
                _isMoving = true;
                break;

            case MoveDir.Right:
                _cellPos += Vector3Int.right;
                _isMoving = true;
                break;
        }
    }


}
