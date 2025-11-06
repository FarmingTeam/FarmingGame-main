
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICollectionSlot : UIBase
{

    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform slotBarBG;

    [SerializeField] TextMeshProUGUI collectionTitle;
    [SerializeField] TextMeshProUGUI collectionDescription;
    

    [SerializeField] Button RewardButton;
    [SerializeField] TextMeshProUGUI rewardButtonText;
    [SerializeField] TextMeshProUGUI rewardText;

    public Collection currentCollection;

    List<int> tempList = new List<int>();

    List<GameObject> slotTempList= new List<GameObject>();
   

    void PressRewardButton()
    {
        PlayerManager.Instance.playerMoney.Add(currentCollection.rewardAmount);
        CollectionManager.Instance.PressGetRewardButton(this.currentCollection);
    }

    public void SetCollectionSlot(Collection collection)
    {
        currentCollection = collection;
        collectionTitle.SetText(collection.collectionName);
        collectionDescription.SetText(collection.collectionDescription);
        rewardText.SetText($"{ collection.rewardAmount} G");


            int collectedCount = 0;
        for(int i=0; i< collection.requiredItemList.Count; i++ )
        {
            GameObject go = null;
            if(slotTempList.Count!=collection.requiredItemList.Count)
            {
                go = Instantiate(slotPrefab, slotBarBG.transform, false);
                slotTempList.Add(go);
            }
            else
            {
                go=slotTempList[i];
            }


            Image image = go.GetComponent<Image>();
            TextMeshProUGUI quantityText=go.GetComponentInChildren<TextMeshProUGUI>();

            RequiredItem requiredItem = collection.requiredItemList[i];
            if (requiredItem.itemID>10000)
            {
                image.sprite = ResourceManager.Instance.GetEquipment(requiredItem.itemID).equipmentIcon;
                quantityText.SetText(1.ToString());
            }
            else
            {
                image.sprite = ResourceManager.Instance.GetItem(requiredItem.itemID).itemIcon;
                quantityText.SetText(requiredItem.amount.ToString());
            }


            if (CollectionManager.Instance.IsCompletedItem(requiredItem))
            {
                image.color = Color.white;
            }
            else
            {
                image.color = Color.gray;
            }

            //if ( PlayerInventory.Instance.obtainedHashSet.Contains(i))
            //{
            //    image.color = Color.red;
            //    collectedCount++;
            //}

            

        }
        if(CollectionManager.Instance.IsCompletedCollection(collection))
        {
            if(collection.collectionState!=2)
            {
                collection.collectionState = 1;
            }
            
        }
        if (collection.collectionState == 0)
        {
            rewardButtonText.SetText("미완료");
        }
        else if (collection.collectionState == 1)
        {
            rewardButtonText.SetText("보상받기");
            RewardButton.onClick.RemoveAllListeners();
            RewardButton.onClick.AddListener(PressRewardButton);
        }
        else if (currentCollection.collectionState == 2)
        {
            rewardButtonText.SetText("획득완료");
            Image image = RewardButton.GetComponent<Image>();
            image.color = Color.gray;
            RewardButton.onClick.RemoveAllListeners();
        }
        //if(collectedCount==collectionIDList.Count)
        //{
        //    //컬렉션 버튼 활성화
        //}
        //else
        //{

        //}
    }
}
