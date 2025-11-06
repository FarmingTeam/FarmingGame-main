
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;

    void Start()
    {
        ActivateTab(0);
    }

    public void ActivateTab(int tabNo)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.grey;
        }

        pages[tabNo].SetActive(true);
        tabImages[tabNo].color = Color.white;

        if(tabNo == 1)
            StartCoroutine(TutSpawner.Instance.DisplayTutorial(12));

    }
}
