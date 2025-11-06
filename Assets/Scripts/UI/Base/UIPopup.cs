
using UnityEngine;

public class UIPopup : UIBase
{
    protected override void OnOpen()
    {
        base.OnOpen();
        UIManager.Instance.uiStack.Push(this);

    }

    protected override void OnClose()
    {
        base.OnClose();

    }
}
