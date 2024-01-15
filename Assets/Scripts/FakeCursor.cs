using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FakeCursor : MonoBehaviour
{
    public Image CursorImage;
    private Canvas canvas;

    private void Start()
    {
        // Hide the actual system cursor
        Cursor.visible = false;

        // Assuming your Image is a child of a Canvas
        canvas = CursorImage.canvas;
    }

    void Update()
    {
        FollowCursor();
    }

    void FollowCursor()
    {
        // 1. Get the mouse position in screen space
        Vector2 mousePosition = Input.mousePosition;

        // 2. Convert the screen position to world position on the Canvas
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.transform as RectTransform,
            mousePosition,
            canvas.worldCamera,
            out Vector3 worldPoint);

        // 3. Set the Image position
        CursorImage.transform.position = worldPoint;
    }
}
