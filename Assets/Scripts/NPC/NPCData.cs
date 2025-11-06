
using UnityEngine;

[CreateAssetMenu(fileName = "NpcData",menuName = "NpcData") ]
public class NPCData : ScriptableObject
{
    public string NpcID;
    public string NpcName;
    public int NpcAge;
    public bool NpcGender;

    public Sprite sprite;
}
