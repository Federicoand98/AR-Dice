using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Container : MonoBehaviour {

    #region Singleton

    public static Container instance;

    private void Awake() {
        if (instance != null) {
            Debug.Log("More than one instance of Container found!");
            return;
        }

        instance = this;
        errorDictionary.Add("del_anchor", "Tap again to delete the anchor");
    }

    #endregion

    [SerializeField] public ThrowMode throwMode;
    [SerializeField] public List<GameObject> dice;
    [SerializeField] public List<Sprite> diceSprites;
    [SerializeField] public List<Sprite> gameModesSprites;
    [SerializeField] public List<Sprite> tableModeSprites;
    [SerializeField] public Sprite presetSprite;
    [SerializeField] public Material themeDie;  //base without color
    [SerializeField] public Material themeNumber;  //base without color
    [SerializeField] public Dictionary<string, string> errorDictionary = new Dictionary<string, string>();

    public List<GameObject> tableMeshes = new List<GameObject>();
    public Pose pointerPosition;
    public bool pointerPositionIsValid = false;
    public bool tableModeIsEnabled = false;
    public bool tableConstraint = false;
    public bool themeChanged = false;

    public Preset activePreset;
    public Theme activeTheme;

}
