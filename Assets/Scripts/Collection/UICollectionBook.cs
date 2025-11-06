using System.Collections.Generic;
using UnityEngine;

//시작하면 이거 세팅



//아예 새로운 컬렉션을 저장해야할듯







public class UICollectionBook : UIBase
{

    List<UICollectionSlot> collectionSlotList = new List<UICollectionSlot>();

    [SerializeField] GameObject collectionSlotPrefab;

    [SerializeField] Transform collectionBigSlotBG;
   

    private void OnEnable()
    {
        //콜렉션 매니저에에 데이터 로드되어있는지 확인
        if(CollectionManager.Instance.collectionsList.Count==0)
        {
            CollectionManager.Instance.GetFromDic();
        }


        //슬롯들 없으면 슬롯들 생성/생성되어있으면 동기화
        SetCollectionSlots();

        CollectionManager.Instance.onCollectionChange += SetCollectionSlots;


    }

    private void OnDisable()
    {
        CollectionManager.Instance.onCollectionChange -= SetCollectionSlots;
    }


    void SetCollectionSlots()
    {
        if (collectionSlotList.Count == 0)
        {
            for (int i = 0; i < CollectionManager.Instance.collectionsList.Count; i++)
            {
                GameObject go = Instantiate(collectionSlotPrefab, collectionBigSlotBG, false);
                UICollectionSlot slot = go.GetComponent<UICollectionSlot>();
                collectionSlotList.Add(slot);
                slot.SetCollectionSlot(CollectionManager.Instance.collectionsList[i]);
            }
        }
        else
        {
            for (int i = 0; i < collectionSlotList.Count; i++)
            {
                collectionSlotList[i].SetCollectionSlot(CollectionManager.Instance.collectionsList[i]);
                
            }
        }
    }
   
    
    
 


        
}
