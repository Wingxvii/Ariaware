using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class QuestBase
{
    //A single list of all quests
    static List<QuestBase> questSingletons;
    protected static List<QuestBase> QuestSingletons
    {
        get { if (questSingletons == null) { questSingletons = new List<QuestBase>(); } return questSingletons; }
    }

    //To flush the list of quests if needed
    public static void Terminate()
    {
        for (int i = 0; i < questSingletons.Count; ++i)
            QuestSingletons[i].DestroySelf();
        QuestSingletons.Clear();
    }

    //To be called in the quests when they're flushed
    protected abstract void DestroySelf();
}

public class QuestObserver<T, U> : QuestBase where T : QuestListener<T, U> where U : QuestEvent<U, T>
{
    //A 'single' singleton - in fact, this can be as many as we want due to the nature of generics
    static QuestObserver<T, U> observer;
    public static QuestObserver<T, U> Observer
    {
        get { if (observer == null) { observer = new QuestObserver<T, U>(); } return observer; }
    }

    //Tells you when a list of listeners is added to the total, as well as addng it to the list of all observers.
    private QuestObserver()
    {
        Debug.Log("ADDED: " + typeof(T));
        QuestSingletons.Add(this);
        questActions = new List<Action<U>>();
    }

    //List of all actions from listeners
    List<Action<U>> questActions;

    //To be used by a listener(s), to add it's own function to the list
    public void AddFunc(Action<U> function)
    {
        questActions.Add(function);
    }

    //Similarly, used to remove a function from the list
    public void RemoveFunc(Action<U> function)
    {
        questActions.Remove(function);
    }

    //When an event is fired, the observer just iterates through its own listener list
    public void FireEvent(U questEvent)
    {
        for (int i = 0; i < questActions.Count; ++i)
        {
            questActions[i](questEvent);
        }
    }

    //Remove all listeners, remove reference to the Observer so it gets collected by garbage collector.
    protected override void DestroySelf()
    {
        questActions.Clear();
        questActions = null;
        observer = null;
    }
}