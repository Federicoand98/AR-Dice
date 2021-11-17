using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
