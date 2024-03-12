using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

[RequireComponent(typeof(Button))]
public class SelectionButton : MonoBehaviour
{
    public Text buttonText;
    void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(() =>
        {
            panelController.CloseSelection();
            DialogUIManager.instance.LoadDialog(nextDialogID);
            executeEffect(callback);
        });
    }

    [ReadOnly]
    public int nextDialogID;
    [ReadOnly]
    public string callback;


    private SelectionPanelController panelController;

    public void BindSelectionPanel(SelectionPanelController panelController)
    {
        this.panelController = panelController;
    }

    public void RefreshSelectionInfo(int id)
    {
        DialogHolder.DialogSelectionBranch branchInfo = DialogHolder.instance.GetSelection(id);
        if (branchInfo == null)
        {
            Debug.LogError("failed to refresh selectoin id : " + id);
        }

        buttonText.text = branchInfo.content;
        nextDialogID = branchInfo.nextDialogID;
        callback = branchInfo.selectionCallback;
    }
    private List<string> temp_args = new List<string>();

    private void executeEffect(string effect)
    {
        Debug.Log("execute effect: " + effect);

        temp_args.Clear();
        string[] strs = effect.Split(",");
        string to_call = strs[0];
        for (int i = 1; i < strs.Length; i++)
        {
            temp_args.Add(strs[i].Trim()); // remove space
        }
        // custom invoke using reflection
        MethodInfo methodInfo = GetType().GetMethod(to_call, BindingFlags.NonPublic | BindingFlags.Instance);
        if (methodInfo != null)
        {
            methodInfo.Invoke(this, null);
        }
        else
        {
            Debug.LogWarning("method " + to_call + " not found!");
        }
    }

}
