using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateNetwork : MonoBehaviour
{
    //this is for example, not to be actually used
    #region SingletonCode
    private static UpdateNetwork _instance;
    public static UpdateNetwork Instance { get { return _instance; } }
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

    int fixedTimeStep = 50;

    private void FixedUpdate()
    {



        #region Fixed Tick
        //count down
        --fixedTimeStep;

        //tick is called 10 times per 50 updates
        if (fixedTimeStep % 5 == 0)
        {
            TickUpdate();
        }

        //reset the clock
        if (fixedTimeStep <= 0)
        {
            //updates 50Hz
            fixedTimeStep = 50;
        }
        #endregion

    }
    void Update()
    {

    }
    void TickUpdate()
    {
        NetworkManager.SendDroidPositions();
    }
    private void LateUpdate()
    {
        
    }

}
