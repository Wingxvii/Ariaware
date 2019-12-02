using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RTSManagers
{
    public class RTSGameManager : MonoBehaviour
    {
        public GameObject spawnpoint;
        public GameObject endPoint;

        public Animation missleLaunch;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void GameEndWin()
        {
            missleLaunch.Play("Missle");
            //go to end scene
        }

    }
}