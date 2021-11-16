using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceModeController : MonoBehaviour {

    private bool animation = false;
    private RectTransform rectMenu;
    private float wSlide;
    private float wGap;
    
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (animation) {
            wSlide = wSlide - wGap;
            rectMenu.anchoredPosition.Set(-wSlide, 0);

            if (wSlide >= 0)
                animation = false;
        }
    }
    
    public void MenuButtonHand(GameObject menu) {
        menu.SetActive(true);
        rectMenu = menu.GetComponent<RectTransform>();
        wSlide = rectMenu.rect.width * .73f;
        wGap = wSlide * .001f;

        animation = true;
    }

    public void SwitchModeHand() {
        
    }

    public void CollimationModeHand() {
        
    }

    public void CurrentDieHand() {
        
    }
    
    public void PreviousDieHand() {
        
    }
    
    public void NextDieHand() {
        
    }
}
