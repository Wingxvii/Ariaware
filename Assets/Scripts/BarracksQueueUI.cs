using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTSManagers;

namespace RTSUI {
    public class BarracksQueueUI : MonoBehaviour
    {

        #region SingletonCode
        private static BarracksQueueUI _instance;
        public static BarracksQueueUI Instance { get { return _instance; } }
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

        public GameObject ButtonPrefab;
        public Slider buildProcess;
        public GameObject UIParent;
    
        public float XStart = 210;
        public float XOffset = -15;
        public int numOfCols = 24;
    
        public float YStart = -6.5f;
        public float YOffset = 0;
        public int numOfRows = 1;
    
        public List<GameObject> buttonPool;
        public Barracks parentSelected;
    
    
        // Start is called before the first frame update
        void Start()
        {
            //add first one
            Vector3 pos = new Vector3(-200 , YStart , 0.0f);
            buttonPool.Add(GameObject.Instantiate(ButtonPrefab, pos + UIParent.transform.position, Quaternion.identity));
            buttonPool[0].GetComponent<QueueButton>().queuePlace = 0;

            //fill positions in slot
            for (int counter2 = 0; counter2 < numOfRows; counter2++)
            {
                for (int counter = 0; counter < numOfCols; counter++)
                {
                    pos = new Vector3(XStart + (counter * XOffset), YStart + (counter2 * YOffset), 0.0f);
                    buttonPool.Add(GameObject.Instantiate(ButtonPrefab, pos + UIParent.transform.position, Quaternion.identity));
    
                    buttonPool[buttonPool.Count-1].GetComponent<QueueButton>().queuePlace = counter;
                }
            }

            foreach (GameObject button in buttonPool)
            {
                button.transform.SetParent(UIParent.transform);
                button.gameObject.SetActive(false);
            }
        }

        private void Update()
        {

            //This calculates wether the UI is shown to the player
            if (SelectionManager.Instance.PrimarySelectable != null && SelectionManager.Instance.PrimarySelectable.type == EntityType.Barracks && SelectionManager.Instance.SelectedObjects.Count == 1)
            {
                UIParent.SetActive(true);
                if (parentSelected != (Barracks)SelectionManager.Instance.PrimarySelectable) {
                    parentSelected = (Barracks)SelectionManager.Instance.PrimarySelectable;
                }
                UpdateQueue();
            }
            else {
                UIParent.SetActive(false);
            }

            //this keeps the slider updated
            if (UIParent.activeSelf && buttonPool[0].activeSelf)
            {
                buildProcess.value = parentSelected.buildProcess.value;
            }
            else if (UIParent.activeSelf) {
                buildProcess.value = 0;
            }
        }

        public void SetSelectedBarrack(Barracks selected) {
            parentSelected = selected;
            UpdateQueue();
        }

        public void UpdateQueue() {

            //resets each button
            foreach (GameObject button in buttonPool)
            {
                button.gameObject.SetActive(false);
            }

            if (parentSelected.currentBuildTime > 0)
            {
                buttonPool[0].SetActive(true);
            }


            if (parentSelected.buildTimes.Count > 0)
            {
                for (int counter = 0; counter < parentSelected.buildTimes.Count; counter++)
                {
                    buttonPool[buttonPool.Count - counter - 1].SetActive(true);
                }
            }
        }
    }
}