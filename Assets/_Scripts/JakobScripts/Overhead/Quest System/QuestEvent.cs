using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Events must know which quest they are going to send data to.
//By having a <T, U> and <U, T>, you can easily invert them when you
//  reference them in the corresponding quest.
public abstract class QuestEvent<T, U> where T : QuestEvent<T, U> where U : QuestListener<U, T>
{
    //Static list detailing the amount of events going at once.
    public static int EventsActive { get; private set; } = 0;

    //Final check determining whether or not you want the event to fire
    public bool eventActive { get; private set; } = false;

    //Constructor - half to tick up the EventsActive, half to prompt its creation in derived classes
    protected QuestEvent(bool activated)
    {
        eventActive = activated;
        if (eventActive) { ++EventsActive; }
    }

    //Destructor to tick down EventsActive
    ~QuestEvent()
    {
        if (eventActive) { --EventsActive; }
    }

    //Fire an event by sending it to the observer. Check if active first.
    public static void FireEvent(T questEvent)
    {
        //Debug.Log("FIRING! " + questEvent.GetType());
        if (questEvent.eventActive)
            QuestObserver<U, T>.Observer.FireEvent(questEvent);
    }
}