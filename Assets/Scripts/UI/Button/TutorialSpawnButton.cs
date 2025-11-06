using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpawnButton : MonoBehaviour
{
    public void OnTutorialSpawn(int tutornumber)
    {
        
        switch (tutornumber)
        {
            case 1:
                TutSpawner.Instance.Interact1Tutor(true); break;
            case 2:
                TutSpawner.Instance.Interact2Tutor(true); break;
            case 3:
                TutSpawner.Instance.FarmingTutor(true); break;
            case 4:
                TutSpawner.Instance.QTETutor(true); break;
            case 5:
                TutSpawner.Instance.InventoryTutor(true); break;
            case 6:
                TutSpawner.Instance.WateringTutor(true); break;
            case 7:
                TutSpawner.Instance.FishingTutor(true); break;
            case 8:
                TutSpawner.Instance.ShopTutor(true); break;
            case 9:
                TutSpawner.Instance.QuickSlotTutor(true); break;
            case 10:
                TutSpawner.Instance.Stamina1Tutor(true); break;
            case 11:
                TutSpawner.Instance.Stamina2Tutor(true); break;
            case 12:
                TutSpawner.Instance.AchivementTutor(true); break;
            case 13:
                TutSpawner.Instance.SystemTutor(true); break;
            case 14:
                TutSpawner.Instance.MapTutor(true); break;
            case 15:
                TutSpawner.Instance.SellBoxTutor(true); break;
            case 16:
                TutSpawner.Instance.RunTutor(true); break;
            case 17:
                TutSpawner.Instance.CookTutor(true); break;
            case 18:
                TutSpawner.Instance.InfoTutor(true); break;
            case 19:
                TutSpawner.Instance.OClockTutor(true); break;
            default:
                throw new System.Exception("Undefined Tutorial Key");
        }
    }
}
