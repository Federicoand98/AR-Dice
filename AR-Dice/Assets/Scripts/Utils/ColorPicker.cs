using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;


[Serializable]
public class ColorEvent : UnityEvent<Color> { }

public class ColorPicker : MonoBehaviour {

    private RectTransform Rect;
    private Texture2D ColorTexture;

    public TextMeshProUGUI DebugText;
    public ColorEvent OnColorSelect;

    // Start is called before the first frame update
    void Start() {
        Rect = GetComponent<RectTransform>();

        ColorTexture = GetComponent<Image>().mainTexture as Texture2D;
    }

    // Update is called once per frame
    void Update() {
        if (Input.touchCount > 0) {
            if (RectTransformUtility.RectangleContainsScreenPoint(Rect, Input.GetTouch(0).position)) {
                Vector2 delta;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, Input.GetTouch(0).position, null,
                    out delta);

                //string debug = "Mouse Position : " + Input.mousePosition;
                //debug += "<br>delta : " + delta;

                float width = Rect.rect.width;
                float height = Rect.rect.height;
                delta += new Vector2(width * .5f, height * .5f);
                // debug += "<br>offset delta : " + delta;

                float x = Mathf.Clamp(delta.x / width, 0f, 1f);
                float y = Mathf.Clamp(delta.y / height, 0f, 1f);
                
                //debug += "<br>x : " + x;
                //debug += "<br>y : " + y;

                int texX = Mathf.RoundToInt(x * ColorTexture.width);
                int texY = Mathf.RoundToInt(y * ColorTexture.height);

                //debug += "<br>Tex x : " + texX;
                //debug += "<br>Tex y : " + texY;

                Color color = ColorTexture.GetPixel(texX, texY);

                if (color.a != 0) {
                    //DebugText.color = color;
                    //DebugText.text = debug;
                    
                    OnColorSelect?.Invoke(color);
                }
            }
        }
    }
}