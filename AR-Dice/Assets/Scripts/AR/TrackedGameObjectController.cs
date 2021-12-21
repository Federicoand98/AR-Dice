using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class TrackedGameObjectController : MonoBehaviour {

    [SerializeField] private Camera arCamera;
    [SerializeField] private Color activeColor = Color.red;
    [SerializeField] private Color inactiveColor = Color.white;
    [SerializeField] private GameObject sliders;
    private Button xButton;
    [SerializeField] private float scaleXZ = 1; // change with different prefabs
    [SerializeField] private float scaleY = 1; // change with different prefabs

    private List<GameObject> fixedObjectList;
    private int selectedIndex = -1;

    private GameObject trovato = null;
    private GameObject lasttrovato = null;
    private Slider hSlider;
    private Slider vSlider;
    private Button canButton;

    private float scaleFactorXZ = 0;
    private float scaleFactorY = 0;
    private bool valueChanged = false;

    private GameObject selectedPref;
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    void Start() {
        fixedObjectList = new List<GameObject>();
        vSlider = sliders.transform.GetChild(0).gameObject.GetComponent<Slider>();
        hSlider = sliders.transform.GetChild(1).gameObject.GetComponent<Slider>();
        xButton = GameObject.Find("XButton").GetComponent<Button>();
        xButton.onClick.AddListener(OnButtonPressed);
        sliders.SetActive(false);
        xButton.gameObject.SetActive(false);
    }

    void Update() {
        SpawnFixedClone();
    }

    private void SpawnFixedClone()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                Vector2 p = t.position;

                Ray ray = arCamera.ScreenPointToRay(t.position);
                RaycastHit hitObject;

                if (Physics.Raycast(ray, out hitObject))
                {
                    trovato = null;

                    Transform selected = hitObject.transform;
                    Debug.Log(selected.name);

                    foreach (GameObject go in fixedObjectList)
                    {
                        if (go.Equals(hitObject.transform.gameObject))
                        {
                            trovato = go;
                        }
                    }

                    if (trovato != null)
                    {
                        MeshRenderer meshRenderer = trovato.GetComponent<MeshRenderer>();
                        MeshRenderer meshRenderer2 = null;
                        if (lasttrovato != null) 
                            meshRenderer2 = lasttrovato.GetComponent<MeshRenderer>();

                        sliders.SetActive(true);
                        xButton.gameObject.SetActive(true);

                        ResetSlider(trovato.transform.localScale);
                        if (lasttrovato == trovato)
                        {
                            if (meshRenderer.material.color == inactiveColor)
                            {
                                meshRenderer.material.color = activeColor;
                                sliders.SetActive(true);
                                xButton.gameObject.SetActive(true);
                            }
                            else
                            {
                                meshRenderer.material.color = inactiveColor;
                                sliders.SetActive(false);
                                xButton.gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            meshRenderer.material.color = activeColor;
                            sliders.SetActive(true);
                            xButton.gameObject.SetActive(true);
                            if (lasttrovato != null)
                                meshRenderer2.material.color = inactiveColor;
                        }

                        lasttrovato = trovato;
                    }
                    else
                    {
                        if (lasttrovato != null)
                        {
                            MeshRenderer meshRenderer2 = lasttrovato.GetComponent<MeshRenderer>();
                            meshRenderer2.material.color = inactiveColor;
                            lasttrovato = null;
                        }


                        GameObject temp = null;

                        foreach (GameObject g in Container.instance.trackedObjects)
                        {
                            Debug.Log("current g name in foreach: " + selected.name);
                            if (g.transform.name.Equals(selected.name))
                            {
                                temp = g;
                                Debug.Log("ROMPO");
                                break;
                            }
                        }

                        Debug.Log("temp nome: " + temp);
                        GameObject goClone = Instantiate(temp, selected.position, selected.rotation);
                        if (goClone.name.Equals("ARBottle"))
                            goClone.transform.localEulerAngles = new Vector3(goClone.transform.rotation.x, goClone.transform.rotation.y, goClone.transform.rotation.z);
                        fixedObjectList.Add(goClone);

                        sliders.SetActive(false);
                        xButton.gameObject.SetActive(false);
                    }
                } 
            }
        }
    }

    public void OnHorizontalSliderValueChanged() {
        if(hSlider.value < 0) {
            scaleFactorXZ = 1 + hSlider.value * 0.01f;
        } else {
            scaleFactorXZ = 1 + hSlider.value * 0.2f;
        }

        trovato.transform.localScale = new Vector3(scaleFactorXZ * scaleXZ, trovato.transform.localScale.y, scaleFactorXZ * scaleXZ);
    }

    public void OnVerticalSliderValueChanged() {
        if(vSlider.value < 0) {
            scaleFactorY = 1 + vSlider.value * 0.01f;
        } else {
            scaleFactorY = 1 + vSlider.value * 0.2f;
        }

        trovato.transform.localScale = new Vector3(trovato.transform.localScale.x, scaleFactorY * scaleY, trovato.transform.localScale.z);
    }

    public void OnButtonPressed()
    {
        trovato.transform.localPosition = new Vector3(trovato.transform.localScale.x, 100.0f, trovato.transform.localScale.z);
        MeshRenderer meshRenderer = trovato.GetComponent<MeshRenderer>();
        meshRenderer.material.color = inactiveColor;
        trovato = null;
        sliders.SetActive(false);
        xButton.gameObject.SetActive(false);
    }

    private void ResetSlider(Vector3 scale) {
        if(scale.x > scaleXZ) {
            hSlider.value = (scale.x - 1) / 0.02f;
        } else if (scale.x < scaleXZ) {
            hSlider.value = (scale.x - 1) / 0.01f;
        }

        if(scale.y > scaleY) {
            vSlider.value = (scale.y - 1) / 0.2f;
        } else if (scale.y < scaleY) {
            vSlider.value = (scale.y - 1) / 0.01f;
        }
    }
   
}

