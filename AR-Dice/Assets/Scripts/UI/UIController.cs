using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    [SerializeField] private GameObject DiceMode;
    [SerializeField] private GameObject SlidingMenu;
    
    // Start is called before the first frame update
    void Start() {
        DiceMode.SetActive(true);
        SlidingMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
