using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GameController : MonoBehaviour {

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;

    private SwipeModeController swipeModeController;
    private FallingModeController fallingModeController;

    private List<GameObject> dice;
    private GameObject instantiatedDie;

    private int currentDie;
    

    void Start() {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        currentDie = 0;

        dice = Container.instance.dice;
        
        swipeModeController = new SwipeModeController();
        fallingModeController = new FallingModeController();

        if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            instantiatedDie = Instantiate(dice[currentDie]);
            swipeModeController.Die = instantiatedDie;
        } else if (Container.instance.throwMode == ThrowMode.FALLING) {
            
        }
    }
    
    void Update() {
        if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            if (!swipeModeController.IsThrowed) {
                swipeModeController.UpdateDiePosition();
            } else {
                // pick and set
                // check die result
            }

            if (swipeModeController.IsThrowable) {
                swipeModeController.SwipeDie();
            }
        } else if(Container.instance.throwMode == ThrowMode.FALLING) {
            
        }
    }

    private void SetupSwipeToThrow() {
        instantiatedDie = Instantiate(dice[currentDie]);
    }
}
