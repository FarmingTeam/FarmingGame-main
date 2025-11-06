using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Minimap : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI locationText;

    [SerializeField] private Button btnFarm;
    [SerializeField] private Button btnRoad;
    [SerializeField] private Button btnTown;

    [SerializeField] private GameObject farmMinimap;
    [SerializeField] private GameObject roadMinimap;
    [SerializeField] private GameObject townMinimap;

    private void Awake()
    {
        btnFarm.onClick.AddListener(OnclickFarmButton);
        btnRoad.onClick.AddListener(OnclickRoadButton);
        btnTown.onClick.AddListener(OnclickTownButton);
    }

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        
        if (sceneName == "TestInHouseScene")
        {
            ShowMinimap("Farm");
        }


        else if(sceneName == "TestFarmScene")
        {
            ShowMinimap("Farm");
        }

        else if (sceneName == "TestMidwayScene")
        {
            ShowMinimap("Road");
        }

        else if (sceneName == "TestTownScene")
        {
            ShowMinimap("Town");
        }

        else if (sceneName == "TestInShopScene")
        {
            ShowMinimap("Town");
        }

        UpdateLocationText();
    }

    public void ShowMinimap(string areaName)
    {
        if (farmMinimap != null)
        {
            farmMinimap.SetActive(false);
        }
        if (roadMinimap != null)
        {
            roadMinimap.SetActive(false);
        }
        if (townMinimap != null)
        {
            townMinimap.SetActive(false);
        }

        if (areaName == "Farm" && farmMinimap != null)
        {
            farmMinimap.SetActive(true);
        }

        else if (areaName == "Road" && roadMinimap != null)
        {
            roadMinimap.SetActive(true);
        }

        else if (areaName == "Town" && townMinimap != null)
        {
            townMinimap.SetActive(true);
        }

        StartCoroutine(TutSpawner.Instance.DisplayTutorial(14));
    }

    public void UpdateLocationText()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "TestInHouseScene")
        {
            locationText.text = "현재 위치는\n농장\n입니다";
        }

        else if (sceneName == "TestFarmScene")
        {
            locationText.text = "현재 위치는\n농장\n입니다";
        }

        else if (sceneName == "TestMidwayScene")
        {
            locationText.text = "현재 위치는\n갈림길\n입니다";
        }

        else if (sceneName == "TestTownScene")
        {
            locationText.text = "현재 위치는\n마을\n입니다";
        }

        else if (sceneName == "TestInShopScene")
        {
            locationText.text = "현재 위치는\n마을\n입니다";
        }

        else
        {
            locationText.text = "잘못된 곳입니다.";
        }
    }

    public void OnclickFarmButton()
    {
        ShowMinimap("Farm");
    }

    public void OnclickRoadButton()
    {
        ShowMinimap("Road");
    }
    public void OnclickTownButton()
    {
        ShowMinimap("Town");
    }
}
