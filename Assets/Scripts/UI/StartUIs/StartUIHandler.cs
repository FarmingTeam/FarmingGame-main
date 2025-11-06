
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;


public class StartUIHandler : MonoBehaviour
{
    public GameObject startbt;
    public GameObject Newcontinuebt;
    public GameObject Optionbox;
    public GameObject savebox;
    public GameObject uiLoaderPopup;


    public async void Awake()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        startbt.SetActive(true);
        Newcontinuebt.SetActive(false);
        Optionbox.SetActive(false);
        savebox.SetActive(false);
        uiLoaderPopup.SetActive(false);
    }

    public void OnClickstart()
    {
        startbt.SetActive(false);
        Newcontinuebt.SetActive(true);
        savebox.SetActive(false);
    }
    public void OnClicknewstart()
    {
        //SceneManager.LoadScene("PlayerTest");
    }

    public void OnOptionboxbt()
    { 
        Optionbox.SetActive(true);
    }

    public void SaveSelection()
    {
        savebox.SetActive(true);

    }



}
