using System.IO;
using UnityEngine;

// 맵을 관리하는 매니저 정의
public class MapManager
{
    public Grid CurrentGrid { get; private set; }

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    bool[,] _collision; // 2차원 배열

    // 갈 수 있는 좌표인지 결정하는 메소드
    public bool CanGo(Vector3Int cellPos)
    {
        if (cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y; // Y값은 배열에서 뒤집혀있기 때문
        return !_collision[y, x]; // true가 collision이 있는 위치
    }

    public void LoadMap(int mapId)
    {
        DestroyMap(); // 기존에 있던 맵을 해제

        // mapId에 해당하는 맵을 가져옴
        string mapName = "Map_" + mapId.ToString("000"); // 00x 포맷 지정
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}");
        go.name = "Map";

        // Collision 정보를 가진 오브젝트 활성 해제
        GameObject collision = Util.FindChild(go, "Tilemap_Collision", true);
        if (collision != null )
            collision.SetActive(false);

        CurrentGrid = go.GetComponent<Grid>(); // 생성한 맵의 그리드 객체를 매니저에 할당

        // Collision 관련 파일
        TextAsset txt = Managers.Resource.Load<TextAsset>($"Map/{mapName}"); // txt파일 로드
        StringReader reader = new StringReader(txt.text);

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());

        int xCount = MaxX - MinX + 1;
        int yCount = MaxY - MinY + 1;
        _collision = new bool[yCount, xCount];

        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for(int x = 0;  x < xCount; x++)
            {
                _collision[y, x] = (line[x] == '1' ? true : false);
            }
        }
    }

    public void DestroyMap() 
    {
        GameObject map = GameObject.Find("Map");
        if (map != null)
        {
            GameObject.Destroy(map);
            CurrentGrid = null;
        }
    }
}
