using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ChunkControl : Singleton<ChunkControl>
{
    //타일 초기화
    public List<Vector2Int> SetTileObjectInMap(Vector2Int startPos, ChunkData chunkData)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        List<Vector2Int> connected = new List<Vector2Int>();
        connected.Add(startPos);
        //연결된 오브젝트 그래프 분리 및 재할당
        if (MapControl.Instance.map.objectGraph.ContainsKey(startPos))
        {
            for (int i = 0; i < MapControl.Instance.map.objectGraph[startPos].Count; i++)
            {
                Vector2Int element = MapControl.Instance.map.objectGraph[startPos][i];
                MapControl.Instance.map.objectGraph.Remove(element);
                connected.Add(element);
            }
            MapControl.Instance.map.objectGraph.Remove(startPos);
        }
        //타일 재설정
        for (int i = 0; i < connected.Count; i++)
        {
            if (UpdateTiles(connected[i], chunkData))
            {
                result.Add(connected[i]);
            }
        }
        //세이브 데이터 변경을 위해 실제 저장할 데이터만 리턴
        return result;
    }

    //변경사항 반영 로직
     bool UpdateTiles(Vector2Int startPos, ChunkData chunkData)
    {
        if (MapControl.Instance.map.objectGraph.ContainsKey(startPos))
            return false;

        List<Vector2Int> curentData = new List<Vector2Int>();

        for (int x = 0; x < chunkData.tileBases.Length; x++)
        {
            for (int y = 0; y < chunkData.tileBases[x].Tiles.Length; y++)
            {
                int curx = startPos.x + x;
                int cury = startPos.y + y;
                curentData.Add(new Vector2Int(curx, cury));
            }
        }

        for (int x = 0; x < chunkData.tileBases.Length; x++)
        {
            for (int y = 0; y < chunkData.tileBases[x].Tiles.Length; y++)
            {
                int curx = startPos.x + x;
                int cury = startPos.y + y;
                List<Vector2Int> connectPos = curentData.ToList<Vector2Int>();
                connectPos.Remove(new Vector2Int(curx, cury));

                MapControl.Instance.map.tiles[curx, cury].chunkData = chunkData;
                MapControl.Instance.map.objectGraph.Add(new Vector2Int(curx, cury), connectPos);
                int layer = chunkData.interactionType[y] == ChunkInteractionType.None ? 4 : 2;
                MapControl.Instance.map.SetTileObject(new Vector2Int(curx, cury), chunkData.tileBases[x].Tiles[y], layer);
            }
        }
        return true;
    }
}
