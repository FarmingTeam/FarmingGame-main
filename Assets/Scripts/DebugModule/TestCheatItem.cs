using UnityEngine;

public class TestCheatItem : MonoBehaviour
{
    void Update()
    {
        // F1 키를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // 인벤토리에 1102번 아이템 1개 추가
            PlayerInventory.Instance.AdditemsByID(1102, 1);
            Debug.Log("1102번 아이템 1개 인벤토리에 추가됨 (F1)");
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            // 인벤토리에 1201번 아이템 1개 추가
            PlayerInventory.Instance.AdditemsByID(2010, 1);
            Debug.Log("1201번 아이템 1개 인벤토리에 추가됨 (F5)");
        }
    }
}
