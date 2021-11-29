using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
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
    private List<GameObject> presetInstantiatedDice;
    private List<Sprite> diceSprites;
    private List<Sprite> modeSprites;
    private List<Sprite> tableSprites;
    private Sprite presetSprite;
    private GameObject instantiatedDie;
    private Rigidbody rb;
    private CanvasGroup canvasGroup;
    private TextMeshProUGUI textMeshPro;
    private Image previousButtonImage;
    private Image nextButtonImage;
    private Image currentButtonImage;
    private Image tableButtonImage;
    private Image throwButtonImage;

    private int currentDie;
    private int tempResult = 0;

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
        presetSprite = Container.instance.presetSprite;

        textMeshPro = resultView.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        canvasGroup = resultView.gameObject.GetComponent<CanvasGroup>();
        resultView.SetActive(true);
        LeanTween.alphaCanvas(canvasGroup, 0f, 0f);

        throwButtonImage = modeButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        tableButtonImage = tableButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        previousButtonImage = prevButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        nextButtonImage = nextButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        currentButtonImage = currButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        
        swipeModeController = new SwipeModeController();
        fallingModeController = new FallingModeController();
        presetInstantiatedDice = new List<GameObject>();

        if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            SetupSwipeToThrow();
        } else if (Container.instance.throwMode == ThrowMode.FALLING) {
            SetupFallingDices();
        }
    }
    
    void Update() {
        if(!Container.instance.tableModeIsEnabled && !Container.instance.tableConstraint) {
            if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
                if (!swipeModeController.IsThrowed && swipeModeController.IsThrowable) {
                    swipeModeController.UpdateDiePosition();
                    tempResult = 0;
                } else if(swipeModeController.IsThrowed && !swipeModeController.IsThrowable) {
                    if(swipeModeController.PickAndSet()) {
                        StartCoroutine(DelayedThrowable());
                    }

                    CheckDieResult();
                }

                if (swipeModeController.IsThrowable) {
                    swipeModeController.SwipeDie();
                }
            } else if(Container.instance.throwMode == ThrowMode.FALLING) {
                if(instantiatedDie != null || presetInstantiatedDice.Count > 0) {
                    CheckDieResult();
                }
            }
        } else if(Container.instance.tableConstraint) {
            CheckDieResult();
        }

        if(Container.instance.themeChanged) {
            SetDiceMaterial();
        }
    }

    private void SetupSwipeToThrow() {
        pointer.SetActive(false);
        instantiatedDie = Instantiate(dice[currentDie]);
        swipeModeController.Die = instantiatedDie;
        throwButtonImage.sprite = modeSprites[0];
    }
    
    private void SetupFallingDices() {
        if (instantiatedDie != null) {
            Destroy(instantiatedDie);
        }
        
        pointer.SetActive(true);
        throwButtonImage.sprite = modeSprites[1];
    }

    public void OnThrowModeButton() {
        if(!Container.instance.tableConstraint) {
            tempResult = 0;

            if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
                Container.instance.throwMode = ThrowMode.FALLING;

                currentButtonImage.sprite = diceSprites[currentDie];
                nextButtonImage.sprite = diceSprites[currentDie + 1];
                previousButtonImage.sprite = presetSprite;
                
                SetupFallingDices();
            } else {
                Container.instance.throwMode = ThrowMode.SWIPE_TO_THROW;

                currentButtonImage.sprite = diceSprites[currentDie];
                nextButtonImage.sprite = diceSprites[currentDie + 1];
                previousButtonImage.sprite = diceSprites[dice.Count - 1];

                SetupSwipeToThrow();
            }
        }
    } 


    public void OnSideSwitchButton(int i) {
        if(i == 0) {
            currentDie--;

            if(currentDie == -1) {
                if(Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
                    currentDie = dice.Count - 1;
                } else if(Container.instance.throwMode == ThrowMode.FALLING) {
                    currentDie = dice.Count;
                }
            }
        } else if(i == 1) {
            currentDie++;

            if(currentDie == dice.Count) {
                if(Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
                    currentDie = 0;
                }
            }

            if(currentDie == dice.Count + 1 && Container.instance.throwMode == ThrowMode.FALLING) {
                currentDie = 0;
            }
        }


        if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            Destroy(instantiatedDie);
            instantiatedDie = Instantiate(dice[currentDie]);
            
            swipeModeController.Die = instantiatedDie;
            swipeModeController.IsThrowed = false;
            swipeModeController.IsThrowable = true;
            swipeModeController.UpdateDiePosition();
        }

        if(Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            if (currentDie == 0) {
                previousButtonImage.sprite = diceSprites[dice.Count - 1];
            } else {
                previousButtonImage.sprite = diceSprites[currentDie - 1];
            }

            currentButtonImage.sprite = diceSprites[currentDie];

            if (currentDie == dice.Count - 1) {
                nextButtonImage.sprite = diceSprites[0];
            } else {
                nextButtonImage.sprite = diceSprites[currentDie + 1];
            }
        } else if (Container.instance.throwMode == ThrowMode.FALLING) {
            if (currentDie == 0) {
                previousButtonImage.sprite = presetSprite;
            } else {
                previousButtonImage.sprite = diceSprites[currentDie - 1];
            }

            if(currentDie == 6) {
                currentButtonImage.sprite = presetSprite;
                nextButtonImage.sprite = diceSprites[0];
                previousButtonImage.sprite = diceSprites[currentDie - 2];
            } else {
                currentButtonImage.sprite = diceSprites[currentDie];
            }

            if (currentDie == dice.Count - 1) {
                nextButtonImage.sprite = presetSprite;
            } else {
                nextButtonImage.sprite = diceSprites[currentDie + 1];
            }
        }
    }

    public void OnCurrentDieButton() {
        if (Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            swipeModeController.IsThrowed = false;
            swipeModeController.IsThrowable = true;
            swipeModeController.UpdateDiePosition();
        } else if (Container.instance.throwMode == ThrowMode.FALLING) {
            tempResult = 0;

            if (instantiatedDie != null) {
                Destroy(instantiatedDie);
            }

            if (currentDie == dice.Count) {  // preset
                if(presetInstantiatedDice.Count > 0) {
                    for(int i = presetInstantiatedDice.Count - 1; i >= 0; i--) {
                        Destroy(presetInstantiatedDice[i]);
                        presetInstantiatedDice.RemoveAt(i);
                    }
                }

                presetInstantiatedDice = fallingModeController.DropPreset(Container.instance.activePreset);
            } else {
                instantiatedDie = fallingModeController.DropDie(dice[currentDie]);
            }
        }
    }

    public void OnTableModeButton() {
        if (Container.instance.tableModeIsEnabled) {
            tableModeController.SetActive(false);
            diceChooser.SetActive(true);
            tableButtons.SetActive(false);
            pointer.SetActive(false);
            modeButton.gameObject.SetActive(true);
            tableButtonImage.sprite = tableSprites[0];

            if(Container.instance.tableConstraint) {
                Container.instance.throwMode = ThrowMode.FALLING;
                SetupFallingDices();
            } else {
                // popup
                // prima devi costruire i muri oppure distruggere tutto

                for(int i = Container.instance.tableMeshes.Count - 1; i >= 0; i--) {
                    Destroy(Container.instance.tableMeshes[i]);
                    Container.instance.tableMeshes.RemoveAt(i);
                }

                arTogglePlaneDetection.EnablePlaneDetection(true);

                if (Container.instance.throwMode == ThrowMode.FALLING)
                    SetupFallingDices();
                else {
                    SetupSwipeToThrow();
                }
            }

            Container.instance.tableModeIsEnabled = false;
        } else {
            Container.instance.tableModeIsEnabled = true;
            tableButtonImage.sprite = tableSprites[1];

            if (instantiatedDie != null)
                Destroy(instantiatedDie);

            tableModeController.SetActive(true);
            diceChooser.SetActive(false);
            tableButtons.SetActive(true);
            pointer.SetActive(true);
            modeButton.gameObject.SetActive(false);

            arTogglePlaneDetection.EnablePlaneDetection(false);
        }
    }

    private void CheckDieResult() {
        if(Container.instance.throwMode == ThrowMode.SWIPE_TO_THROW) {
            DieResult dieResult = instantiatedDie.GetComponent<DieResult>();

            if(dieResult.availableResult) {
                SetDieResult(dieResult);
            }
        } else if(Container.instance.throwMode == ThrowMode.FALLING) {

            if(currentDie == dice.Count) {
                List<DieResult> results = presetInstantiatedDice.Select(d => d.GetComponent<DieResult>()).ToList();

                SetDiceResults(results);
            } else {
                DieResult dieResult = instantiatedDie.GetComponent<DieResult>();

                if(dieResult.availableResult) {
                    SetDieResult(dieResult);
                }
            }
        }
    }

    private void SetDieResult(DieResult dieResult) {
        if(dieResult.result != tempResult) {
            resultView.SetActive(true);
            LeanTween.alphaCanvas(canvasGroup, 1f, 0f);
            textMeshPro.SetText(dieResult.result.ToString());

            StartCoroutine(ShowResultView());
        }

        tempResult = dieResult.result;
    }

    private void SetDiceResults(List<DieResult> results) {
        int totalResult = 0;
        bool available = false;

        foreach(DieResult result in results) {
            totalResult += result.result;

            if(result.availableResult) {
                available = true;
            }
        }

        if(totalResult != tempResult && available) {
            resultView.SetActive(true);
            LeanTween.alphaCanvas(canvasGroup, 1f, 0f);
            textMeshPro.SetText(totalResult.ToString());

            StartCoroutine(ShowResultView());
        }

        tempResult = totalResult;
    }

    private IEnumerator ShowResultView() {
        yield return new WaitForSeconds(3);
        LeanTween.alphaCanvas(canvasGroup, 0f, 1f).setOnComplete(DisableResult);
    }

    private void DisableResult() {
        resultView.SetActive(false);
    }

    private void SetDiceMaterial() {
        Theme currTheme = Container.instance.activeTheme;
        
        Container.instance.themeDie.color = currTheme.GetDieColor();
        Container.instance.themeNumber.color = currTheme.GetNumbColor();
        
        Container.instance.themeChanged = false;
    }

    private IEnumerator DelayedThrowable() {
        yield return new WaitForSeconds(0.2f);
        swipeModeController.IsThrowable = true;
    }
}
