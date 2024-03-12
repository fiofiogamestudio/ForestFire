using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TuteSelectionManager : MonoBehaviour
{
    public Transform SelectionRoot;

    public Text DetailTitle;
    public Image DetailIcon;

    public Text DetailDesc;

    [ReadOnly]
    public int SelectedIndex = 0;

    public Button LaunchButton;

    void Awake()
    {
        registerSelection();

        registerLaunchButton();
    }

    void Start()
    {
        SelectTarget(SelectedIndex);

        // GameConfig.MAP_TO_LOAD = "Test";
        // SceneManager.LoadScene("GameScene");
    }

    void registerSelection()
    {
        int count = SelectionRoot.childCount;
        for (int i = 0; i < count; i++)
        {
            int index = i;
            SelectionRoot.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
            {
                UnSelectAll();
                SelectTarget(index);
            });
        }
    }

    void registerLaunchButton()
    {
        LaunchButton.onClick.AddListener(() =>
        {
            // print level name
            var intro = LevelInfoHolder.instance.GetLevelIntro(SelectedIndex);
            string levelName = intro.TargetLevel;
            Debug.Log("Load " + levelName);
            GameConfig.MAP_TO_LOAD = levelName;
            SceneManager.LoadScene("GameScene");
        });
    }

    void UnSelectAll()
    {
        int count = SelectionRoot.childCount;
        for (int i = 0; i < count; i++)
        {
            var child = SelectionRoot.GetChild(i);

            child.GetComponent<Image>().color = Color.white;
        }
    }

    void SelectTarget(int index)
    {
        // Debug.Log(index);
        var child = SelectionRoot.GetChild(index);

        child.GetComponent<Image>().color = Color.grey;

        refreshDetail(index);

        SelectedIndex = index;
    }

    void refreshDetail(int index)
    {
        var intro = LevelInfoHolder.instance.GetLevelIntro(index);
        DetailTitle.text = intro.IntroTitle;
        DetailIcon.sprite = intro.LevelSprite;
        DetailDesc.text = intro.LevelDesc;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
