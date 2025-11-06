using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Text;



[Serializable]
public class SlotData
{
    public Item slotItem = null;


    public SlotData()
    {

    }
    public SlotData(Item slotItem)
    {
        this.slotItem = slotItem;
    }
}


public class PlayerInventory : Singleton<PlayerInventory>
{
   

    //아이템 목록
    public List<SlotData> slotDataList = new List<SlotData>();

    //퀵슬롯 아이템 목록
    public List<SlotData> itemQuickSlotDatas = new List<SlotData>();



    //여기 씨앗 리스트에는 인벤토리중 씨앗만(인벤토리는 씨앗포함 모두)

    //이 두개의 컬렉션은 정렬용으로 임시사용
    Dictionary<ItemData, int> itemDic = new Dictionary<ItemData, int>();
    List<SlotData> slotTempList = new List<SlotData>();

    //갯수 제외용 임시 사용
    List<SlotData> wantedItemSlotList = new List<SlotData>();


    public int InventoryMaxNum { get; } = 40; //짝수만 가능 책 UI때매


    public HashSet<int> obtainedHashSet = new HashSet<int>();


    //Refactor:후에 저장 json경로는 딴곳으로 옮길수도있어요
    private string JsonPath => Path.Combine(Application.persistentDataPath);
    private const string FILENAME = "Inventory.Json";

    //이 갯수 체인지가 될경우에는 슬롯들의 상황을 업데이트 해줍니다


    public Action onItemChange;
    private Action onItemQuickSlotChange;

    public Item CreateRuntimeItemData(ItemData itemData)
    {
        return new Item(itemData);
    }

    public Item CreateRuntimeItemData(ItemData itemData,int quantity)
    {
        return new Item(itemData,quantity);
    }
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (slotDataList != null && slotDataList.Count != 0)
            return;

        for (int i = 0; i < InventoryMaxNum+3; i++)
        {
            slotDataList.Add(new SlotData());
        }
    }

    //단일 아이템 더하기
    public void AdditemsByID(int itemID,int addQuantity)     
    {
        CollectionManager.Instance.ObtainedItem(itemID, addQuantity);
        ItemData itemdata= ResourceManager.Instance.GetItem(itemID);
        if (itemdata == null)
        {
            Debug.LogError($"[PlayerInventory] 존재하지 않는 아이템ID: {itemID} → 인벤토리에 추가 불가");
            return;
        }

        //만약 쌓을 수 있는 아이템이라면 일단 모든 슬롯데이터 리스트를 확인한다

        int quantity=addQuantity;
        //근데 이 공식의 치명적인 문제는 결국 여유분 슬롯을 찾지못하면 quantity를 줄일 방법이 없어서 무한루프에 빠진다는 점이다. 

        if(itemdata.itemType==ItemType.Crop)
        {
            //여기에 작물 먹는 튜토리얼
            StartCoroutine(TutSpawner.Instance.DisplayTutorial(8));
        }



        if (itemdata.isStackable)
        {
            for (int i = 0; i < slotDataList.Count; i++)
            {



                //?. 를 통하여 slotitem이 null일경우를 대비
                if (slotDataList[i].slotItem?.itemData == itemdata)
                {
                    //만약 슬롯데이터의 아이템의 데이터가 들어온 아이템데이터와 같다면
                    //우선 현재 그 슬롯에 들어있는 수량을 확인한다. 
                    //그리고 그 아이템의 최대수량- 현재수량으로 더 넣을수 있는 갯수를 체크한다.


                    //사실 여기 if문으로 들어온 시점부터 이 슬롯의 슬롯아이템은 null일수없음
                    Item thisSlotItem = slotDataList[i].slotItem;
                    int canAddNum = CheckCanAddNum(thisSlotItem);
                    //만약 더 넣을 여유칸이 없다면 이 슬롯은 스킵한다
                    if (canAddNum == 0)
                    {
                        continue;
                    }
                    else
                    {
                        // 추가되는 양이 여유분보다 많은지 적은지 체킹한다


                        //만약 추가하려는 수량이 여유분보다 많다면

                        //이 슬롯의 아이템 수량은 그 아이템의 최대갯수가 된다
                        if (quantity > canAddNum)
                        {
                            thisSlotItem.currentQuantity = thisSlotItem.itemData.maxQuantity;
                            //그리고 남은 것들은 다시 또 딴슬롯을 찾아야한다. 

                            quantity -= canAddNum;

                            continue;
                        }
                        else
                        {
                            //만약 여유분이 더하려는 양보다 많다면
                            thisSlotItem.currentQuantity += quantity;
                            //다 넣었으니 이제 배정해야할 quantity가 없어
                            quantity = 0;
                            //여기에 이제 완료 업데이트
                            onItemChange?.Invoke();

                            return;
                        }
                    }
                }
            }

            //만약 슬롯들 쭉 살펴봤는데 기존에 이거랑 같은 아이템이 없어 그럼 새로 더미를 만들어줘야겠지?
            PlaceRemainingQuantity(itemdata, quantity);
            //남은 수량 관리 메서드에 넘겨주고 이 메서드는 여기서 끝내기
            return;
            //더미는 언제까지 만들어? 남은 수량을 다 쓸때까지
            //여기까지는 그럼 이제 배정받지 못하고 남은 quantity가 있겠지

            //애초부터 빈슬롯이 있는지도 체크가 필요
        }
        else
        {
            //만약 쌓을수 없는 아이템이면 그냥 바로 남은 갯수 배치 메서드로 보내기
            PlaceRemainingQuantity(itemdata,quantity);
        }


    }
    void PlaceRemainingQuantity(ItemData itemData, int remainingQuantity)
    {
        while(remainingQuantity > 0)
        {
            //일단 슬롯이 비었는지 확인
            int slotIndex = FindFirstEmptyInventoryIndex();
            if (slotIndex == -1)
            {
                //아예 남은 슬롯이 없음
                Debug.Log("남은 슬롯 없음");
                return;
            }
            //만약 남은 갯수가 아이템 최대 수량보다 많다면
            if(remainingQuantity>itemData.maxQuantity)
            {
                Item newItem = CreateRuntimeItemData(itemData,itemData.maxQuantity);
                remainingQuantity-=itemData.maxQuantity;
                slotDataList[slotIndex].slotItem = newItem;
                

            }
            else
            {
                Item newItem = CreateRuntimeItemData(itemData, remainingQuantity);
                remainingQuantity = 0;
                slotDataList[slotIndex].slotItem = newItem;

                //여기에 완료 업데이트
                onItemChange?.Invoke();

            }
            
        }
        
    }



    //인벤토리에서 바꾸는 로직이 필요
    //첫번쨰 슬롯 인덱스는 스위칭 타겟(즉 가만히 있는 아이템), 두번째는 내가 드래그해서 들고 내려놓는 그 아이템의 인덱스
    public void SwitchItemPlaces(int firstIndex, int secondIndex)
    {
        if(firstIndex==secondIndex)
        {
            onItemChange?.Invoke();
            return;
        }
        if (slotDataList[secondIndex].slotItem == null)
        {
            return; //애초부터 드래그하는쪽이 null이면 안됨
        }
        

        //쌓이는 아이템을 알아보자
        if ((slotDataList[firstIndex].slotItem?.itemData == slotDataList[secondIndex].slotItem?.itemData)&& slotDataList[firstIndex].slotItem?.itemData.isStackable==true)
        {
            //만약 두 아이템이 같고, 두 아이템이 stackable이라면 쌓이는 로직을 적용
            if (slotDataList[firstIndex].slotItem == null)
            {
                //그냥 첫슬롯이 빈거면 생각할것도 없이 자리바꾸기
                SlotData temp = slotDataList[firstIndex];
                slotDataList[firstIndex] = slotDataList[secondIndex];
                slotDataList[secondIndex] = temp;
                
            }            
            else
            {
                Debug.Log("작동확인");
                //둘다 빈 슬롯이 아니고, 둘다 아이템이 같고, 쌓이기 가능
                //만약 첫슬롯의 갯수+ 두번쨰 슬롯의 갯수가 아이템 최대 수량보다 크다면 1번슬롯꺼를 최대수량으로 하고 2번슬롯에 나머지 배치
                int firstQuantity = slotDataList[firstIndex].slotItem.currentQuantity;
                int secondQuantity= slotDataList[secondIndex].slotItem.currentQuantity;
                int maxQuantity = slotDataList[firstIndex].slotItem.itemData.maxQuantity;
                if ((firstQuantity+secondQuantity)>maxQuantity)
                {
                    secondQuantity = (firstQuantity + secondQuantity) - maxQuantity;
                    firstQuantity = slotDataList[firstIndex].slotItem.itemData.maxQuantity;

                    slotDataList[firstIndex].slotItem.currentQuantity=firstQuantity;
                    slotDataList[secondIndex].slotItem.currentQuantity=secondQuantity;

                }
                else
                {
                    Debug.Log("작동확인1");
                    //만약 둘이 더한게 최대수량보다 작거나 같으면 그냥 하나로 합치고 2번째는 그냥 null로 바꾼다. 
                    firstQuantity =firstQuantity+secondQuantity;
                    slotDataList[firstIndex].slotItem.currentQuantity = firstQuantity;
                    slotDataList[secondIndex].slotItem = null;
                }
            }
            onItemChange.Invoke();
        }
        else
        {
            //만약 쌓이는게 아니면, 그냥 첫번쨰 아이템, 두번쨰 아이템을 각각 리스트에서 순서만 바꾸기
            //기본로직
            SlotData temp = slotDataList[firstIndex];
            slotDataList[firstIndex] = slotDataList[secondIndex];
            slotDataList[secondIndex] = temp;
            onItemChange?.Invoke();
            //이 뒤에 그냥 ui는 세팅만 
        }


    }

    //만약 아이템을 바꾸려면 그럼 어떻게 해야되는가?

    //그냥 드래그앤 드롭했다는 신호만 준다. 그리고 바꾼다. 
    public int CheckItemQuantity(int itemID)
    {
        ItemData itemData = ResourceManager.Instance.GetItem(itemID);
        int itemSum = 0;
        wantedItemSlotList.Clear();

        foreach (var slotData in slotDataList)
        {
            if (slotData.slotItem == null||slotData.slotItem.itemData==null)
            {
                continue;
            }
            if (slotData.slotItem.itemData.itemID == itemData.itemID)
            {
                itemSum += slotData.slotItem.currentQuantity;
                wantedItemSlotList.Add(slotData);

                

            }
        }
        return itemSum;
    }




    //이건 주로 상점같은 곳이나 이런곳에서 재료 다 모았는지 확인할때(혹은 1개씩 뺄때도 써도되긴함)
    public bool SubtractItemQuantity( int itemID,int subtractAmount)
    {
       //일단 빼려는 그 아이템을 찾아준다
        ItemData itemData=ResourceManager.Instance.GetItem(itemID);
        int itemSum = 0;
        wantedItemSlotList.Clear();

        foreach ( var slotData in slotDataList )
        {
            if( slotData.slotItem ==null||slotData.slotItem.itemData==null)
            {
                continue;
            }
            if(slotData.slotItem.itemData.itemID==itemData.itemID)
            {
                itemSum += slotData.slotItem.currentQuantity;
                wantedItemSlotList.Add(slotData);

                if(itemSum>=subtractAmount)
                {
                    break;
                }

            }
        }

        if( itemSum >=subtractAmount ) //차감 요구되는 양보다 내가 가진게 많을때
        {

            int subtractSum = 0;
            //만약 재료가 충분히 있다면아까 저장해둔 슬롯리스트를 돌면서 마지막꺼만 주의해서 차감하면됨.( 그 이전꺼는 걍 다 비워도됨)
            for(int i = 0;i<wantedItemSlotList.Count-1; i++)
            {
                subtractSum+=wantedItemSlotList[i].slotItem.currentQuantity;
                EmptyOutSlot(wantedItemSlotList[i]);
            }

            //마지막 슬롯은
            wantedItemSlotList[wantedItemSlotList.Count - 1].slotItem.currentQuantity -= (subtractAmount - subtractSum); //부족한 갯수만큼 여기서 차감
            if(wantedItemSlotList[wantedItemSlotList.Count - 1].slotItem.currentQuantity==0)//마지막 슬롯에서 차감했을때 남은게 0개면 슬롯 비우기
            {
                EmptyOutSlot(wantedItemSlotList[wantedItemSlotList.Count - 1]);
                //만약 여기서 바구니에 선택된 씨앗이 0개가 된다면 바구니를 연다

                //if(PlayerEquipment.Instance.CheckEquipmentExtra(EquipmentType.SeedBasket)==itemData.itemID) 
                //{
                //    UIManager.Instance.OpenUI<UISeedBasket>();
                //}
                if(PlayerEquipment.Instance.CheckEquipmentExtra(EquipmentType.SeedBasket)==itemData.itemID)
                {
                    PlayerEquipment.Instance.ChangeEquipmentExtra(EquipmentType.SeedBasket, -1);
                }

            }

        }
        else
        {
            return false;
        }

        onItemChange?.Invoke();
        return true;

        
        //만약 사용하려는(혹은 버리려는) 갯수가 아이템 currentquantity보다 크다면
    }




    public void EmptyOutSlot(SlotData slotData)
    {
        slotData.slotItem = null;
    }

    //이건 아예 묶음 통으로 버리기
    


    int FindFirstEmptyInventoryIndex()
    {
        //Refactor : 제거
        for (int i=0; i<slotDataList.Count; i++)
        {
            if (slotDataList[i].slotItem==null || slotDataList[i].slotItem.itemData == null)
            {
                return i;
            }
        }
        return -1;
    }


    // 이 메서드를 통해 현재 이 아이템은 몇개의 여유 수용 갯수( 최대수량으로 부터)가 있는지 알려줍니다
    int CheckCanAddNum(Item item)
    {
        return (item.itemData.maxQuantity) - (item.currentQuantity);
    }

    
    
    public void SortInventory()
    {
        itemDic.Clear();
        slotTempList.Clear();
        //foreach(SlotData slotData in slotDataList)
        //{
        //    if(slotData.slotItem==null || slotData.slotItem.itemData == null)
        //    {
        //        continue;
        //    }

        //    //이미 값이 있으면 quantity에 추가
        //    if(itemDic.TryGetValue(slotData.slotItem.itemData, out int quantity))
        //    {
        //        itemDic[slotData.slotItem.itemData] = quantity + slotData.slotItem.currentQuantity;
        //    }
        //    else
        //    {
        //        //딕셔너리에 미등록이면
        //        itemDic.Add(slotData.slotItem.itemData, slotData.slotItem.currentQuantity);
        //    }

        //}

        for(int i=0; i<40;  i++)
        {
            if (slotDataList[i].slotItem==null || slotDataList[i].slotItem.itemData==null)
            {
                continue;
            }

            if (itemDic.TryGetValue(slotDataList[i].slotItem.itemData, out int quantity))
            {
                itemDic[slotDataList[i].slotItem.itemData] = quantity + slotDataList[i].slotItem.currentQuantity;
            }
            else
            {
                //딕셔너리에 미등록이면
                itemDic.Add(slotDataList[i].slotItem.itemData, slotDataList[i].slotItem.currentQuantity);
            }
        }

        //여기까지 하면 <아이템데이터, 현재갯수> 이렇게 저장이 된다 이걸 정렬후 인벤토리에 분배하면된다.


        itemDic = itemDic.OrderBy(x => x.Key.itemID).ToDictionary(x => x.Key, x => x.Value);
        
        //아이디순으로 재정렬된 딕셔너리를 가지고 다시 배치해보자

        //키( 아이템) 하나씩 확인해보기

        foreach(var dictionaryItem in itemDic.Keys)
        {
            int sortQuantity=itemDic[dictionaryItem];  //몇개 배정해야하는지

            while(sortQuantity > 0)
            {
                if(sortQuantity>=dictionaryItem.maxQuantity)  //배정할게 최대수량보다 많으면
                {
                    Item it=new Item(dictionaryItem,dictionaryItem.maxQuantity);
                    SlotData slotData=new SlotData(it);
                    slotTempList.Add(slotData);
                    sortQuantity -= dictionaryItem.maxQuantity;
                }
                else
                {

                    //만약 배정할게 최대수량보다 적으면
                    Item it = new Item(dictionaryItem, sortQuantity);
                    SlotData slotData = new SlotData(it);
                    slotTempList.Add(slotData);
                    sortQuantity = 0;
                }
            }
        }

        //이 과정을 다 거치면 임시 리스트를 원래 리스트에 넣어주기만하면됨
        //근데 이게 남은건 빈칸으로 채워줘야함
        for(int i=0;  i<slotTempList.Count; i++)
        {
            slotDataList[i]=slotTempList[i];
        }
        for(int i=slotTempList.Count;i<40 ;i++)
        {
            EmptyOutSlot(slotDataList[i]);  
        }
        onItemChange?.Invoke();


       
    }


    //아이템 섭취하는 메서드
    public void ConsumeItem(int itemID)
    {
        ItemData itemData= ResourceManager.Instance.GetItem(itemID);
        Debug.Log("아이템 섭취");
        if(itemData.itemEffect==0)
        {
            return;
        }
        if(SubtractItemQuantity(itemID, 1))
        {
            PlayerManager.Instance.playerStamina.Recover((float)itemData.itemEffect);
        }
        
        
    }



    //여기는 키 바인딩으로 슬롯 선택임
    public void SelectSlot(int slot)
    {

        if(slot==8)
        {
            if (slotDataList[40].slotItem != null && slotDataList[40].slotItem.itemData!=null)
            {
                ConsumeItem(slotDataList[40].slotItem.itemData.itemID);
            }
           
        }
        else if(slot==9)
        {
            if (slotDataList[41].slotItem != null && slotDataList[41].slotItem.itemData != null)
            {
                ConsumeItem(slotDataList[41].slotItem.itemData.itemID);
            }
        }
        else if (slot==10)
        {
            if (slotDataList[42].slotItem != null && slotDataList[42].slotItem.itemData != null)
            {
                ConsumeItem(slotDataList[42].slotItem.itemData.itemID);
            }
        }
        onItemChange?.Invoke();
        
        
    }





    public void SubscribeOnItemChange(Action action)
    {
        onItemChange += action;
    }

    public void UnsubscribeOnItemChange(Action action)
    {
        onItemChange -= action;
    }


    public void SubscribeOnItemQuickSlotChange(Action action)
    {
        onItemQuickSlotChange += action;
    }

    public void UnsubscribeOnItemQuickSlotChange(Action action)
    {
        onItemQuickSlotChange -= action;
    }





    public void SaveInventoryStatus(int slotNumber)
    {

        ItemListWrapper itemListWrapper=new ItemListWrapper();
        foreach (var slot in slotDataList)
        {
            ItemForWrapping itemForWrapping = null;
            if (slot.slotItem == null || slot.slotItem.itemData == null)
            {
                itemForWrapping = new ItemForWrapping(-1, 0); //만약 id가 -1이면 빈칸으로 불러오게
            }
            else
            {
                int id = slot.slotItem.itemData.itemID;
                int quantity = slot.slotItem.currentQuantity;
                itemForWrapping= new ItemForWrapping(id, quantity);
            }
            itemListWrapper.slotDataWrapperList.Add(itemForWrapping);

        }


        string json=JsonUtility.ToJson(itemListWrapper,true);

        StringBuilder savePath = new StringBuilder();
        savePath.Append(JsonPath).Append('/').Append(SaveManager.SAVEFILEPATH).Append(slotNumber).Append('/').Append(FILENAME);
        
        File.WriteAllText(savePath.ToString(), json);

        Debug.Log("저장 완료: " + JsonPath);

    }


    public void LoadInventoryStatus(int slotNumber)
    {
        StringBuilder loadPath = new StringBuilder();
        loadPath.Append(JsonPath).Append('/').Append(SaveManager.SAVEFILEPATH).Append(slotNumber).Append('/').Append(FILENAME);
        if (File.Exists(loadPath.ToString()))
        {


            Debug.Log("파일 있음");
            string json = File.ReadAllText(loadPath.ToString());
            Init();
            ItemListWrapper itemListWrapper = JsonUtility.FromJson<ItemListWrapper>(json);
            for(int i = 0; i<slotDataList.Count; i++)
            {
                ItemForWrapping itemWrappingData = itemListWrapper.slotDataWrapperList[i];
                if(itemWrappingData.ID==-1)
                {
                    slotDataList[i].slotItem = null;
                }
                else
                {
                    slotDataList[i].slotItem = new Item(ResourceManager.Instance.GetItem(itemWrappingData.ID), itemWrappingData.quantity);
                }
                    
            }
            
        }
        onItemChange?.Invoke();
    }

}

[Serializable]
public class ItemListWrapper
{
    public List<ItemForWrapping> slotDataWrapperList = new List<ItemForWrapping>();
}

[Serializable]
public class ItemForWrapping
{
    public int ID;
    public int quantity;
    public ItemForWrapping(int id, int quantity)
    {
        this.ID = id;
        this.quantity = quantity;
    }
}
