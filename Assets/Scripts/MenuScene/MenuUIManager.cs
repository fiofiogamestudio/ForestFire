using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    [Header("Page Base")]
    [ReadOnly]
    public int StartIndex = 0;
    private const int MAX_PAGE = 3;
    private const int PAGE_WIDTH = 1920;
    private const int PAGE_HEIGHT = 1080;
    public RectTransform PageRoot;
    [ReadOnly]
    public Vector3 TargetPagePos = Vector3.zero;


    [Header("Page 1")]
    public Button StartButton;
    public Button QuitButton;

    [Header("Page 2")]
    public Button TuteModeButton;
    public Button CaseModeButton;
    public Button FreeModeButton;

    public Button BackToOneButton;

    public Button BackToTowButton;
    [Header("Page 3 - Tute Mode")]
    public GameObject TuteGroup;

    [Header("Page 3 - Case Mode")]
    public GameObject CaseGroup;


    [Header("Page 3 - Free Mode")]
    public GameObject FreeGroup;



    private List<GameObject> SwitchGroup = new List<GameObject>();

    void Awake()
    {
        Time.timeScale = 1.0f;
        // Register Button
        StartButton.onClick.AddListener(() =>
        {
            ToPage(1);
        });
        QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        TuteModeButton.onClick.AddListener(() =>
        {
            ToPage(2);
        });
        CaseModeButton.onClick.AddListener(() =>
        {
            ToPage(2);
        });
        FreeModeButton.onClick.AddListener(() =>
        {
            ToPage(2);
        });
        BackToOneButton.onClick.AddListener(() =>
        {
            ToPage(0);
        });
        BackToTowButton.onClick.AddListener(() =>
        {
            ToPage(1);
        });




        // Init Page
        ToPage(0);


        // Init Switch Group
        SwitchGroup.Add(TuteGroup);
        SwitchGroup.Add(CaseGroup);
        SwitchGroup.Add(FreeGroup);


    }

    public void FixedUpdate()
    {
        PageRoot.position = Vector3.Lerp(PageRoot.position, TargetPagePos, 0.15f);
    }

    public void ToPage(int index)
    {
        if (index >= 0 && index < MAX_PAGE)
        {
            TargetPagePos = new Vector3(-PAGE_WIDTH * index + PAGE_WIDTH / 2, PAGE_HEIGHT / 2, 0);
        }
    }


    public void OpenSwitch(int index)
    {
        if (index >= 0 && index < SwitchGroup.Count)
        {
            foreach (var group in SwitchGroup) group.SetActive(false);
        }
        SwitchGroup[index].SetActive(true);
    }


}
