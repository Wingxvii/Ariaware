using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSourceEvent : QuestEvent<PlayerSourceEvent, PlayerSourceListener>
{
    public WritePlayerData WPD;

    public PlayerSourceEvent(WritePlayerData playerWriter) : base(true)
    {
        WPD = playerWriter;
    }
}
