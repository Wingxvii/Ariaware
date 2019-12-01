using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Quests must know which event they are going to respond to.
//By having a <T, U> and <U, T>, you can easily invert them when you
//  reference them in the corresponding event.
public abstract class QuestListener<T, U> : MonoBehaviour where T : QuestListener<T, U> where U : QuestEvent<U, T>
{
    //List of all it's own functions
    List<Action<U>> functions;

    //create the list of its own functions on awake
    protected virtual void Awake()
    {
        functions = new List<Action<U>>();
    }

    //destroy the list of functions when it's destroyed, easy due to it knowing all its functions
    protected void OnDestroy()
    {
        for (int i = 0; i < functions.Count; ++i)
        {
            QuestObserver<T, U>.Observer.RemoveFunc(functions[i]);
        }

        functions.Clear();
    }

    //Base function to add a function to the observer (and itself)
    public void AddFunction(Action<U> function)
    {
        functions.Add(function);
        QuestObserver<T, U>.Observer.AddFunc(function);
    }
}