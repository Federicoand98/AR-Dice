using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GameController : MonoBehaviour {

    [SerializeField] private Button prevButton;
    [SerializeField] private Button currButton;
    [SerializeField] private Button nextButton;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;

    private SwipeModeController swipeModeController;
    private FallingModeController fallingModeController;

    private List<GameObject> dice;
    private List<Sprite> diceSprites;
    private GameObject instantiatedDie;
    private Rigidbody rigidbody;

    private int currentDie;

    private bool throwed = false;
    private bool throwable = true;
    

    void Start() {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        currentDie = 0;

        dice = Container.instance.dice;
        diceSprites = Container.instance.diceSprites;
        
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

    public void NextDieHand() {
        currentDie++;

        if (currentDie == dice.Count) {
            currentDie = 0;
        }

        if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            Destroy(instantiatedDie);
            instantiatedDie = Instantiate(dice[currentDie]);
            rigidbody = instantiatedDie.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;

            swipeModeController.Die = instantiatedDie;
        
            if (throwable) {
                Vector3 v = new Vector3(instantiatedDie.transform.position.x, 
                                            instantiatedDie.transform.position.y, 1f);
                
                instantiatedDie.transform.position = v;
            } else if (throwed) {
                throwable = true;
                throwed = false;
            }
        }

        if (currentDie == 0) {
            prevButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = diceSprites[dice.Count - 1];
        }
        else {
            prevButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = diceSprites[currentDie - 1];
        }

        currButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = diceSprites[currentDie];

        if (currentDie == dice.Count) {
            nextButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = diceSprites[0];
        }
        else {
            nextButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = diceSprites[currentDie + 1];
        }
    }
    
    public void PreviousDieHand() {
        currentDie--;

        if (currentDie == -1) {
            currentDie = dice.Count;
        }

        if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            Destroy(instantiatedDie);
            instantiatedDie = Instantiate(dice[currentDie]);
            rigidbody = instantiatedDie.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            
            swipeModeController.Die = instantiatedDie;
        
            if (throwable) {
                Vector3 v = new Vector3(instantiatedDie.transform.position.x, 
                                            instantiatedDie.transform.position.y, 1f);
                
                instantiatedDie.transform.position = v;
            } else if (throwed) {
                throwable = true;
                throwed = false;
            }
        }

        if (currentDie == 0) {
            prevButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = diceSprites[dice.Count - 1];
        }
        else {
            prevButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = diceSprites[currentDie - 1];
        }

        currButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = diceSprites[currentDie];

        if (currentDie == dice.Count) {
            nextButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = diceSprites[0];
        }
        else {
            nextButton.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = diceSprites[currentDie + 1];
        }
    }

    public void SwitchModeHand() {
        
    }

    public void CollimationModeHand() {
        
    }

    public void CurrentDieHand() {
        
    }
}
