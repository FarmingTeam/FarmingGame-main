
using System.Collections.Generic;
using UnityEngine;

public static class CsvQuestParser
{
    public static List<QuestData> ParseQuestCsv(TextAsset csvFile)
    {
        var questList = new List<QuestData>();
        if (csvFile == null)
            return questList;
        var lines = csvFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
            var cols = ParseCsvLine(line);
            if (cols.Length < 8 || cols[0].Trim() == "QuestID") continue;

            var quest = new QuestData
            {
                QuestId = cols[0].Trim(),
                QuestTitle = cols[1].Trim(),
                QuestReward = cols[2].Trim(),
                QuestGiverNpcId = cols[3].Trim(),
                PreQuestId = cols[4].Trim(),
                Status = QuestStatus.Locked,
                Conditions = new List<QuestConditionBase>()
            };

            string action = cols[5].Trim();
            string target = cols[6].Trim();
            int count = 1;
            int.TryParse(cols[7].Trim(), out count);

            if (!string.IsNullOrEmpty(action))
            {
                if (action == "Talk")
                    quest.Conditions.Add(new TalkQuestCondition(target, count));
                else if (action == "Deliver")
                    quest.Conditions.Add(new DeliverQuestCondition(QuestParsingUtil.ParseItemIDs(target), count, quest.QuestGiverNpcId));
            }

            if (!string.IsNullOrEmpty(target))
            {
                // "ItemID:1101~1106" 또는 "1101~1106" 둘 다 처리
                if (target.StartsWith("ItemID:", System.StringComparison.OrdinalIgnoreCase))
                {
                    quest.DeductionTarget = target;
                    quest.DeductionCount = count;
                }
                else if (action == "Deliver") 
                {
                    quest.DeductionTarget = "ItemID:" + target;
                    quest.DeductionCount = count;
                }

            }
            questList.Add(quest);
        }
        return questList;
    }

    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        string current = "";
        foreach (var c in line)
        {
            if (c == '"') inQuotes = !inQuotes;
            else if (c == ',' && !inQuotes)
            {
                result.Add(current); current = "";
            }
            else current += c;
        }
        result.Add(current);
        return result.ToArray();
    }
}
