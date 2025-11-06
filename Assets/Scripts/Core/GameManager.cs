using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    ResourceManager resourceManagerInstance;
    TileDataBase tileDatabaseInstance;
    ChunkControl chunkControlInstance;

    MapDataBase mapDataBaseInstance;
    MapSaveManager mapSaveManagerInstance;
    TimeManager timeManagerInstance;
    
    PlayerInventory playerInventoryInstance;
    PlayerEquipment playerEquipmentInstance;
    PlayerManager playerManagerInstance;

    MapControl mapControlInstance;

    GameInfoSaveManager gameInfoSaveManagerInstance;
    QuestManager questManagerInstance;
    NpcDialog npcDialogInstance;
    NPCManager npcManagerInstance;
    UIManager uiManagerInstance;
    ShopCSVParser shopCSVParserInstance;

    CollectionManager collectionManagerInstance;


    protected override void Initialize()
    {
        if(ResourceManager.Instance == null)
            resourceManagerInstance = new GameObject("Resource Manager").AddComponent<ResourceManager>();
        else
            resourceManagerInstance = ResourceManager.Instance;
        resourceManagerInstance.transform.parent = transform;

        if (tileDatabaseInstance == null)
            tileDatabaseInstance = new GameObject("TileDataBase").AddComponent<TileDataBase>();
        else
            tileDatabaseInstance = TileDataBase.Instance;
        tileDatabaseInstance.transform.parent = transform;

        if(chunkControlInstance == null)
            chunkControlInstance = new GameObject("ChunkControl").AddComponent<ChunkControl>();
        else
            chunkControlInstance = ChunkControl.Instance;
        chunkControlInstance.transform.parent = transform;

        if (MapDataBase.Instance == null)
            mapDataBaseInstance = new GameObject("MapDataBase").AddComponent<MapDataBase>();
        else
            mapDataBaseInstance = MapDataBase.Instance;
        mapDataBaseInstance.transform.parent = transform;


        if(MapSaveManager.Instance == null)
            mapSaveManagerInstance = new GameObject("Map Save Manager").AddComponent<MapSaveManager>();
        else
            mapSaveManagerInstance = MapSaveManager.Instance;
        mapSaveManagerInstance.transform.parent = transform;

        if (TimeManager.Instance == null)
            timeManagerInstance = new GameObject("Time Manager").AddComponent<TimeManager>();
        else
            timeManagerInstance = TimeManager.Instance;
        timeManagerInstance.transform.parent = transform;

        if (PlayerInventory.Instance == null)
            playerInventoryInstance = new GameObject("Player Inventory").AddComponent<PlayerInventory>();
        else
            playerInventoryInstance = PlayerInventory.Instance;
        playerInventoryInstance.transform.parent = transform;

        if (PlayerEquipment.Instance == null)
            playerEquipmentInstance = new GameObject("Player Equipment").AddComponent<PlayerEquipment>();
        else
            playerEquipmentInstance = PlayerEquipment.Instance;
        playerEquipmentInstance.transform.parent = transform;

        if (PlayerManager.Instance == null)
            playerManagerInstance = new GameObject("Player Manager").AddComponent<PlayerManager>();
        else
            playerManagerInstance = PlayerManager.Instance;
        playerManagerInstance.transform.parent = transform;

        if (MapControl.Instance == null)
            mapControlInstance = new GameObject("MapControl").AddComponent<MapControl>();
        else
            mapControlInstance = MapControl.Instance;
        mapControlInstance.transform.parent = transform;


        if (GameInfoSaveManager.Instance == null)
            gameInfoSaveManagerInstance = new GameObject("GameInfo Save Manager").AddComponent<GameInfoSaveManager>();
        else
            gameInfoSaveManagerInstance = GameInfoSaveManager.Instance;
        gameInfoSaveManagerInstance.transform.parent = transform;


        if (NpcDialog.Instance == null)
            npcDialogInstance = new GameObject("Npc Dialgoue Manager").AddComponent<NpcDialog>();
        else
            npcDialogInstance = NpcDialog.Instance;
        npcDialogInstance.transform.parent = transform;
        if (QuestManager.Instance == null)
            questManagerInstance = new GameObject("Quest Manager").AddComponent<QuestManager>();
        else
            questManagerInstance = QuestManager.Instance;
        questManagerInstance.transform.parent = transform;
        if (NPCManager.Instance == null)
            npcManagerInstance = new GameObject("Npc Manager").AddComponent<NPCManager>();
        else
            npcManagerInstance = NPCManager.Instance;
        npcManagerInstance.transform.parent = transform;

        if (UIManager.Instance == null)
            uiManagerInstance = new GameObject("UI Manager").AddComponent<UIManager>();
        else
            uiManagerInstance = UIManager.Instance;
        uiManagerInstance.transform.parent = transform;


        if (ShopCSVParser.Instance == null)
            shopCSVParserInstance = new GameObject("Shop CSV Manager").AddComponent<ShopCSVParser>();
        else
            shopCSVParserInstance = ShopCSVParser.Instance;
        shopCSVParserInstance.transform.parent = transform;

        if (CollectionManager.Instance == null)
            collectionManagerInstance = new GameObject("Collection Manager").AddComponent<CollectionManager>();
        else
            collectionManagerInstance = CollectionManager.Instance;
        collectionManagerInstance.transform.parent = transform;

    }

    protected override void OnDestroy()
    {
        if(ShopCSVParser.Instance != null)
            Destroy(ShopCSVParser.Instance);
        if(UIManager.Instance != null)
            Destroy(UIManager.Instance);
        if(NPCManager.Instance != null)
            Destroy(NPCManager.Instance);
        if(QuestManager.Instance != null)
            Destroy(QuestManager.Instance);
        if(NpcDialog.Instance != null)
            Destroy(NpcDialog.Instance);

        if(GameInfoSaveManager.Instance != null)
            Destroy(GameInfoSaveManager.Instance);
        if(PlayerManager.Instance != null)
            Destroy(PlayerManager.Instance);
        if(PlayerEquipment.Instance != null)
            Destroy(PlayerEquipment.Instance);

        if(PlayerInventory.Instance != null)
            Destroy(PlayerInventory.Instance);
        if(TimeManager.Instance != null) 
            Destroy(TimeManager.Instance);
        if(MapSaveManager.Instance!= null)
            Destroy(MapSaveManager.Instance);

        if (CollectionManager.Instance != null)
            Destroy(CollectionManager.Instance);
        base.OnDestroy();
    }
}
