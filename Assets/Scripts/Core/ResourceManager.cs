using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ResourceManager : Singleton<ResourceManager>
{
    
    Dictionary<int, ItemData> itemDataDic=new Dictionary<int,ItemData>();
    Dictionary<int, Equipment> equipmentDataDIc = new Dictionary<int, Equipment>();
    Dictionary<int,SeedTempClass> seedTempDataDic = new Dictionary<int,SeedTempClass>(); //이거는 추가 데이터용
    public Dictionary<int,Collection> collectionDataDic = new Dictionary<int,Collection>(); //일단 임시로 갯수떄매 public해놓았어요
    Dictionary<int,Upgrade> upgradeDataDic= new Dictionary<int,Upgrade>();
    protected override void Awake()
    {
        base.Awake();
        SetSeed();

        SetItem();
        SetEquipment();
        SetCollection();
        SetUpgrade();

    } 
    


    void SetItem()
    {
        TextAsset itemCSVText = Resources.Load<TextAsset>("ItemData/ItemDataCSV/ItemDataTable");
        string[] rows = itemCSVText.text.Split('\n');
        for (int i = 1; i < rows.Length; i++)
        {
            if (string.IsNullOrEmpty(rows[i]))
            {
                return;
            }
            string[] columns = rows[i].Split(',');
            for (int j = 0; j < columns.Length; j++)
            {
                columns[j] = columns[j].Trim();
            }
            int ID = int.Parse(columns[0]);
            int itemEffect = columns[5] == string.Empty ? 0 : int.Parse(columns[5]);
            int maxNum=int.Parse(columns[6]);

            //미리 씨앗데이터나 기타 데이터들은 로드후에
            //여기에 딕셔너리에 이 아이템의 ID로 trygetvalue를 해보고
            //그게 가능하면 SeedData로 넣고 아니면 아이템데이터로 생성자 만드는 식으로
            ItemData itemData;
            if(seedTempDataDic.TryGetValue(ID, out var seedTempData))
            {

                itemData = new SeedData(ID, columns[1], columns[2], columns[3], columns[4],itemEffect, maxNum,seedTempData.growTime,seedTempData.seedTilePath, seedTempData.growthTilePath, seedTempData.cropTilePath ); //여기에 더 추가
                itemDataDic[ID] = itemData;
            }
            else
            {
                itemData = new ItemData(ID, columns[1], columns[2], columns[3], columns[4],itemEffect, maxNum);
                itemDataDic.Add(ID, itemData);
            }
                
            


        }
    }


    public ItemData GetItem(int itemID)
    {
        if(itemDataDic.TryGetValue(itemID, out ItemData itemData))
        {
            itemData.itemIcon = Resources.Load<Sprite>($"ItemData/ItemNewImage/{itemData.itemPath}");
            if(itemData is SeedData seed)
            {
                seed.seedTileBase= Resources.Load<TileBase>($"SeedData/{seed.seedTilePath}");
                seed.growthTileBase = Resources.Load<TileBase>($"SeedData/{seed.growthTilePath}");
                seed.cropTileBase = Resources.Load<TileBase>($"SeedData/{seed.cropTilePath}");
            }
            return itemData;
        }
        else
        {
            Debug.Log("등록되지 않은 아이템");
            return null;
        }    
    }
    


    void SetEquipment()
    {
        TextAsset itemCSVText = Resources.Load<TextAsset>("EquipmentData/EquipmentDataCSV/EquipmentExcel");

        string[] rows = itemCSVText.text.Split('\n');
        for (int i = 1; i < rows.Length; i++)
        {
            if (string.IsNullOrEmpty(rows[i]))
            {
                return;
            }
            string[] columns = rows[i].Split(',');
            for (int j = 0; j < columns.Length; j++)
            {
                columns[j] = columns[j].Trim();
            }
            int ID = int.Parse(columns[0]);
            Equipment equipment = new Equipment(ID, columns[1], columns[2], columns[3], columns[4]);
            equipmentDataDIc.Add(ID, equipment);


        }
    }
    public Equipment GetEquipment(int equipmentID)
    {
        if (equipmentDataDIc.TryGetValue(equipmentID, out Equipment equipment))
        {
            equipment.equipmentIcon= Resources.Load < Sprite > ($"EquipmentData/{equipment.equipmentPath}"); //테스트용 추후 지울예정
            //equipment.equipmentIcon = Resources.Load<Sprite>($"ItemData/{equipment.equipmentPath}");
            return equipment;
        }
        else
        {
            Debug.Log("등록되지 않은 장비");
            return null;
        }
    }


    public void SetSeed()
    {
        TextAsset seedCSVText = Resources.Load<TextAsset>("SeedData/SeedDataCSV/SeedExcel");

        string[] rows = seedCSVText.text.Split('\n');
        for (int i = 1; i < rows.Length; i++)
        {
            if (string.IsNullOrEmpty(rows[i]))
            {
                return;
            }
            string[] columns = rows[i].Split(',');
            for (int j = 0; j < columns.Length; j++)
            {
                columns[j] = columns[j].Trim();
            }
            int ID = int.Parse(columns[0]);
            int growTime=int.Parse(columns[1]);

            SeedTempClass seedTempClass=new SeedTempClass(ID, growTime, columns[2], columns[3], columns[4]);
            seedTempDataDic.Add(ID, seedTempClass);

            
            
           


        }
    }

    void SetCollection()
    {
        TextAsset collectionCSVText = Resources.Load<TextAsset>("CollectionData/CollectionDataCSV/CollectionDataCSV");

        string[] rows = collectionCSVText.text.Split('\n');
        for (int i = 1; i < rows.Length; i++)
        {
            if (string.IsNullOrEmpty(rows[i]))
            {
                return;
            }
            string[] columns = rows[i].Split(',');
            for (int j = 0; j < columns.Length; j++)
            {
                columns[j] = columns[j].Trim();
            }
            int ID = int.Parse(columns[0]);
            Collection collection = new Collection(ID, columns[1], columns[2], columns[3], columns[4], columns[5], columns[6], columns[7], columns[8]);
            collectionDataDic.Add(ID, collection);
            //사전에 넣기
            

        }
        Debug.Log($"컬렉션 갯수:{collectionDataDic.Count}");
    }


    public Collection GetCollection(int id)
    {
        if (collectionDataDic.TryGetValue(id,out Collection collection))
        {


            return collection;
        }
        else
        {
            Debug.Log("등록되지 않은 컬렉션");
            return null;
        }
    }


    void SetUpgrade()
    {
        TextAsset upgradeCSVText = Resources.Load<TextAsset>("UpgradeData/UpgradeDataCSV/UpgradeCSV");

        string[] rows = upgradeCSVText.text.Split('\n');
        for (int i = 1; i < rows.Length; i++)
        {
            if (string.IsNullOrEmpty(rows[i]))
            {
                return;
            }
            string[] columns = rows[i].Split(',');
            for (int j = 0; j < columns.Length; j++)
            {
                columns[j] = columns[j].Trim();
            }

            Upgrade upgrade = new Upgrade(columns[0], columns[1], columns[2], columns[3], columns[4], columns[5], columns[6], columns[7]) ;

            upgradeDataDic.Add(int.Parse(columns[0]),upgrade);
            //사전에 넣기


        }
        
    }

    public Upgrade GetUpgrade(int id)
    {
        if(upgradeDataDic.TryGetValue(id,out Upgrade upgrade))
        {
            return upgrade;
        }
        else
        {
            return null;
        }
    }



    //그냥 임시 데이터 정리용 클래스
    private class SeedTempClass
    {
        public int ID;
        public int growTime;
        public string seedTilePath;
        public string growthTilePath;
        public string cropTilePath;

        public SeedTempClass(int ID, int growTIme, string  seedtilePath,string growthTilePath, string cropTIlePath)
        {
            this.ID = ID;
            this.growTime = growTIme;
            this.seedTilePath = seedtilePath;
            this.growthTilePath = growthTilePath;
            this.cropTilePath = cropTIlePath;
        }
        
    }

    public bool TryGetItemName(int itemId, out string name)
    {
        if (itemDataDic != null && itemDataDic.TryGetValue(itemId, out var data) && data != null)
        {
            name = data.itemName;
            return true;
        }

        name = null;
        return false;
    }

}


