using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    #region SingletonCode
    private static AnimationManager _instance;
    public static AnimationManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    //single pattern ends here
    #endregion  

    public Animator Movement;
    public Animator Attack;

    public void PlayMove(Vector3 location) {
        Movement.transform.position = location;
        Movement.Play("IssueMovement");
    }
    public void PlayAttack(Vector3 location)
    {
        Attack.transform.position = location;
        Attack.Play("IssueMovement");
    }

}
