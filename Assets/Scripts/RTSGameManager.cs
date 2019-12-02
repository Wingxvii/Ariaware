using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RTSManagers
{
    public class RTSGameManager : MonoBehaviour
    {

        #region SingletonCode
        private static RTSGameManager _instance;
        public static RTSGameManager Instance { get { return _instance; } }
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
        #endregion

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

        public void GameEndWin()
        {
            Debug.Log("Game End");
            missleLaunch.Play("Missle");
            //go to end scene
        }

    }
}