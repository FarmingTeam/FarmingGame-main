using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Collection
{
    public int collectionID;
    public string collectionName;
    public string collectionDescription;
    public List<RequiredItem> requiredItemList = new List<RequiredItem>();
    public int collectionState; //0이면 안모은거, 1이면 모은거(보상은 안받음), 2면(보상도 받음) 
    public RewardType rewardType;
    public int rewardAmount;

    public Collection(int collectionID, string collectionName, string collectionDescription, string archive_1, string archive_1Amount, string archive_2, string archive_2Amount, string rewardType, string rewardAmount)
    {
        this.collectionID = collectionID;
        this.collectionName = collectionName;
        this.collectionDescription = collectionDescription;

        ParseArchive(archive_1, archive_1Amount);
        ParseArchive(archive_2, archive_2Amount);

        this.rewardAmount = int.Parse(rewardAmount);



    }
    //이렇게 먼저 리소스 매니저에서 컬렉션 데이터 만들어두기


    public void ParseArchive(string archive, string amount)
    {


        string[] a2 = null;
        if (archive != "null")
        {
            a2 = archive.Split("/").Select(x => x.Trim()).ToArray();
            int amountNum = int.Parse(amount);
            if (a2 != null)
            {
                foreach (string a in a2)
                {
                    var num = int.Parse(a);
                    RequiredItem requiredItem = new RequiredItem(num, amountNum);
                    requiredItemList.Add(requiredItem);
                }
            }
        }






    }


}



public enum RewardType
{
    Coin
}


[Serializable]
public class RequiredItem
{
    public int itemID;
    public int amount;

    public RequiredItem(int itemId, int amount)
    {
        this.itemID = itemId;
        this.amount = amount;

    }
}
