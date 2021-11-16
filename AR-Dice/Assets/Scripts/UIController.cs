using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    [SerializeField] private GameObject DiceMode;
    [SerializeField] private GameObject SlidingMenu;
    
    // Start is called before the first frame update
    void Start() {
        DiceMode.SetActive(true);
        
        SlidingMenu.SetActive(false);
        RectTransform rectMenu = SlidingMenu.GetComponent<RectTransform>();
        float wSlide = rectMenu.rect.width * .73f;
        rectMenu.anchoredPosition.Set(-wSlide, 0);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
