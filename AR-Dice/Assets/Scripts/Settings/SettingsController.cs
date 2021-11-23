using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TMPro;
using UnityEngine;

public class SettingsController : MonoBehaviour {

    private PersistanceController persistanceController;
    
    [SerializeField] public GameObject presets;
    [SerializeField] public GameObject modPreset;
    [SerializeField] public GameObject themes;
    [SerializeField] public GameObject modTheme;
    [SerializeField] public GameObject help;

    private List<Preset> presetList = null;
    private List<Theme> themesList = null;
    private int selectedPreset;
    private int selectedTheme;
    
    private int modifyPresNumb;
    private int modifyThemeNumb;
    private List<TextMeshProUGUI> modifyPresetNumbersTexts;


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
    }

    private void LoadFiles() {
        presetList = persistanceController.LoadAllPresets();
        themesList = persistanceController.LoadAllThemes();
        List<int> temp = persistanceController.LoadDefaultValues();
        selectedPreset = temp[0];
        selectedTheme = temp[1];
    }

    public void OnThemes() {
        themes.SetActive(true);
    }
    
    public void OnCloseThemes() {
        themes.SetActive(false);
    }
    
    public void OnPresets() {
        presets.SetActive(true);
    }

    public void OnClosePresets() {
        presets.SetActive(false);
    }

    private void LoadPresetNumbersTexts(GameObject items) {
        for (int i = 0; i < items.transform.childCount; i++) {
            modifyPresetNumbersTexts.Add(items.transform.GetChild(i).transform.GetChild(2).GetComponent<TextMeshProUGUI>());
        }
    }

    public void OnModifyPreset(int numb) {
        modifyPresNumb = numb;
        modPreset.SetActive(true);
        
        // set preset number text
        TextMeshProUGUI presText = modPreset.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        presText.SetText("Preset " + (modifyPresNumb+1));

        GameObject items = modPreset.transform.GetChild(1).gameObject;
        LoadPresetNumbersTexts(items);
        
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
        // save
        
        modPreset.SetActive(true);
        modifyPresNumb = 0;
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
