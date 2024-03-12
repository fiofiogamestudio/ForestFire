using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [ReadOnly]
    public string nextScene;
    [ReadOnly]
    public bool canStart = false;
    void Awake()
    {
        if (instance == null) instance = this;

        Time.timeScale = 1.0f;
        WorldGenerator.instance.LoadMap();
    }

    public Text DebugText;

    void Start()
    {
        nextScene = WorldGenerator.instance.targetInfo.nextScene;

        StartButton.onClick.AddListener(() =>
        {
            if (canStart)
            {
                StartGame();
                StartButton.gameObject.SetActive(false);
            }
        });

        RestartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
        NextLevelButton.onClick.AddListener(() =>
        {
            if (nextScene == "MenuScene")
            {
                SceneManager.LoadScene("MenuScene");
            }
            else
            {
                GameConfig.MAP_TO_LOAD = nextScene;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        });

        BackButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MenuScene");
        });

        StopButton.onClick.AddListener(() =>
        {
            ContinueButton.gameObject.SetActive(true);
            StopButton.gameObject.SetActive(false);
            StopTime();
        });

        ContinueButton.onClick.AddListener(() =>
        {
            StopButton.gameObject.SetActive(true);
            ContinueButton.gameObject.SetActive(false);
            ContinueTime();
        });


        startLoss = FireSimulator.instance.GetLossCount();
        currentLoss = 0;
    }

    public void StartGame()
    {
        DebugText.text = DebugText.text + "start\n";
        bStartGame = true;
        DebugText.text = DebugText.text + $"set bStartGame as {bStartGame} check bEndGame as {bEndGame}\n";
        DebugText.text = DebugText.text + "before start fire\n";
        try
        {
            // UnitManager.instance.StartFire();
        }
        catch (Exception e)
        {
            DebugText.text = DebugText.text + e.Message + "\n";
        }
        DebugText.text = DebugText.text + "after start fire\n";
        StartButton.gameObject.SetActive(false);

        UnitGroup.gameObject.SetActive(true);
        TimeGroup.gameObject.SetActive(true);


        UnitManager.instance.StartFire();
        ContinueTime();

    }

    [Header("Game")]
    [ReadOnly]
    public bool bStartGame = false;
    [ReadOnly]
    public bool bEndGame = false;

    public GameObject EndGamePanel;

    public Text LossText;
    public Text JudgeText;

    public Button StartButton;
    public Button RestartButton;
    public Button NextLevelButton;

    public Button BackButton;

    public GameObject UnitGroup;


    public GameObject TimeGroup;
    public Button StopButton;
    public Button ContinueButton;

    [ReadOnly]
    public int startLoss = 0;

    [ReadOnly]
    public int currentLoss = 0;

    public int MaxLossAmount = 1000000;
    public Image LossAmountImage;



    [ReadOnly]
    public float timer = 0;

    private const float MAX_TIME = 80;


    public Text LeftTimeText;

    int updateStep = 0;



    public void FixedUpdate()
    {
        // DebugText.text = DebugText.text + "update Step\n";
        if (bStartGame && !bEndGame)
        {
            updateStep++;
            if (updateStep > 5)
            {
                updateStep = 0;


                currentLoss = FireSimulator.instance.GetLossCount() - startLoss;

                LossAmountImage.fillAmount = (float)currentLoss / MaxLossAmount;
            }
        }
    }

    public void Update()
    {
        if (!bStartGame)
        {

        }
        else
        {
            if (!bEndGame)
            {
                timer += Time.deltaTime;
                float reverseTimer = MAX_TIME - timer;
                if (reverseTimer < 0)
                {
                    bEndGame = true;
                    onEndGame();
                }
                else
                {
                    LeftTimeText.text = "<color=red>" + Mathf.Floor(reverseTimer).ToString() + "</color>";
                }
            }
        }
    }

    void onEndGame()
    {
        EndGamePanel.gameObject.SetActive(true);
        LossText.text = currentLoss.ToString();
        if ((float)currentLoss / MaxLossAmount < 0.33f)
        {
            JudgeText.text = "<color=green>好</color>";
        }
        else if ((float)currentLoss / MaxLossAmount < 0.67f)
        {
            JudgeText.text = "<color=yellow>中</color>";
        }
        else
        {
            JudgeText.text = "<color=red>差</color>";
        }

        StopTime();
    }

    public void StopTime()
    {
        Time.timeScale = 0.0f;
    }

    public void ContinueTime()
    {
        Time.timeScale = 1.0f;
    }
}
