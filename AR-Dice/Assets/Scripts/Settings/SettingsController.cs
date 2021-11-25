using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour {

    private PersistanceController persistanceController;
    
    [SerializeField] public GameObject presets;
    [SerializeField] public GameObject modPreset;
    [SerializeField] public GameObject themes;
    [SerializeField] public GameObject modTheme;
    [SerializeField] public GameObject help;

    private List<Preset> presetList = null;
    private List<Theme> themesList = null;
    private int selectedPreset;     // 0-n
    private int selectedTheme;      // 0-n
    
    private int modifyPresNumb;
    private List<TextMeshProUGUI> modifyPresetNumbersTexts;
    
    private int modifyThemeNumb;
    private bool toggleColorChange;
    private Image dieNumber;
    private Image dieColor;

    private int presetTouched;
    private int themeTouched;
    private bool pressingButton;
    private float touchStartTime;
    private float touchThreshold = .5f;
    private Touch currTouch;

    void Start() {
        presets.SetActive(false);
        modPreset.SetActive(false);
        themes.SetActive(false);
        modTheme.SetActive(false);
        //help.SetActive(false);

        modifyPresNumb = -1;
        modifyThemeNumb = -1;
        persistanceController = new PersistanceController();
        LoadFiles();

        Container.instance.activePreset = presetList[selectedPreset];
        Container.instance.activeTheme = themesList[selectedTheme];

        modifyPresetNumbersTexts = new List<TextMeshProUGUI>();

        presetTouched = -1;
        themeTouched = -1;
    }
    
    private void LoadFiles() {
        presetList = persistanceController.LoadAllPresets();
        //presetList = persistanceController.ResetPresets());
        
        themesList = persistanceController.LoadAllThemes();
        //themesList = persistanceController.ResetThemes();
        
        List<int> temp = persistanceController.LoadDefaultValues();
        //List<int> temp = persistanceController.ResetDefaultValues();
        selectedPreset = temp[0];
        selectedTheme = temp[1];
    }

    private void Update() {
        if (pressingButton) {
            currTouch = Input.GetTouch(0);
            CheckThreshold();
        }
    }

    public void OnPresetClick(int i) {
        pressingButton = true;
        presetTouched = i;

        touchStartTime = Time.time;
    }
    
    public void OnThemeClick(int i) {
        pressingButton = true;
        themeTouched = i;

        touchStartTime = Time.time;
    }
    
    private void CheckThreshold() {
        if (currTouch.phase == TouchPhase.Ended) {
            pressingButton = false;
            if(presetTouched >= 0)
                OnSelectPreset(presetTouched);
            else if (themeTouched >= 0)
                OnSelectTheme(themeTouched);
        }
        else {
            if (Time.time - touchStartTime > touchThreshold) {
                pressingButton = false;
                if(presetTouched >= 0)
                    OnModifyPreset(presetTouched);
                else if(themeTouched >= 0)
                    OnModifyTheme(themeTouched);
                
                Vibration.Vibrate(100);
            }
        }
    }

    private void SetUpPresets() {
        GameObject items = presets.transform.GetChild(1).gameObject;
        for (int i = 0; i < items.transform.childCount; i++) {
            GameObject presN = items.transform.GetChild(i).gameObject;
            GameObject texts = presN.transform.GetChild(0).gameObject;
            LoadPresetTexts(texts);

            if (i == selectedPreset) {
                modifyPresetNumbersTexts[0].color = Color.white;
                presN.GetComponent<Image>().color = Color.gray;
            }
            else {
                modifyPresetNumbersTexts[0].color = Color.black;
                presN.GetComponent<Image>().color = Color.white;
            }

            for (int j = 1; j < modifyPresetNumbersTexts.Count; j++) {
                modifyPresetNumbersTexts[j].SetText(presetList[i].GetIndex(j-1).ToString());
                if (i == selectedPreset) {
                    modifyPresetNumbersTexts[j].color = Color.white;
                }
                else {
                    modifyPresetNumbersTexts[j].color = Color.black;
                }
            }
            
            modifyPresetNumbersTexts.Clear();
        }
    }
    
    public void OnPresets() {
        presets.SetActive(true);

        SetUpPresets();
    }

    public void OnClosePresets() {
        presets.SetActive(false);
    }
    
    private void LoadPresetTexts(GameObject texts) {
        for (int i = 0; i < texts.transform.childCount; i++) {
            modifyPresetNumbersTexts.Add(texts.transform.GetChild(i).GetComponent<TextMeshProUGUI>());
        }
    }

    private void LoadModPresetTexts(GameObject item) {
        for (int i = 0; i < item.transform.childCount; i++) {
            modifyPresetNumbersTexts.Add(item.transform.GetChild(i).transform.GetChild(2).GetComponent<TextMeshProUGUI>());
        }
    }
    
    private void OnSelectPreset(int k) {
        presetTouched = -1;
        selectedPreset = k;
        Container.instance.activePreset = presetList[selectedPreset];
        
        SetUpPresets();
        
        persistanceController.SaveDefaultValues(selectedPreset, selectedTheme);
    }

    public void OnModifyPreset(int numb) {
        presetTouched = -1;
        modifyPresNumb = numb;
        modPreset.SetActive(true);
        
        // set preset number text
        TextMeshProUGUI presText = modPreset.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        presText.SetText("Preset " + (modifyPresNumb+1));

        GameObject items = modPreset.transform.GetChild(1).gameObject;
        LoadModPresetTexts(items);
        
        for (int i = 0; i < modifyPresetNumbersTexts.Count; i++) {
            modifyPresetNumbersTexts[i].SetText(presetList[modifyPresNumb].GetIndex(i).ToString());
        }
    }

    public void OnPlusButton(int k) {
        if(presetList[modifyPresNumb].Increment(k)) {
            modifyPresetNumbersTexts[k].SetText(presetList[modifyPresNumb].GetIndex(k).ToString());
        }
        else {
            // popup error
        }
    }

    public void OnMinusButton(int k) {
        if(presetList[modifyPresNumb].Decrement(k)) {
            modifyPresetNumbersTexts[k].SetText(presetList[modifyPresNumb].GetIndex(k).ToString());
        }
        else {
            // popup error
        }
    }
    
    public void OnCloseModifyPreset() {
        persistanceController.SavePreset(presetList[modifyPresNumb], modifyPresNumb+1);
        
        modifyPresetNumbersTexts.Clear();
        modPreset.SetActive(false);
        modifyPresNumb = -1;
        
        SetUpPresets();
    }

    public void OnThemes() {
        themes.SetActive(true);
        
        SetUpThemes();
    }
    
    public void OnCloseThemes() {
        themes.SetActive(false);
    }
    
    private void SetUpThemes() {
        GameObject cards = themes.transform.GetChild(1).gameObject;
        for (int i = 0; i < cards.transform.childCount; i++) {
            GameObject cardN = cards.transform.GetChild(i).gameObject;
            
            Image number = cardN.transform.GetChild(0).gameObject.GetComponent<Image>();
            Image die = cardN.transform.GetChild(1).gameObject.GetComponent<Image>();
            TextMeshProUGUI text = cardN.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();

            number.color = themesList[i].GetNumbColor();
            die.color = themesList[i].GetDieColor();
            
            if (selectedTheme == i) {
                cardN.GetComponent<Image>().color = Color.gray;
                text.color = Color.white;
            }
            else {
                cardN.GetComponent<Image>().color = Color.white;
                text.color = Color.black;
            }
        }
    }

    public void OnModifyTheme(int numb) {
        presetTouched = -1;
        modifyThemeNumb = numb;
        toggleColorChange = false;
        modTheme.SetActive(true);

        // set theme number text
        TextMeshProUGUI themeText = modTheme.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        themeText.SetText("Theme " + (modifyThemeNumb+1));

        GameObject preview = modTheme.transform.GetChild(1).transform.GetChild(0).gameObject;
        dieNumber = preview.transform.GetChild(0).GetComponent<Image>();
        dieColor = preview.transform.GetChild(1).GetComponent<Image>();

        dieNumber.color = themesList[modifyThemeNumb].GetNumbColor();
        dieColor.color = themesList[modifyThemeNumb].GetDieColor();
    }
    
    private void OnSelectTheme(int k) {
        themeTouched = -1;
        selectedTheme = k;
        Container.instance.activeTheme = themesList[selectedTheme];
        
        SetUpThemes();
        
        persistanceController.SaveDefaultValues(selectedPreset, selectedTheme);
    }

    public void OnColorChange(Color color) {
        if(!toggleColorChange)
            OnDieColorChange(color);
        else 
            OnNumbColorChange(color);
    }

    private void OnDieColorChange(Color color) {
        dieColor.color = color;
        themesList[modifyThemeNumb].SetDieColor(color);
    }
    
    private void OnNumbColorChange(Color color) {
        dieNumber.color = color;
        themesList[modifyThemeNumb].SetNumbColor(color);
    }

    public void OnToggleColorChange() {
        if (toggleColorChange)
            toggleColorChange = false;
        else
            toggleColorChange = true;
    }

    public void OnCloseModifyTheme() {
        persistanceController.SaveTheme(themesList[modifyThemeNumb], modifyThemeNumb+1);
        
        modTheme.SetActive(false);
        modifyThemeNumb = -1;
        
        SetUpThemes();
    }

    public void OnHelp() {
        help.SetActive(true);
    }
    
    public void OnCloseHelp() {
        help.SetActive(true);
    }
}
