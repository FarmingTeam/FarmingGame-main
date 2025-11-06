using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class NPCSaveData
{
    public List<string> DestroyedNpcIDs = new List<string>();
}

[Serializable]
public class NPCPosWrapper
{
    public List<NPCInfo> npcInfos;
}

[Serializable]
public class NPCInfo
{
    public float[] Pos;
    public string NPCID;
}

public class NPCManager : Singleton<NPCManager>
{
    //경로
    const string NPCDATA = "NPCData";
    const string NPCPREFABPATH = "NPCData/NPC/";
    const string NPCINITIALPATH = "NPCDATA/InitialMapNPC/";

    public Dictionary<SceneName, List<NPCInfo>> NPCDictionary = new Dictionary<SceneName, List<NPCInfo>>();
    List<NPC> currentNPCs = new List<NPC>();

    private HashSet<string> destroyedNpcIDs = new HashSet<string>();

    public bool IsInteracting = false;


    public void OnLoadMap(SceneName sceneName)
    {
        if (!NPCDictionary.ContainsKey(sceneName))
        {
            //Initial NPC 위치 불러오기
            NPCDictionary.Add(sceneName, LoadInitialNPCData(sceneName).npcInfos);
        }

        //NPC 배치
        PlaceNPC(sceneName);
    }

    void PlaceNPC(SceneName sceneName)
    {
        Debug.Log("[디버그-PlaceNPC 진입] 현재 삭제 NPC: " + string.Join(",", destroyedNpcIDs));
        currentNPCs.Clear();
        List<NPCInfo> npcinfos = NPCDictionary[sceneName];
        GameObject parent = new GameObject("NPCs");
        for (int i = 0; i < npcinfos.Count; i++)
        {
            Debug.Log($"생성 시도 ID: {npcinfos[i].NPCID}, 삭제 목록 포함 여부: {destroyedNpcIDs.Contains(npcinfos[i].NPCID)}");
            if (destroyedNpcIDs.Contains(npcinfos[i].NPCID))
                continue;

            GameObject instance = null;

            // SellBox만 분기 처리
            if (npcinfos[i].NPCID == "SellBox")
            {
                // SellBox 프리팹 Resources에서 로드 (예: NPCData/SellBox.prefab)
                var sellPrefab = Resources.Load<GameObject>("NPCData/NPC/SellBox");
                if (sellPrefab != null)
                    instance = Instantiate(sellPrefab, parent.transform);
                else
                    Debug.LogError("SellBox 프리팹이 Resources/NPCData/NPC/SellBox 에 존재하지 않습니다.");
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(NPCPREFABPATH).Append(npcinfos[i].NPCID);
                NPC npc = Instantiate(Resources.Load<NPC>(stringBuilder.ToString()), parent.transform);
                npc.transform.position = new Vector3(npcinfos[i].Pos[0], npcinfos[i].Pos[1]);
                currentNPCs.Add(npc);
            }
            if (instance != null)
                instance.transform.position = new Vector3(npcinfos[i].Pos[0], npcinfos[i].Pos[1]);
        }
    }

    public void DestroyNpc(string npcID)
    {
        var npc = currentNPCs.FirstOrDefault(n => n.npcID == npcID);
        if (npc != null)
        {
            destroyedNpcIDs.Add(npcID);
            Debug.Log("NPC 삭제 처리됨, destroyedNpcIDs: " + string.Join(",", destroyedNpcIDs));
            currentNPCs.Remove(npc);
            GameObject.Destroy(npc.gameObject);
        }
    }

    public NPC CanNPCInteract(Vector2 InteractPos)
    {
        for (int i = currentNPCs.Count - 1; i >= 0; i--)
        {
            var npc = currentNPCs[i];
            if (npc == null)
            {
                currentNPCs.RemoveAt(i); // null 참조 정리
                continue;
            }
            if (npc.IsInteractable(InteractPos))
            {
                return npc;
            }
        }
        return null;
    }

    public void SaveCacheToSlot(int slotNumber)
    {
        // 파일 디렉토리 경로 설정
        StringBuilder stringBuilderDirectory = new StringBuilder().Append(Application.persistentDataPath).Append("/")
            .Append(SaveManager.SAVEFILEPATH).Append(slotNumber)
            .Append("/").Append(NPCDATA);
        //디렉토리 생성
        Directory.CreateDirectory(stringBuilderDirectory.ToString());

        //맵 정보 저장
        for (int i = 0; i < NPCDictionary.Count; i++)
        {
            string sceneName = SceneChangeManager.Instance.SCENENAMEDICT[MapDataBase.Instance.mapTileData.Keys.ElementAt(i)].Name;
            //파일 이름 지정
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Application.persistentDataPath).Append("/").Append(SaveManager.SAVEFILEPATH).Append(slotNumber)
                .Append("/").Append(NPCDATA).Append("/").Append(sceneName).Append(NPCDATA).Append(".json");
            //위에서 만든 mapData를 Json으로 변경
            NPCPosWrapper npcPosWrapper = new NPCPosWrapper();
            npcPosWrapper.npcInfos = NPCDictionary.Values.ElementAt(i);
            string NPCInfoString = JsonUtility.ToJson(npcPosWrapper, true);
            //파일에 해당 Json 텍스트를 파일에다 적기
            File.WriteAllText(stringBuilder.ToString(), NPCInfoString);
        }
        SaveDestroyedNpcIDs(slotNumber);
    }

    public void LoadSlotToCache(int slotNumber)
    {
        NPCDictionary.Clear();

        StringBuilder stringBuilder = new StringBuilder();
        //디렉토리 경로
        stringBuilder.Append(Application.persistentDataPath).Append("/").Append(SaveManager.SAVEFILEPATH).Append(slotNumber)
            .Append("/").Append(NPCDATA);

        if (!Directory.Exists(stringBuilder.ToString()))
            return;

        string directorypath = stringBuilder.ToString();

        string[] savedfiles = Directory.GetFiles(directorypath);
        //맵로드
        for (int i = 0; i < savedfiles.Length; i++)
        {
            string filename = Path.GetFileName(savedfiles[i]);
            if (filename == "destroyedNPCs.json")
                continue; // 이 파일은 삭제 ID 목록이므로 스킵

            StringBuilder mapStringBuilder = new StringBuilder();
            mapStringBuilder.Append(Path.GetFileName(savedfiles[i])).Replace(NPCDATA, "").Replace(".json", "");

            //Key
            string mapName = mapStringBuilder.ToString();
            SceneName type = SceneChangeManager.Instance.FindSceneNameByString(mapName);

            //Value
            string npcInfoFile = File.ReadAllText(savedfiles[i]);
            NPCPosWrapper npcInfos = JsonUtility.FromJson<NPCPosWrapper>(npcInfoFile);

            if (NPCDictionary.ContainsKey(type))
                NPCDictionary[type] = npcInfos.npcInfos;
            else
                NPCDictionary.Add(type, npcInfos.npcInfos);
        }
        LoadDestroyedNpcIDs(slotNumber);
    }

    public NPCPosWrapper LoadInitialNPCData(SceneName sceneName)
    {
        StringBuilder InitialFileString = new StringBuilder();
        var initialNPC = Resources.Load(InitialFileString.Append(NPCINITIALPATH)
            .Append(SceneChangeManager.Instance.SCENENAMEDICT[sceneName].Name).Append(NPCDATA).ToString()) as TextAsset;

        return JsonUtility.FromJson<NPCPosWrapper>(initialNPC.text);
    }

    private void SaveDestroyedNpcIDs(int slotNumber)
    {
        NPCSaveData saveData = new NPCSaveData();
        saveData.DestroyedNpcIDs = destroyedNpcIDs.ToList();
        string json = JsonUtility.ToJson(saveData, true);

        string path = Path.Combine(Application.persistentDataPath,
            SaveManager.SAVEFILEPATH + slotNumber + "/NPCData/destroyedNPCs.json");
        File.WriteAllText(path, json);
        Debug.Log("삭제된 NPC ID 목록 저장 완료: " + path);
    }

    private void LoadDestroyedNpcIDs(int slotNumber)
    {
        string path = Path.Combine(Application.persistentDataPath,
            SaveManager.SAVEFILEPATH + slotNumber + "/NPCData/destroyedNPCs.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            NPCSaveData saveData = JsonUtility.FromJson<NPCSaveData>(json);
            destroyedNpcIDs.Clear();
            if (saveData != null && saveData.DestroyedNpcIDs != null)
            {
                foreach (var id in saveData.DestroyedNpcIDs)
                    destroyedNpcIDs.Add(id);
            }
            Debug.Log("삭제된 NPC ID 목록 로드 완료: " + path);
        }
        else
        {
            Debug.LogWarning("삭제된 NPC ID 목록 저장 파일이 없음: " + path);
        }
    }
}
