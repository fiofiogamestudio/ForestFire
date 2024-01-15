using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugUIManager : MonoBehaviour
{
    public Slider WindXSlider;
    public Slider WindYSlider;

    public Slider SpeedSlider;

    public FireSimulator simulator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public static GameManager instance;
    public Vector2 GetWind()
    {
        return new Vector2(WindXSlider.value, WindYSlider.value);
    }

    void Awake()
    {
        Screen.SetResolution(1920, 1080, false);

        WindXSlider.onValueChanged.AddListener((float value) =>
        {
            Vector2 wind = new Vector2(WindXSlider.value, WindYSlider.value);
            simulator.SetWind(wind);
        });

        WindYSlider.onValueChanged.AddListener((float value) =>
        {
            Vector2 wind = new Vector2(WindXSlider.value, WindYSlider.value);
            simulator.SetWind(wind);
        });

        SpeedSlider.onValueChanged.AddListener((float value) =>
        {
            simulator.SetSpeed(SpeedSlider.value);
        });
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
