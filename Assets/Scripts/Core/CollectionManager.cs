
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CollectionManager : Singleton<CollectionManager>
{
    // Start is called before the first frame update
    public List<Collection> collectionsList=new List<Collection>();
    //시작할떄 컬렉션을 로드


    private string JsonPath => Path.Combine(Application.persistentDataPath);
    private const string FILENAME = "collection.Json";


    private string JsonPath_1 => Path.Combine(Application.persistentDataPath);
    private const string FILENAME_1 = "obtainedItems.Json";


    public Action onCollectionChange;

    Dictionary<int,int> obtainedItemDic=new Dictionary<int,int>(); //아이템 아이디랑 얻은 갯수
    void Start()
    {

        //이게 제일 먼저 와야함
        

        GetFromDic();


    }

    //저장 로드해야하는게 1) 컬렉션 상태: 미수령/ 완료but 미수령/완료 후 수령
    //2) 플레이어가 이떄까지 얻었던 아이템 정보

    public void GetFromDic()
    {
        if (collectionsList.Count > 0)
        {
            return;
        }
        for (int i = 0; i < ResourceManager.Instance.collectionDataDic.Count; i++)
        {
            collectionsList.Add(ResourceManager.Instance.collectionDataDic[i]);
        }
    }


    //이걸 아이템 먹는 메서드쪽에 호출, 나중에 컬렉션 오픈할때 
    public void ObtainedItem(int obtainedItemID,int amount)
    {
        if (obtainedItemDic.TryGetValue(obtainedItemID,out int value))
        {
            value += amount;
            obtainedItemDic[obtainedItemID] = value;
        }
        else
        {
            obtainedItemDic.Add(obtainedItemID, amount);
        }
    }

    public void PressGetRewardButton(Collection collection)
    {
        collection.collectionState = 2;
        onCollectionChange?.Invoke();
    }


    public bool IsCompletedCollection(Collection collection)
    {
        int completedItemNum = 0;
        foreach (RequiredItem item in collection.requiredItemList)
        {
            
            if(IsCompletedItem(item))
            {
                completedItemNum++;
            }
            
        }
        if (completedItemNum == collection.requiredItemList.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsCompletedItem(RequiredItem item)
    {
        if (obtainedItemDic.TryGetValue(item.itemID, out int value))
        {
            if (value >= item.amount)
            {
                Debug.Log("완료 아이템");
                return true;
            }
        }
        return false;
    }


    public void SaveCollection(int slotNumber)
    {
        CollectionListWrapper collectionListwrapper = new CollectionListWrapper();
        //컬렉션들을 담기
        for(int i=0;i<collectionsList.Count;i++)
        {
            int id = collectionsList[i].collectionID;
            int collectionstatus = collectionsList[i].collectionState;
            KeyValueParsing keyValueParsing = new KeyValueParsing(id,collectionstatus);
            collectionListwrapper.collectionList.Add(keyValueParsing);
        }

        string json = JsonUtility.ToJson(collectionListwrapper, true);

        StringBuilder savePath = new StringBuilder();
        savePath.Append(JsonPath).Append('/').Append(SaveManager.SAVEFILEPATH).Append(slotNumber).Append('/').Append(FILENAME);

        File.WriteAllText(savePath.ToString(), json);

        Debug.Log("컬렉션 저장 완료: " + JsonPath);
    }

    public void LoadCollection(int slotNumber)
    {
        StringBuilder loadPath = new StringBuilder();
        loadPath.Append(JsonPath).Append('/').Append(SaveManager.SAVEFILEPATH).Append(slotNumber).Append('/').Append(FILENAME);
        if (File.Exists(loadPath.ToString()))
        {

            Debug.Log("콜렉션 파일 있음");
            string json = File.ReadAllText(loadPath.ToString());

            CollectionListWrapper collectionListWrapper = JsonUtility.FromJson<CollectionListWrapper>(json);


            GetFromDic(); //일단 먼저 로드함
            for (int i = 0; i < collectionListWrapper.collectionList.Count; i++)
            {
                collectionsList[i].collectionID= collectionListWrapper.collectionList[i].key;
                collectionsList[i].collectionState = collectionListWrapper.collectionList[i].value;
            }

        }
        
    }

    public void SaveObtainedItem(int slotNumber)
    {
        ObtainedItemListWrapper obtainedItemListWrapper = new ObtainedItemListWrapper();
        //컬렉션들을 담기
        foreach(var dic in obtainedItemDic)
        {
            
            KeyValueParsing keyValueParsing = new KeyValueParsing(dic.Key, dic.Value);
            obtainedItemListWrapper.obtainedItemList.Add(keyValueParsing);
        }
        

        string json = JsonUtility.ToJson(obtainedItemListWrapper, true);

        StringBuilder savePath = new StringBuilder();
        savePath.Append(JsonPath_1).Append('/').Append(SaveManager.SAVEFILEPATH).Append(slotNumber).Append('/').Append(FILENAME_1);

        File.WriteAllText(savePath.ToString(), json);

        Debug.Log("모은 아이템 저장 완료: " + JsonPath_1);
    }

    public void LoadObtainedItem(int slotNumber)
    {
        StringBuilder loadPath = new StringBuilder();
        loadPath.Append(JsonPath_1).Append('/').Append(SaveManager.SAVEFILEPATH).Append(slotNumber).Append('/').Append(FILENAME_1);
        if (File.Exists(loadPath.ToString()))
        {

            Debug.Log("획득 아이템 파일 있음");
            string json = File.ReadAllText(loadPath.ToString());

            ObtainedItemListWrapper obtainedItemListWrapper = JsonUtility.FromJson<ObtainedItemListWrapper>(json);


            
            for (int i = 0; i < obtainedItemListWrapper.obtainedItemList.Count; i++)
            {
                ObtainedItem(obtainedItemListWrapper.obtainedItemList[i].key, obtainedItemListWrapper.obtainedItemList[i].value);
            }

        }
    }

    
}

[Serializable]
public class CollectionListWrapper
{
    public List<KeyValueParsing> collectionList = new List<KeyValueParsing>();
}


[Serializable]
public class ObtainedItemListWrapper
{
    public List<KeyValueParsing> obtainedItemList=new List<KeyValueParsing>();
}


[Serializable]
public class KeyValueParsing
{
    public int key;
    public int value;
    public KeyValueParsing(int key, int value)
    {
        this.key = key;
        this.value = value;

    }
}
