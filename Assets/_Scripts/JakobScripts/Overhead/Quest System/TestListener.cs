using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Sample class, detailing how listeners are created
public class TestListener : QuestListener<TestListener, TestEvent>
{
    //First calls the base awake to ensure the list of functions is there, then adds its own functions
    protected override void Awake()
    {
        base.Awake();
        AddFunction(UseEvent);
    }

    //UseEvent can be any function, but it must only take in a single event of the corresponding type - in this case, TestEvent
    //This can do anything in theory, but due to it being a test, it's only debugging the message from TestEvent
    protected void UseEvent(TestEvent questEvent)
    {
        Debug.Log(questEvent.message + ", " + name);
    }
}
