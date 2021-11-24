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
    private int modifyThemeNumb;
    private List<TextMeshProUGUI> modifyPresetNumbersTexts;

    private int presetTouched;
    private bool pressingButton;
    private float touchStartTime;
    private float touchThreshold = .5f;
    private Touch currTouch;

    void Start() {
        presets.SetActive(false);
        modPreset.SetActive(false);
        themes.SetActive(false);
        //modTheme.SetActive(false);
        //help.SetActive(false);

        modifyPresNumb = -1;
        modifyThemeNumb = -1;
        persistanceController = new PersistanceController();
        LoadFiles();

        Container.instance.activePreset = presetList[selectedPreset];
        Container.instance.activeTheme = themesList[selectedTheme];

        modifyPresetNumbersTexts = new List<TextMeshProUGUI>();

        presetTouched = -1;
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
    
    private void CheckThreshold() {
        if (currTouch.phase == TouchPhase.Ended) {
            pressingButton = false;
            OnSelectPreset(presetTouched);
        }
        else {
            if (Time.time - touchStartTime > touchThreshold) {
                pressingButton = false;
                OnModifyPreset(presetTouched);
                Vibration.Vibrate(100);
            }
        }
    }

    private void LoadFiles() {
        presetList = persistanceController.LoadAllPresets();
        themesList = persistanceController.LoadAllThemes();
        List<int> temp = persistanceController.LoadDefaultValues();
        selectedPreset = temp[0];
        selectedTheme = temp[1];
    }
    
    private void SetUpThemes() {
        GameObject cards = themes.transform.GetChild(1).gameObject;
        for (int i = 0; i < cards.transform.childCount; i++) {
            GameObject cardN = cards.transform.GetChild(i).gameObject;
            
            Image bg = cards.transform.GetChild(0).gameObject.GetComponent<Image>();
            Image mid = cards.transform.GetChild(1).gameObject.GetComponent<Image>();
            TextMeshProUGUI text = cards.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();

            Debug.Log(text);

            bg.color = new Color(themesList[i].numbR, themesList[i].numbG, themesList[i].numbB);
            mid.color = new Color(themesList[i].dieR, themesList[i].dieG, themesList[i].dieB);
            
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
        selectedPreset = k;
        Container.instance.activePreset = presetList[selectedPreset];
        
        SetUpPresets();
        
        persistanceController.SaveDefaultValues(selectedPreset, selectedTheme);
    }

    public void OnModifyPreset(int numb) {
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

    public void OnModifyTheme(int numb) {
        modifyThemeNumb = numb;
        modTheme.SetActive(true);
        
        // etc
    }

    public void OnCloseModifyTheme() {
        // save
        
        modTheme.SetActive(false);
        modifyThemeNumb = 0;
    }

    public void OnHelp() {
        help.SetActive(true);
    }
    
    public void OnCloseHelp() {
        help.SetActive(true);
    }
}
