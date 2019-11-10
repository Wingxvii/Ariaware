using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using netcodeRTS;
using RTSManagers;

namespace RTSUI
{

    public class UIManager : MonoBehaviour
    {
        #region SingletonCode
        private static UIManager _instance;
        public static UIManager Instance { get { return _instance; } }
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

        public GameObject UIBuilding;
        public GameObject UIBarracks;
        public GameObject UIDroid;
        public GameObject UITurret;
        public GameObject UIWall;
        public GameObject UIPlayer;
        public GameObject UISelection;

        // Start is called before the first frame update
        void Start()
        {
            UIBarracks.SetActive(false);
            UIDroid.SetActive(false);
            UITurret.SetActive(false);
            UIWall.SetActive(false);
            UIPlayer.SetActive(false);
            UISelection.SetActive(false);

        }

        // Update is called once per frame
        void Update()
        {
            //use observer for this
            if (SelectionManager.Instance.SelectedObjects.Count > 0)
            {
                switch (SelectionManager.Instance.PrimarySelectable.type)
                {
                    case EntityType.Barracks:
                        EnableUI(UIBarracks);
                        GetStats(UIBarracks, SelectionManager.Instance.PrimarySelectable);
                        break;

                    case EntityType.Droid:
                        EnableUI(UIDroid);
                        GetStats(UIDroid, SelectionManager.Instance.PrimarySelectable);
                        break;
                    case EntityType.Turret:
                        EnableUI(UITurret);
                        GetStats(UITurret, SelectionManager.Instance.PrimarySelectable);
                        break;
                    case EntityType.Wall:
                        EnableUI(UIWall);
                        GetStats(UIWall, SelectionManager.Instance.PrimarySelectable);
                        break;
                    case EntityType.Player:
                        EnableUI(UIPlayer);
                        GetStats(UIPlayer, SelectionManager.Instance.PrimarySelectable);
                        break;

                    default:
                        EnableUI();
                        break;
                }

                if (SelectionManager.Instance.SelectedObjects.Count > 1)
                {
                    UISelection.SetActive(true);
                }
                else
                {
                    UISelection.SetActive(false);
                }
            }
            else
            {
                EnableUI(UIBuilding);
                UISelection.SetActive(false);
            }
        }

        void EnableUI(GameObject enabledUI = null)
        {

            if (!enabledUI.activeSelf)
            {
                UIBuilding.SetActive(false);
                UIBarracks.SetActive(false);
                UIDroid.SetActive(false);
                UITurret.SetActive(false);
                UIWall.SetActive(false);
                UIPlayer.SetActive(false);

                if (enabledUI != null)
                {
                    enabledUI.SetActive(true);
                }
            }
        }

        void GetStats(GameObject UI, SelectableObject obj)
        {
            UI.transform.Find("Health").GetComponent<UnityEngine.UI.Text>().text = obj.currentHealth.ToString() + "/" + obj.maxHealth.ToString();
        }
    }

}