using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingUIManager : MonoBehaviour
{
    public Button BackButton;
    // Start is called before the first frame update
    void Start()
    {
        BackButton.onClick.AddListener(()=>{
            SceneManager.LoadScene("MenuScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
