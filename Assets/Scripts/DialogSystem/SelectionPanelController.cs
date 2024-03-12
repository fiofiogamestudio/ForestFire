using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectionPanelController : MonoBehaviour
{
    public GameObject SelectionPrefab;

    [ReadOnly]
    public List<SelectionButton> SelectionButtonList = new List<SelectionButton>();
    public void OpenSelection(int[] selectionList)
    {
        clearChildren();

        gameObject.SetActive(true);


        foreach (var selection in selectionList)
        {
            GameObject go = GameObject.Instantiate(SelectionPrefab);
            go.transform.parent = transform;
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            SelectionButton selectionButton = go.GetComponent<SelectionButton>();
            selectionButton.BindSelectionPanel(this);
            selectionButton.RefreshSelectionInfo(selection);
            SelectionButtonList.Add(selectionButton);
        }
    }

    public void CloseSelection()
    {
        gameObject.SetActive(false);

        List<SelectionButton> buttonList = new List<SelectionButton>();
        foreach (var button in SelectionButtonList) buttonList.Add(button);
        foreach (var button in buttonList) GameObject.Destroy(button);
        SelectionButtonList.Clear();

        DialogUIManager.instance.IsSelecting = false;
    }


    void clearChildren()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

    }
}
