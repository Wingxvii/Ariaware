using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSManagers;

public interface ICommand
{
    void ExecuteAction();
    void UnExecuteAction();
    void Cleanup();
}
public enum BuildingActions
{
    Upgrade,
    Delete,
}

class AddCommand : ICommand
{
    private SelectableObject buildingElement;
    bool done = false;

    public AddCommand(SelectableObject building)
    {
        buildingElement = building;
        ExecuteAction();
    }

    public void ExecuteAction()
    {
        buildingElement.gameObject.SetActive(true);
        buildingElement.OnActivation();
        SelectionManager.Instance.AllObjects.Add(buildingElement);
        done = true;
    }
    public void UnExecuteAction()
    {
        buildingElement.OnDeactivation();
        buildingElement.gameObject.SetActive(false);
        SelectionManager.Instance.AllObjects.Remove(buildingElement);
        done = false;
    }
    public void Cleanup()
    {
        if (!done)
        {
            Object.Destroy(buildingElement);
        }
    }
}

class UpgradeCommand : ICommand
{
    private SelectableObject buildingElement;
    bool done = false;

    public UpgradeCommand(SelectableObject building)
    {
        buildingElement = building;
        ExecuteAction();
    }

    public void ExecuteAction()
    {
        buildingElement.level++;
        Transform objTransf = buildingElement.GetComponent<Transform>();

        //make it bigger
        objTransf.localScale = new Vector3(objTransf.localScale.x * 1.1f, objTransf.localScale.y * 1.1f, objTransf.localScale.z * 1.1f);
        //make sure its anchored on the ground
        objTransf.position = new Vector3(objTransf.position.x, 0 + objTransf.localScale.y * 0.5f, objTransf.position.z);
        done = true;
    }
    public void UnExecuteAction()
    {
        buildingElement.level--;
        Transform objTransf = buildingElement.GetComponent<Transform>();

        //make it smaller
        objTransf.localScale = new Vector3(objTransf.localScale.x * 0.9f, objTransf.localScale.y * 0.9f, objTransf.localScale.z * 0.9f);
        //make sure its anchored on the ground
        objTransf.position = new Vector3(objTransf.position.x, 0 + objTransf.localScale.y * 0.5f, objTransf.position.z);
        done = false;
    }
    public void Cleanup() {
        //nothing needs to be done
    }
}

class DeleteCommand : ICommand
{
    private SelectableObject buildingElement;
    bool done = false;

    public DeleteCommand(SelectableObject building)
    {
        buildingElement = building;
        ExecuteAction();
    }

    public void ExecuteAction()
    {
        buildingElement.OnDeactivation();
        buildingElement.gameObject.SetActive(false);
        SelectionManager.Instance.AllObjects.Remove(buildingElement);
        done = true;
    }
    public void UnExecuteAction()
    {
        buildingElement.gameObject.SetActive(true);
        buildingElement.OnActivation();
        SelectionManager.Instance.AllObjects.Add(buildingElement);
        done = false;
    }
    public void Cleanup()
    {
        if (done) {
            Object.Destroy(buildingElement);
        }
    }
}