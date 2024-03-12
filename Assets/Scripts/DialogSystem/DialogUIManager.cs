using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogUIManager : MonoBehaviour
{
    [Header("Dialog UI")]
    // public GameObject PlayerDialogPanel;
    // public GameObject NPCDialogPanel;
    public List<GameObject> HolderPanelList;
    public int Startid = 0;
    public List<Text> HolderDialogTextList;
    // public Text PlayerDialogText;
    // public Text NPCDialogText;
    public GameObject DialogueCanvas;
    public SelectionPanelController SelectionPanel;

    [ReadOnly]
    public bool IsSelecting = false;

    public static DialogUIManager instance;
    void Awake()
    {
        if (instance == null) instance = this;

    }

    void Start()
    {
        if (GetComponent<AudioSource>() == null)
        {
            this.gameObject.AddComponent<AudioSource>();
            GetComponent<AudioSource>().playOnAwake = false;
            GetComponent<AudioSource>().loop = false;
        }
        // if (QueryButton)
        // {
        //     QueryButton.gameObject.SetActive(false);
        //     QueryButton.onClick.AddListener(() =>
        //     {
        //         // 检查质疑结果
        //         if (CurrentQuery != null)
        //         {
        //             if (CurrentDialog.dialogID == CurrentQuery.targetDialog)
        //             {
        //                 int nextId = CurrentQuery.successNextDialogID;
        //                 checkEnterQuery(nextId);
        //                 executeEffect(CurrentCallback);
        //                 LoadDialog(nextId);
        //             }
        //             else
        //             {
        //                 int nextId = CurrentQuery.failedNextDialogID;
        //                 checkEnterQuery(nextId);
        //                 executeEffect(CurrentCallback);
        //                 LoadDialog(nextId);
        //             }
        //         }
        //         else
        //         {
        //             int nextId = CurrentQuery.failedNextDialogID;
        //             checkEnterQuery(nextId);
        //             executeEffect(CurrentCallback);
        //             LoadDialog(nextId);
        //         }
        //     });

        // }
        Startid = WorldGenerator.instance.targetInfo.startID;

        if (Startid != 0)
        {
            LoadDialog(Startid);
            Debug.Log("start dialog");
        }
        else CloseDialog();
    }


    void Update()
    {
        UpdateDialogContentProgress();

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && (DialogueCanvas.activeSelf))
        {
            if (CurrentDialogProgress < CurrentDialogLength)
            {
                CurrentDialogProgress = CurrentDialogLength;
                RefreshDialogContent();
            }
            else NextDialog();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) CloseDialog();
    }

    public void CloseDialog()
    {
        // PlayerDialogPanel.gameObject.SetActive(false);
        // NPCDialogPanel.gameObject.SetActive(false);
        // PlayerDialogText.text = "";
        // NPCDialogText.text = "";

        foreach (var panel in HolderPanelList)
        {
            panel.gameObject.SetActive(false);
        }

        foreach (var dialogText in HolderDialogTextList)
        {
            dialogText.text = "";
        }

        DialogueCanvas.gameObject.SetActive(false);

    }

    private void clearPanel()
    {
        foreach (var panel in HolderPanelList)
        {
            panel.gameObject.SetActive(false);
        }

    }

    private void showPanel(int index)
    {
        if (HolderPanelList.Count > index)
        {
            HolderPanelList[index].SetActive(true);
        }
    }

    private void clearText()
    {
        foreach (var text in HolderDialogTextList)
        {
            text.text = "";
        }

    }

    private void showText(int index, string content)
    {
        if (HolderDialogTextList.Count > index)
        {
            HolderDialogTextList[index].text = content;
        }
    }

    private void setTextColor(Color color)
    {
        foreach (var text in HolderDialogTextList)
        {
            text.color = color;
        }
    }

    void OpenDialog()
    {
        if (DialogueCanvas)
        { DialogueCanvas.SetActive(true); }
        // switch (CurrentHolderID)
        // {
        //     case 1:
        //         PlayerDialogPanel.gameObject.SetActive(true);
        //         NPCDialogPanel.gameObject.SetActive(false);
        //         break;
        //     case 2:
        //         NPCDialogPanel.gameObject.SetActive(true);
        //         PlayerDialogPanel.gameObject.SetActive(false);
        //         break;
        // }
        clearPanel();
        showPanel(CurrentHolderID - 1); // HolderId 1 -> Panel index 0
        // PlayerDialogText.text = "";
        // NPCDialogText.text = "";
        clearText();

        // set face
        DialogController controller = HolderPanelList[CurrentHolderID - 1].GetComponent<DialogController>();

        if (controller != null && CurrentFaceID != 0)
        {
            controller.SetFace(CurrentFaceID);
        }

    }




    public void LoadDialog(int id)
    {
        DialogHolder.DialogBase dialog = DialogHolder.instance.GetDialog(id);
        if (dialog != null)
        {
            CurrentDialog = dialog;
            CurrentDialogContent = dialog.content;
            CurrentDialogLength = dialog.content.Length;
            CurrentDialogProgress = 0;
            CurrentHolderID = dialog.holderID;
            CurrentFaceID = dialog.faceID;
            CurrentCallback = dialog.dialogCallback;

            OpenDialog();

        }
        else
        {
            Debug.LogError("failed to load dialog id : " + id);
        }
    }

    public void NextDialog()
    {
        if (CurrentDialog == null)
        {
            Debug.LogError("failed to load next dialog, no current dialog!");
        }

        if (IsSelecting)
        {
            return;
        }


        switch (CurrentDialog.dialogType)
        {
            case DialogHolder.DialogType.DialogSequence:
                nextDialogSequence(CurrentDialog as DialogHolder.DialogSequece);
                break;
            case DialogHolder.DialogType.DialogSelection:
                nextDialogSelection(CurrentDialog as DialogHolder.DialogSelection);
                break;
            case DialogHolder.DialogType.DialogPackage:
                nextPackageSelection(CurrentDialog as DialogHolder.DialogPackage);
                break;
        }
    }

    // Query

    [ReadOnly]
    public DialogHolder.Query CurrentQuery;

    [ReadOnly]
    public bool QueryMode = false;

    public Button QueryButton;

    private void checkEnterQuery(int id)
    {
        DialogHolder.Query query = null;
        if (DialogHolder.instance.IsInQuery(id, ref query))
        {
            Debug.Log("query");
            setTextColor(Color.green);
            RefreshDialogContent();

            if (query != null)
            {
                CurrentQuery = query;
                QueryMode = true;
                QueryButton.gameObject.SetActive(true);
            }

        }
        else
        {
            CurrentQuery = null;
            QueryMode = false;
            if (QueryButton != null)
            { QueryButton.gameObject.SetActive(false); }


            setTextColor(Color.white);
        }

    }

    void nextDialogSequence(DialogHolder.DialogSequece dialog)
    {
        // if (!QueryMode)
        {
            // checkEnterQuery(dialog.nextDialogID);
            // if (!QueryMode)
            {
                Debug.Log("nextDialog " + dialog.nextDialogID);
                int nextDialogID = dialog.nextDialogID;
                if (nextDialogID <= 0)
                {
                    executeEffect(CurrentCallback);
                    CloseDialog();
                }
                else
                {
                    // Handle dialog callback
                    // handleEvent
                    executeEffect(CurrentCallback);
                    LoadDialog(nextDialogID);
                }
                return;
            }
        }

        // handle query mode
        int count = CurrentQuery.dialogList.Length;
        int index = -1;
        for (int i = 0; i < count; i++)
        {
            if (CurrentQuery.dialogList[i] == dialog.dialogID)
            {
                index = i;
                break;
            }
        }
        int nextIndex = (index + 1) % count;
        int nextId = CurrentQuery.dialogList[nextIndex];
        executeEffect(CurrentCallback);
        LoadDialog(nextId);
    }

    void nextDialogSelection(DialogHolder.DialogSelection dialog)
    {
        Debug.Log("nextSelectoin " + dialog.selectionList.Length);
        IsSelecting = true;
        openSelection(dialog.selectionList);
    }

    void nextPackageSelection(DialogHolder.DialogPackage dialog)
    {
        Debug.Log("nextPackage " + dialog.targetInventoryID);
        IsSelecting = true;
        openInventory();
    }


    void openSelection(int[] selectionList)
    {
        // foreach (var selection in selectionList)
        // {
        //     Debug.Log("selection " + selection);
        // }
        SelectionPanel.OpenSelection(selectionList);
    }

    void openInventory()
    {
        // InventoryUIManager.instance.OpenTarget();
    }

    public int GetDialogPackageTarget()
    {
        if (CurrentDialog != null)
        {
            var dialogPackage = CurrentDialog as DialogHolder.DialogPackage;
            return dialogPackage.targetInventoryID;
        }
        return 0;
    }

    public void HandleDialogPackage(bool success)
    {
        if (CurrentDialog != null)
        {
            var dialogPackage = CurrentDialog as DialogHolder.DialogPackage;
            if (success)
            {
                LoadDialog(dialogPackage.successNextDialogID);
                Debug.Log("success call " + dialogPackage.successCallback);
                executeEffect(dialogPackage.successCallback);
            }
            else
            {
                LoadDialog(dialogPackage.failedNextDialogID);
                Debug.Log("failed call " + dialogPackage.failedCallback);
                executeEffect(dialogPackage.failedCallback);
            }
        }
    }

    private List<string> temp_args = new List<string>();

    private void executeEffect(string effect)
    {
        if (effect == null || effect.Length == 0) return;
        Debug.Log("execute effect: " + effect);

        temp_args.Clear();
        string[] strs = effect.Split(",");
        string to_call = strs[0];
        for (int i = 1; i < strs.Length; i++)
        {
            temp_args.Add(strs[i].Trim()); // remove space
        }
        // custom invoke using reflection
        System.Reflection.MethodInfo methodInfo = GetType().GetMethod(to_call, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (methodInfo != null)
        {
            methodInfo.Invoke(this, null);
        }
        else
        {
            Debug.LogWarning("method " + to_call + " not found!");
        }
    }

    private void change_hp()
    {
        // int id = System.Convert.ToInt32(temp_args[0]);
        // int count = System.Convert.ToInt32(temp_args[1]);
        // DialogUIManager.instance.ChangeHP(id, count);
        // Debug.Log("change " + id + " " + count);
        // GameManager.instance.AddFood(count);
    }

    private void test_noarg()
    {
        Debug.Log("test noargs");
    }

    private void start_fire()
    {
        // Game
        // GameManager.instance.StartGame();
        // UnitManager.instance.StartFire();
        GameManager.instance.canStart = true;
    }

    private void end_dialog()
    {
        SceneManager.LoadScene("Chapter1");
    }

    private void back_menu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    private void begin_event()
    {
        string name = System.Convert.ToString(temp_args[0]);
        string fangfa = System.Convert.ToString(temp_args[1]);
        Debug.Log(name);
        Debug.Log(fangfa);
        System.Type type = System.Type.GetType(fangfa);
        GameObject obj = GameObject.Find(name);
        Component component = obj.GetComponent(type);
        System.Reflection.MethodInfo method = type.GetMethod("Begin_Event");
        method.Invoke(component, null);
    }

    private void start_chase()
    {
        // TODO
    }












    [ReadOnly]
    public DialogHolder.DialogBase CurrentDialog;
    [ReadOnly]
    public int CurrentHolderID;
    [ReadOnly]
    public string CurrentDialogContent;
    [ReadOnly]
    public int CurrentDialogLength = 0;
    [ReadOnly]
    public int CurrentDialogProgress = 0;

    [ReadOnly]
    public int CurrentFaceID;

    [ReadOnly]
    public string CurrentCallback;

    public int DialogSpeed = 10;

    public List<AudioClip> DialogClipList;

    /// <summary>
    /// 几个字播放一次音效
    /// </summary>
    public int NumToAudio = 1;
    [ReadOnly]
    public int dialogAudioCounter = 0;

    private float dialogTimer = 0.0f;
    private float dialogTime
    {
        get { return 1.0f / DialogSpeed; }
    }
    void UpdateDialogContentProgress()
    {
        dialogTimer += Time.deltaTime;
        if (dialogTimer > dialogTime)
        {
            dialogTimer = 0;
            if (CurrentDialogProgress < CurrentDialogLength)
            {
                if (CurrentDialogContent[CurrentDialogProgress] == '<')
                {
                    int rightBraceCount = 0;
                    int endFlagCount = 0;
                    while (true)
                    {
                        CurrentDialogProgress++;
                        if (CurrentDialogContent[CurrentDialogProgress] == '/')
                        {
                            endFlagCount++;
                        }
                        if (CurrentDialogContent[CurrentDialogProgress] == '>')
                        {
                            rightBraceCount++;
                            if (endFlagCount != 0 && rightBraceCount == endFlagCount * 2)
                            {
                                break;
                            }
                        }
                    }
                    CurrentDialogProgress++;
                }
                else
                {
                    CurrentDialogProgress++;
                }
                RefreshDialogContent();


                dialogAudioCounter++;
                if (dialogAudioCounter >= NumToAudio)
                {
                    // GetComponent<AudioSource>().PlayOneShot(DialogClipList[CurrentHolderID - 1]);
                    dialogAudioCounter = 0;
                }
            }
        }
    }

    void RefreshDialogContent()
    {
        // switch (CurrentHolderID)
        // {
        //     case 1:
        //         // Player
        //         PlayerDialogText.text = CurrentDialogContent.Substring(0, CurrentDialogProgress);
        //         break;
        //     case 2:
        //         // NPC
        //         NPCDialogText.text = CurrentDialogContent.Substring(0, CurrentDialogProgress);
        //         break;
        // }
        // Debug.Log("Refresh " + CurrentDialogContent);
        showText(CurrentHolderID - 1, CurrentDialogContent.Substring(0, CurrentDialogProgress));
    }

}
