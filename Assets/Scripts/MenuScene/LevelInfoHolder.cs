using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfoHolder : MonoBehaviour
{
    [System.Serializable]
    public class LevelIntro
    {
        [ReadOnly]
        public string IntroTitle;
        [ReadOnly]
        public Sprite LevelSprite;
        [ReadOnly]
        public string LevelDesc;
    }

    public List<LevelIntro> LevelIntroList = new List<LevelIntro>();

    [System.Serializable]
    public class LevelIntroData
    {
        public string IntroTitle;
        public string LevelSpritePath;
        public string LevelDesc;
    }

    [System.Serializable]
    public class LevelIntroJson
    {
        public LevelIntroData[] datas;
    }

    public static LevelInfoHolder instance;
    public void Awake()
    {
        if (instance == null) instance = this;
    }


    private const string LEVEL_INTRO_PATH = "LevelIntro/LevelIntro";
    [ContextMenu("Load")]
    public void LoadLevelIntroJson()
    {
        LevelIntroJson jsonObject = DataLoader.LoadJson<LevelIntroJson>(LEVEL_INTRO_PATH);

        LevelIntroList.Clear();
        foreach (var data in jsonObject.datas)
        {
            LevelIntro intro = new LevelIntro();
            intro.IntroTitle = data.IntroTitle;
            // intro.sp
            intro.LevelDesc = data.LevelDesc;
            LevelIntroList.Add(intro);
        }
    }



    public LevelIntro GetLevelIntro(int index)
    {
        if (index < 0 || index >= LevelIntroList.Count)
        {
            Debug.LogError("try to get LevelIntro : index out of range!");
            return null;
        }
        return LevelIntroList[index];
    }

}
