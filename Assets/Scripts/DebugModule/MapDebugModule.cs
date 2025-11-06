using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDebugModule : MonoBehaviour
{
    [SerializeField] Tilemap BigTreemap;
    [SerializeField] Tilemap SmallTreemap;


    private void Start()
    {
        StringBuilder Bigpath = new StringBuilder().Append(Application.persistentDataPath).Append("/").Append("BigTree.txt");
        StreamWriter Bigwriter = new StreamWriter(Bigpath.ToString());

        StringBuilder Smallpath = new StringBuilder().Append(Application.persistentDataPath).Append("/").Append("SmallTree.txt");
        StreamWriter Smallwriter = new StreamWriter(Smallpath.ToString());
        for (int x = 0; x < MapControl.Instance.map.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < MapControl.Instance.map.tiles.GetLength(1); y++)
            {
                if (BigTreemap.GetTile(new Vector3Int(x, y)) != null)
                    Bigwriter.WriteLine(MakeToJsonFormat(x,y, "55"));
                if (SmallTreemap.GetTile(new Vector3Int(x,y)) != null)
                    Smallwriter.WriteLine(MakeToJsonFormat(x,y,"43"));

            }
        }
        Bigwriter.Close();
        Smallwriter.Close();
    }

    string MakeToJsonFormat(int x, int y, string type)
    {
        string writer = "{ \n \"Pos\": [ \n" + x.ToString() + ",\n" + y.ToString() + "\n ], \n \"ChunkType\" : " + type + "\n }, \n";
        return writer;
    }
}
