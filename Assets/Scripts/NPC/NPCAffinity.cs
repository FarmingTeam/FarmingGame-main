
using System.Collections.Generic;
using UnityEngine;

public static class NPCAffinity
{
    public const int MaxFavor = 100;
    private static Dictionary<string, int> favorDict = new Dictionary<string, int>();

    public static void AddFavor(string npcId, int amount)
    {
        if (!favorDict.ContainsKey(npcId))
            favorDict[npcId] = 0;

        favorDict[npcId] += amount;
        favorDict[npcId] = Mathf.Clamp(favorDict[npcId], 0, MaxFavor);
    }

    public static int GetFavor(string npcId)
    {
        if (!favorDict.ContainsKey(npcId))
            return 0;
        return favorDict[npcId];
    }
}
