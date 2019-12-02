using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneManagement;
using netcodeRTS;

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
            ScenePresent.Instance.gameState = 1;
            NetworkManager.SendGameData(2, 0.0f);
            ScenePresent.Instance.SwapScene(4);

        }

        public void GameEndLose()
        {
            ResourceManager.Instance.gameState = GameState.Loss;
            Debug.Log("Game End");
            ScenePresent.Instance.gameState = 2;
            NetworkManager.SendGameData(3, 0.0f);
            //go to end scene
            ScenePresent.Instance.SwapScene(4);

        }

    }
}