
using UnityEngine;

public class NameInput : Singleton<NameInput>
{
    private string playerName;
    [SerializeField] private string defaultName = "에릭";

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            playerName = defaultName;
            Debug.Log("기본 이름인 에릭 으로 진행됩니다.");
        }
        else
        {
            playerName = name;
            Debug.Log($"{playerName} 이름으로 진행됩니다.");
        }
    } 

    public string GetName()
    {
        return playerName;
    }
}
