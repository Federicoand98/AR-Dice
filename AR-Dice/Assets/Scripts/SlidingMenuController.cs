using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingMenuController : MonoBehaviour {
    
    private GameObject sideButton;
    private RectTransform thisRect;
    
    // Start is called before the first frame update
    void Start() {
        thisRect = gameObject.GetComponent<RectTransform>();
        sideButton = gameObject.transform.GetChild(1).gameObject;
        sideButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ToggleSideButton() {
        if(sideButton.activeSelf)
            sideButton.SetActive(false);
        else
            sideButton.SetActive(true);
    }
    
    public void MenuButtonHand() {
        LeanTween.move(thisRect, Vector3.zero, .25f).setOnComplete(ToggleSideButton);
    }

    public void CloseMenuHand() {
        Debug.Log("chiudi stammerda");
        LeanTween.move(thisRect, new Vector3(-600, 0,0), .25f).setOnComplete(ToggleSideButton);
    }
    
    public void ThemesMenuHand() {
        
    }
    
    public void PresetMenuHand() {
        
    }
    
    public void HelpMenuHand() {
        
    }
}
