using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GameController : MonoBehaviour {

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;

    private List<GameObject> dice;

    void Start() {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        dice = Container.instance.dice;
    }
    
    void Update() {
        if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            
        } else if(Container.instance.throwMode == ThrowMode.FALLING) {
            
        }
    }
}
