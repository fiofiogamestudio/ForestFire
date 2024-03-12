using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DialogHolder : MonoBehaviour
{
    [System.Serializable]
    public enum DialogType
    {
        DialogSequence,
        DialogSelection,
        DialogPackage
    }

    [System.Serializable]
    public class DialogBase
    {
        public int dialogID;
        public DialogType dialogType;
        public int holderID;
        public string content;
        public string dialogCallback;

        public int faceID;
    }

    [System.Serializable]
    public class DialogSequece : DialogBase
    {
        public int nextDialogID; // -1
    }

    [System.Serializable]
    public class DialogSelection : DialogBase
    {
        public int[] selectionList;
    }

    [System.Serializable]
    public class DialogPackage : DialogBase
    {
        public int targetInventoryID;
        public int successNextDialogID;
        public int failedNextDialogID;
        public string successCallback;
        public string failedCallback;
    }

    [System.Serializable]
    public class DialogSelectionBranch
    {
        public int selectionID;
        public int nextDialogID;
        public string content;
        public string selectionCallback;

    }

    [System.Serializable]
    public class DialogWrapper
    {
        public DialogBase[] dialogData;
    }

    [System.Serializable]
    public class BranchWrapper
    {
        public DialogSelectionBranch[] branchData;
    }

    public List<DialogBase> DialogList = new List<DialogBase>();


    public List<DialogSelectionBranch> BranchList = new List<DialogSelectionBranch>();

    public List<Query> QueryList = new List<Query>();


    [System.Serializable]
    public class Query
    {
        public int[] dialogList;
        public int targetDialog;
        public int successNextDialogID;
        public int failedNextDialogID;
    }

    [System.Serializable]
    public class QueryWrapper
    {
        public Query[] queryData;
    }


    public static DialogHolder instance;
    public void Awake()
    {
        if (instance == null) instance = this;

        // Load Dialog List
        LoadDialogList();

        // Load Branch List
        LoadBranchList();

        // string fullname = typeof(DialogSequece).AssemblyQualifiedName;
        // Debug.Log(fullname);

        // Load Query List
        LoadQueryList();
    }

    public void LoadDialogList(string file = "dialog")
    {
        DialogList = LoadDialogListStatic(file);
    }

    public static List<DialogBase> LoadDialogListStatic(string file)
    {
        List<DialogBase> DialogList = new List<DialogBase>();

        TextAsset textAsset = Resources.Load<TextAsset>("Dialog/" + file);
        if (textAsset == null)
        {
            Debug.LogError("failed to load dialog.json!");
            return new List<DialogBase>();
        }

        DialogWrapper dialogWrapper = JsonConvert.DeserializeObject<DialogWrapper>(textAsset.text, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
        });

        foreach (var dialog in dialogWrapper.dialogData)
        {
            DialogList.Add(dialog);
        }

        return DialogList;
    }

    public void LoadBranchList(string file = "selection")
    {
        BranchList = LoadBranchListStatic(file);
    }

    public static List<DialogSelectionBranch> LoadBranchListStatic(string file)
    {
        List<DialogSelectionBranch> BranchList = new List<DialogSelectionBranch>();

        TextAsset textAsset = Resources.Load<TextAsset>("Dialog/selection");
        if (textAsset == null)
        {
            Debug.LogError("failed to load selection.json!");
            return new List<DialogSelectionBranch>();
        }
        BranchWrapper branchWrapper = JsonUtility.FromJson<BranchWrapper>(textAsset.text);

        BranchList.Clear();

        foreach (var branch in branchWrapper.branchData)
        {
            BranchList.Add(branch);
        }

        return BranchList;
    }

    public void LoadQueryList(string file = "query")
    {
        QueryList = LoadQueryListStatic(file);
    }

    public static List<Query> LoadQueryListStatic(string file)
    {
        List<Query> QueryList = new List<Query>();

        TextAsset textAsset = Resources.Load<TextAsset>("Dialog/query");
        if (textAsset == null)
        {
            Debug.LogError("failed to load query.json");
            return new List<Query>();
        }

        QueryWrapper queryWrapper = JsonUtility.FromJson<QueryWrapper>(textAsset.text);

        QueryList.Clear();

        foreach (var query in queryWrapper.queryData)
        {
            QueryList.Add(query);
        }

        return QueryList;
    }


    public DialogBase GetDialog(int id)
    {
        foreach (var dialog in DialogList)
        {
            if (dialog.dialogID == id)
            {
                return dialog;
            }
        }
        return null;
    }

    public DialogSelectionBranch GetSelection(int id)
    {
        foreach (var branch in BranchList)
        {
            if (branch.selectionID == id)
            {
                return branch;
            }
        }
        return null;
    }

    public bool IsInQuery(int id, ref Query outQuery)
    {
        foreach (var query in QueryList)
        {
            foreach (var dialog in query.dialogList)
            {
                if (id == dialog)
                {
                    outQuery = query;
                    return true;
                }
            }
        }
        return false;
    }

}
