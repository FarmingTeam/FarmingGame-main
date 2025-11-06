
using System.Collections.Generic;
using UnityEngine;

public static class QuestParsingUtil
{
    public static List<int> ParseItemIDs(string data)
    {
        List<int> ids = new List<int>();
        if (string.IsNullOrWhiteSpace(data)) return ids;
        if (data.Contains("~"))
        {
            var tokens = data.Split('~');
            int start, end;
            if (tokens.Length != 2 ||
                !int.TryParse(tokens[0].Trim(), out start) ||
                !int.TryParse(tokens[1].Trim(), out end))
            {
                Debug.LogError($"ParseItemIDs 잘못된 범위: {data}");
                return ids;
            }
            for (int i = start; i <= end; i++) ids.Add(i);
        }
        else if (data.Contains(","))
        {
            foreach (var t in data.Split(','))
            {
                int id;
                if (int.TryParse(t.Trim(), out id))
                    ids.Add(id);
                else
                    Debug.LogError($"ParseItemIDs 잘못된 아이템 ID: {t}");
            }
        }
        else
        {
            int id;
            if (int.TryParse(data.Trim(), out id))
                ids.Add(id);
            else
                Debug.LogError($"ParseItemIDs 잘못된 단일 값: {data}");
        }
        return ids;
    }
}