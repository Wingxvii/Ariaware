using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectionUI : MonoBehaviour
{
    #region SingletonCode
    private static SelectionUI _instance;
    public static SelectionUI Instance { get { return _instance; } }
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

    public List<Button> pages;
    public int currPage = 1;
    public int itemsPerPage;
    public int numOfPages = 1;

    //these are the UI Parameters
    public float XStart = -215;
    public float XOffset = 99;
    public int numOfCols = 14;

    public float YStart = -400;
    public float YOffset = -55;
    public int numOfRows = 2;

    //prefabs
    public Sprite ButtonDroid;
    public Sprite ButtonBarracks;
    public Sprite ButtonWall;
    public Sprite ButtonTurret;
    public GameObject ButtonPrefab;

    public GameObject UIParent;

    public List<GameObject> buttonPool;

    // Start is called before the first frame update
    void Start()
    {
        //fill positions in slot
        for (int counter2 = 0; counter2 < numOfRows; counter2++)
        {
            for (int counter = 0; counter < numOfCols; counter++)
            {
                Vector3 pos = new Vector3(XStart + (counter * XOffset), YStart + (counter2 * YOffset), 0.0f);
                buttonPool.Add(GameObject.Instantiate(ButtonPrefab, pos + UIParent.transform.position, Quaternion.identity));
            }
        }

        foreach (GameObject button in buttonPool)
        {
            button.transform.SetParent(UIParent.transform);
            button.gameObject.SetActive(false);
        }


        itemsPerPage = buttonPool.Count;

        foreach (Button button in pages)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void ProcessUI(bool resetPage) {

        //resets
        if (resetPage) { currPage = 1; }
        foreach (GameObject button in buttonPool)
        {
            button.gameObject.SetActive(false);
        }
        foreach (Button page in pages) {
            page.gameObject.SetActive(false);
        }
        numOfPages = 1;


        //calculate num of pages
        if (SelectionManager.Instance.SelectedObjects.Count > itemsPerPage) {
            numOfPages = SelectionManager.Instance.SelectedObjects.Count/itemsPerPage;
            numOfPages++;
        }

        //show page tabs
        for (int counter = 1; counter <= pages.Count; counter++)
        {
            if (counter <= numOfPages && numOfPages != 1)
            {
                pages[counter-1].gameObject.SetActive(true);
            }
        }

        //display based on the page
        int counterOffset = (currPage - 1) * itemsPerPage;

        for (int counter = counterOffset; counter < SelectionManager.Instance.SelectedObjects.Count; counter++) {

            //counter surpass items exception
            if (counter >= currPage * itemsPerPage) {
                break;
            }

            buttonPool[counter - counterOffset].SetActive(true);
                switch (SelectionManager.Instance.SelectedObjects[counter].type)
                {
                    case EntityType.Barracks:
                        buttonPool[counter - counterOffset].GetComponent<Image>().sprite = ButtonBarracks;
                        break;
                    case EntityType.Droid:
                        buttonPool[counter - counterOffset].GetComponent<Image>().sprite = ButtonDroid;
                        break;
                    case EntityType.Wall:
                        buttonPool[counter - counterOffset].GetComponent<Image>().sprite = ButtonWall;
                        break;
                    case EntityType.Turret:
                        buttonPool[counter - counterOffset].GetComponent<Image>().sprite = ButtonTurret;
                        break;
                }

            buttonPool[counter - counterOffset].GetComponent<SelectionButton>().OnCreate(SelectionManager.Instance.SelectedObjects[counter]);
        }
    }

    public void OnPageSwitch( int page) {
        currPage = page;
        ProcessUI(false);
    }

    public void OnElementSelected(SelectionButton button) {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl))
        {
            SelectionManager.Instance.DeselectItem(button.parentObject);
        }
        else {
            SelectionManager.Instance.OnFocusSelected(button.parentObject);
        }
    }

}



