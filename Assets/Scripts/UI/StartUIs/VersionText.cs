
using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI versiontext;
    void Start()
    {
        versiontext.text = "v " + Application.version;
    }
}
