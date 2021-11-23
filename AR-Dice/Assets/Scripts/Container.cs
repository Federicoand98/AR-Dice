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
    }

    #endregion

    [SerializeField] public ThrowMode throwMode;
    [SerializeField] public List<GameObject> dice;
    [SerializeField] public List<Sprite> diceSprites;
    [SerializeField] public List<Sprite> gameModesSprites;
    [SerializeField] public List<Sprite> tableModeSprites;

    public Pose pointerPosition;
    public bool pointerPositionIsValid = false;
    public bool tableModeIsEnabled = false;
    public bool tableConstraint = false;

    public Preset activePreset;
    public Theme activeTheme;

}
