using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 인게임 씬이 시작할 때 필요한 동작을 제어하는 스크립트
public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.Map.LoadMap(1); // 1번 맵 로드

        // 플레이어 생성 후 ObjectManager에 추가
        GameObject player = Managers.Resource.Instantiate("Creature/Player");
        player.name = "Player";
        Managers.Object.Add(player);

        for (int i = 0; i < 5; i++)
        {
            // 몬스터 생성 후 ObjectManager에 추가
            GameObject monster = Managers.Resource.Instantiate("Creature/Monster");
            monster.name = $"Monster_{i + 1}";

            // 랜덤 위치 지정
            Vector3Int pos = new Vector3Int()
            {
                x = Random.Range(-20, 20),
                y = Random.Range(-10, 10),
            };

            MonsterController mc = monster.GetComponent<MonsterController>();
            mc.CellPos = pos; // 몬스터의 초기 위치를 랜덤 위치로 이동

            Managers.Object.Add(monster);
        }

        //Managers.UI.ShowSceneUI<UI_Inven>();
        //Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        //gameObject.GetOrAddComponent<CursorController>();

        //GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        //Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player);

        ////Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
        //GameObject go = new GameObject { name = "SpawningPool" };
        //SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        //pool.SetKeepMonsterCount(2);
    }

    public override void Clear()
    {
        
    }
}
