using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Sample class, detailing how events are created
public class TestEvent : QuestEvent<TestEvent, TestListener>
{
    //Data which must be stored in the event prior to firing
    public string message { get; private set; }

    //By using the constructor to pack data, you can ensure all data gets stored before firing
    public TestEvent(string messageRelay) : base(true)
    {
        message = messageRelay;
    }
}
