using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageRootPreviewer : MonoBehaviour
{
    [System.Serializable]
    public enum PageNumber
    {
        PageOne,
        PageTwo,
        PageThree
    }

    public MenuUIManager menuUIManager;
    public PageNumber PageToPreview;

    [ContextMenu("Preview Page")]
    public void PreviewPage()
    {
        switch (PageToPreview)
        {
            case PageNumber.PageOne:
                menuUIManager.ToPage(0);
                break;
            case PageNumber.PageTwo:
                menuUIManager.ToPage(1);
                break;
            case PageNumber.PageThree:
                menuUIManager.ToPage(2);
                break;
        }
        menuUIManager.PageRoot.transform.position = menuUIManager.TargetPagePos;
    }
}
