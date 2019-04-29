using UnityEngine;

public class Gravitized : MonoBehaviour
{
    public bool byAll = false;
    public eType typeOfG = eType.other;

    public enum eType
    {
        player,
        enemy,
        other
    }
}
