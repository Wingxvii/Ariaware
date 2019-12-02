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

        public void GameEndWin() {
            ResourceManager.Instance.gameState = GameState.Win;
            Debug.Log("Game End");
        }

        public void GameEndLose()
        {
            ResourceManager.Instance.gameState = GameState.Loss;
            Debug.Log("Game End");
            missleLaunch.Play("Missle");
            //go to end scene
        }

    }
}