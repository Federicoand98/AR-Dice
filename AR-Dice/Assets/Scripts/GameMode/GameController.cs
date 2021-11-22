using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class GameController : MonoBehaviour {

    [SerializeField] private Button prevButton;
    [SerializeField] private Button currButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button tableButton;
    [SerializeField] private Button modeButton;
    [SerializeField] private GameObject pointer;
    [SerializeField] private GameObject tableModeController;
    [SerializeField] private GameObject diceChooser;
    [SerializeField] private GameObject tableButtons;
    [SerializeField] private GameObject sessionOrigin;
    [SerializeField] private GameObject resultView;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;
    private ARTogglePlaneDetection arTogglePlaneDetection;

    private SwipeModeController swipeModeController;
    private FallingModeController fallingModeController;

    private List<GameObject> dice;
    private List<Sprite> diceSprites;
    private List<Sprite> modeSprites;
    private List<Sprite> tableSprites;
    private GameObject instantiatedDie;
    private Rigidbody rb;
    private TextMeshPro textMeshPro;
    private Image previousButtonImage;
    private Image nextButtonImage;
    private Image currentButtonImage;
    private Image tableButtonImage;
    private Image throwButtonImage;

    private int currentDie;

    private bool throwed = false;
    private bool throwable = true;
    

    void Start() {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        arTogglePlaneDetection = sessionOrigin.GetComponent<ARTogglePlaneDetection>();

        currentDie = 0;

        dice = Container.instance.dice;
        diceSprites = Container.instance.diceSprites;
        modeSprites = Container.instance.gameModesSprites;
        tableSprites = Container.instance.tableModeSprites;

        textMeshPro = resultView.transform.GetChild(0).transform.gameObject.GetComponent<TextMeshPro>();

        throwButtonImage = modeButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        tableButtonImage = tableButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        previousButtonImage = prevButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        nextButtonImage = nextButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        currentButtonImage = currButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        
        swipeModeController = new SwipeModeController();
        fallingModeController = new FallingModeController();

        if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            SetupSwipeToThrow();
        } else if (Container.instance.throwMode == ThrowMode.FALLING) {
            SetupFallingDices();
        }
    }
    
    void Update() {
        if(!Container.instance.tableModeIsEnabled) {
            if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
                if (!swipeModeController.IsThrowed) {
                    swipeModeController.UpdateDiePosition();
                } else {
                    swipeModeController.PickAndSet();
                    // check die result
                }

                if (swipeModeController.IsThrowable) {
                    swipeModeController.SwipeDie();
                }
            } else if(Container.instance.throwMode == ThrowMode.FALLING) {
                // check die result
            }
        }
    }

    private void SetupSwipeToThrow() {
        pointer.SetActive(false);
        instantiatedDie = Instantiate(dice[currentDie]);
        swipeModeController.Die = instantiatedDie;
    }
    
    private void SetupFallingDices() {
        if (instantiatedDie != null) {
            Destroy(instantiatedDie);
        }
        
        pointer.SetActive(true);
    }

    public void OnThrowModeButton() {
        if(!Container.instance.tableConstraint) {
            if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
                Container.instance.throwMode = ThrowMode.FALLING;
                throwButtonImage.sprite = modeSprites[1];
            
                SetupFallingDices();
            } else {
                Container.instance.throwMode = ThrowMode.SWIPE_TO_THROW;
                throwButtonImage.sprite = modeSprites[0];
            
                SetupSwipeToThrow();
            }
        }
    } 

    public void OnNextDieButton() {
        currentDie++;

        if (currentDie == dice.Count) {
            currentDie = 0;
        }

        if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            Destroy(instantiatedDie);
            instantiatedDie = Instantiate(dice[currentDie]);
            rb = instantiatedDie.GetComponent<Rigidbody>();
            rb.isKinematic = true;

            swipeModeController.Die = instantiatedDie;
        
            if (swipeModeController.IsThrowable) {
                Vector3 v = new Vector3(instantiatedDie.transform.position.x, instantiatedDie.transform.position.y, 1f);
                instantiatedDie.transform.position = v;
            } else if (swipeModeController.IsThrowed) {
                swipeModeController.IsThrowable = true;
                swipeModeController.IsThrowed = false;
            }
        }

        if (currentDie == 0) {
            previousButtonImage.sprite = diceSprites[dice.Count - 1];
        } else {
            previousButtonImage.sprite = diceSprites[currentDie - 1];
        }

        currentButtonImage.sprite = diceSprites[currentDie];

        if (currentDie == dice.Count) {
            nextButtonImage.sprite = diceSprites[0];
        } else {
            nextButtonImage.sprite = diceSprites[currentDie + 1];
        }
    }
    
    public void OnPreviousDieButton() {
        currentDie--;

        if (currentDie == -1) {
            currentDie = dice.Count;
        }

        if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            Destroy(instantiatedDie);
            instantiatedDie = Instantiate(dice[currentDie]);
            rb = instantiatedDie.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            
            swipeModeController.Die = instantiatedDie;
        
            if (swipeModeController.IsThrowable) {
                Vector3 v = new Vector3(instantiatedDie.transform.position.x, 
                                            instantiatedDie.transform.position.y, 1f);
                
                instantiatedDie.transform.position = v;
            } else if (swipeModeController.IsThrowed) {
                swipeModeController.IsThrowable = true;
                swipeModeController.IsThrowed = false;
            }
        }

        if (currentDie == 0) {
            previousButtonImage.sprite = diceSprites[dice.Count - 1];
        } else {
            previousButtonImage.sprite = diceSprites[currentDie - 1];
        }

        currentButtonImage.sprite = diceSprites[currentDie];

        if (currentDie == dice.Count) {
            nextButtonImage.sprite = diceSprites[0];
        } else {
            nextButtonImage.sprite = diceSprites[currentDie + 1];
        }
    }

    public void OnCurrentDieButton() {
        if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            // do nothing
        } else if (Container.instance.throwMode == ThrowMode.FALLING) {
            if (instantiatedDie != null) {
                Destroy(instantiatedDie);
            }

            if (currentDie == 6) {
                // preset
                //list = fallingModeController.DropPreset();
            } else {
                instantiatedDie = fallingModeController.DropDie(dice[currentDie]);
            }
        }
    }

    public void OnTableModeButton() {
        if (Container.instance.tableModeIsEnabled) {
            Container.instance.tableModeIsEnabled = false;
            tableButtonImage.sprite = tableSprites[0];

            tableModeController.SetActive(false);
            diceChooser.SetActive(true);
            tableButtons.SetActive(false);
            pointer.SetActive(false);
            modeButton.gameObject.SetActive(true);
            
            if(Container.instance.tableConstraint) {
                SetupFallingDices();
            } else {
                modeButton.transform.parent.gameObject.SetActive(true);

                arTogglePlaneDetection.TogglePlaneDetection();
                
                if(Container.instance.throwMode == ThrowMode.FALLING)
                    SetupFallingDices();
                else {
                    SetupSwipeToThrow();
                }
            }
        } else {
            Container.instance.tableModeIsEnabled = true;
            tableButtonImage.sprite = tableSprites[1];
            if(instantiatedDie != null)
                Destroy(instantiatedDie);

            tableModeController.SetActive(true);
            diceChooser.SetActive(false);
            tableButtons.SetActive(true);
            pointer.SetActive(true);
            modeButton.gameObject.SetActive(false);

            arTogglePlaneDetection.TogglePlaneDetection();
        }
    }

    private void CheckDieResult() {
        DieResult dieResult = null;

        if(Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            dieResult = instantiatedDie.GetComponent<DieResult>();
            textMeshPro.SetText(dieResult.result.ToString());

            StartCoroutine(DisableResultView());
        } else if(Container.instance.throwMode == ThrowMode.FALLING) {

            // prima bisogna fare i preset
        }
    }

    private IEnumerator DisableResultView() {
        yield return new WaitForSeconds(5);
        resultView.SetActive(false);
    }
}
