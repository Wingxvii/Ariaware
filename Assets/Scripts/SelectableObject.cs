using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using netcodeRTS;
using RTSManagers;

public enum EntityType
{
    None,
    Wall,
    Barracks,
    Droid,
    Turret,
    Player,
    TOTAL
}

//literally just send all instances of this over to make networking work
//plus MAYBE a few resources

public class SelectableObject : MonoBehaviour
{
    public Behaviour halo;
    public int level = 1;

    public int id;
    public EntityType type;
    public bool destructable = false;
    public static int idtracker { get; private set; }
    public static List<SelectableObject> indexedList = new List<SelectableObject>();

    public bool selected = false;
    public Slider healthBar;
    public RectTransform canvasTransform;

    public int currentHealth = 1;
    public int maxHealth = 1;

    // Start is called before the first frame update
    void Start()
    {
        halo = (Behaviour)this.GetComponent("Halo");
        halo.enabled = false;
        canvasTransform = this.transform.Find("Canvas").GetComponent<RectTransform>();

        id = ++idtracker;
        indexedList.Add(this);
        //call base function
        BaseStart();

    }

    public void OnSelect()
    {
        selected = true;
        if (halo != null) { halo.enabled = true; }
    }
    public void OnDeselect()
    {
        selected = false;
        halo.enabled = false;
    }

    private void Update()
    {
        healthBar.value = (float)currentHealth / (float)maxHealth;
        //call base function
        BaseUpdate();
    }

    private void LateUpdate()
    {
        canvasTransform.eulerAngles = new Vector3(90, 0, 0);
        BaseLateUpdate();
    }

    private void FixedUpdate()
    {
        //call base function
        BaseFixedUpdate();
    }
    public void OnDestroy()
    {
        //call base function
        BaseOnDestory();
    }

    public virtual void OnDamage(int num, SelectableObject culprit) {
        if (destructable || type == EntityType.Player) {
            currentHealth -= num;
        }
        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }


    public void ResetValues()
    {
        this.gameObject.transform.position = Vector3.zero;
        this.gameObject.transform.rotation = Quaternion.identity;
        this.currentHealth = maxHealth;
        this.level = 1;

        BaseResetValues();
    }

    //base class overrides
    protected virtual void BaseStart() { }
    protected virtual void BaseUpdate() { }
    protected virtual void BaseLateUpdate() { }
    protected virtual void BaseFixedUpdate() { }
    protected virtual void BaseOnDestory() { }
    protected virtual void BaseResetValues() { }

    public virtual void IssueLocation(Vector3 location) { }
    public virtual void OnActivation() { }
    public virtual void OnDeactivation() { }


    public virtual void OnDeath() {
        if (gameObject.activeSelf)
        {

            switch (type) {
                case EntityType.Barracks:
                    SelectionManager.Instance.deactivatedObjects[1].Enqueue(this);
                    break;
                case EntityType.Turret:
                    SelectionManager.Instance.deactivatedObjects[0].Enqueue(this);
                    break;
                case EntityType.Wall:
                    SelectionManager.Instance.deactivatedObjects[2].Enqueue(this);
                    break;
                default:
                    break;
            }

            OnDeselect();
            NetworkManager.SendKilledEntity(this);
            SelectionManager.Instance.DeselectItem(this);
            gameObject.SetActive(false);
        }
    }
}
