using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using netcodeRTS;
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

    int fixedTimeStep = (int)(1f / Time.fixedDeltaTime);

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
            fixedTimeStep = (int)(1f / Time.fixedDeltaTime);
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
