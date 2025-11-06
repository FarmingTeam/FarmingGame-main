using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSign : MonoBehaviour
{
    public void SignOn()
    {
        gameObject.SetActive(true);
    }

    public void SignOff()
    {
        gameObject.SetActive(false);
    }
}
