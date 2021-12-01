using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARImageTracking : MonoBehaviour
{
    private GameObject _pottery;
    private List<GameObject> pots;
    private List<GameObject> meshes;
    private List<Vector3> points;
    private MeshDrawer meshDrawer;
    private LineRenderer lineRenderer;
    private GameObject meshPrefabClone;
    private bool selected = false;
    private bool deleting = false;
    private bool wallBuilded = false;
    private float fixedY;
    private float tempY;
    private int selectedAnchorIndex;

    private int spamlock = 0;

    [SerializeField] private GameObject[] placedPrefabs;

    private List<GameObject> settled;

    private Dictionary<string, GameObject> prefabsList = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    private List<GameObject> fixedPrefabs = new List<GameObject>();
    private ARTrackedImageManager trackedImageManager;
    private GameObject prefabContainer;


    //new
    private Button place;

    //[SerializeField] private PlacementObject[] placedObjects;

    [SerializeField]
    private Color activeColor = Color.red;

    [SerializeField]
    private Color inactiveColor = Color.gray;

    [SerializeField]
    private Camera arCamera;

    private Vector2 touchPosition = default;

    private ARRaycastManager arRaycastManager;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private GameObject trovato = null;
    private GameObject lasttrovato = null;
    private int i = 0;

    private void Update()
    {

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                Vector2 p = t.position;
                
                Debug.Log($"TOCCO RILEVATO");
                Ray ray = arCamera.ScreenPointToRay(t.position);
                RaycastHit hitObject;
                if (Physics.Raycast(ray, out hitObject))
                {
                    Debug.Log($"OGGETTO COLPITO");
                    trovato = null;

                    Transform selected = hitObject.transform;

                    Debug.Log($"RAY NAME: {hitObject.transform.name} --- RAY POSITION");


                    foreach (GameObject go in fixedPrefabs)
                    {
                        if (go.Equals(hitObject.transform.gameObject))
                            trovato = go;
                        Debug.Log($"TROVATO");
                    }
                    

                    if (trovato != null)
                    {
                        Debug.Log($"DENTRO TROVATO");
                        //PlacementObject placementObject = hitObject.transform.GetComponent<PlacementObject>();
                        MeshRenderer meshRenderer = trovato.GetComponent<MeshRenderer>();
                        meshRenderer.material.color = activeColor;
                        if (lasttrovato != null)
                        {
                            MeshRenderer meshRenderer2 = lasttrovato.GetComponent<MeshRenderer>();
                            meshRenderer2.material.color = inactiveColor;
                        }
                        lasttrovato = trovato;
                    }
                    else
                    {
                        GameObject goClone = Instantiate(spawnedPrefabs[selected.name], selected.position, selected.rotation);
                        fixedPrefabs.Add(goClone);
                        /*
                        goClone.transform.parent = prefabContainer.transform;
                        goClone.name = "set" + (i + 1);*/
                        Debug.Log($"CLONE: {goClone.name}");
                    }

                        //GameObject prova = hitObject.transform.gameObject;
                        //Instantiate(prova, prova.)
                        Debug.Log($"PROVA GAMEOBJECT: {selected.name}");
                    }
                
            }
        }

    }

    private void Awake()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        arRaycastManager = GetComponent<ARRaycastManager>();

        foreach (GameObject prefab in placedPrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.SetActive(false);
            newPrefab.name = prefab.name;
            spawnedPrefabs.Add(prefab.name, newPrefab);
            prefabsList.Add(prefab.name, newPrefab);
        }
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
            spawnedPrefabs.Add(trackedImage.name, prefabsList[trackedImage.name]);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            spawnedPrefabs.Remove(trackedImage.name);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {

        string name = trackedImage.referenceImage.name;
        Vector3 position = trackedImage.transform.position;
        Quaternion rotation = trackedImage.transform.rotation;

        GameObject prefab = spawnedPrefabs[name];
        prefab.SetActive(true);
        prefab.transform.position = position;
        prefab.transform.rotation = rotation;

        foreach (GameObject go in spawnedPrefabs.Values)
        {
            if (go.name != name)
            {
                //go.SetActive(false);
            }
        }
    }
}
