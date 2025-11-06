using UnityEngine;

public class MapControl : Singleton<MapControl>
{
    public Player player;
    public Map map;

    public void OnSceneEnd()
    {
        player = null;
        map = null;
    }
}
