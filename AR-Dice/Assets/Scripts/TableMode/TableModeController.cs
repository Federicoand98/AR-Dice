using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class TableModeController : MonoBehaviour {

    [SerializeField] private GameObject anchorPrefab;
    [SerializeField] private GameObject meshPrefab;
    [SerializeField] private GameObject slider;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material selectedAnchorMaterial;
    [SerializeField] private Material deletingAnchorMaterial;
    [SerializeField] private GameObject popupGameObject;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;

    private List<GameObject> vertices;
    private List<GameObject> meshes;
    private MeshDrawer meshDrawer;
    private LineRenderer lineRenderer;
    private GameObject meshPrefabClone;
    private bool selected = false;
    private bool deleting = false;
    private bool wallBuilded = false;
    private float fixedY;
    private float tempY;
    private int selectedAnchorIndex;
    private Popup popup;

    void Start() {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 0;

        meshDrawer = new MeshDrawer();
        meshes = new List<GameObject>();
        vertices = new List<GameObject>();
        
        slider.SetActive(false);

        popup = popupGameObject.GetComponent<Popup>();
    }
    
    void Update() {
        if (vertices.Count > 0) {
            lineRenderer.positionCount = vertices.Count;

            for (int i = 0; i < vertices.Count; i++) {
                lineRenderer.SetPosition(i, vertices[i].transform.position);
            }
        }

        if (Input.touchCount > 0) {
            SelectAnchor();
        }

        if (selected) {
            UpdateModifiedAnchorPosition();
        }
    }

    public void AddPoint() {
        if (Container.instance.pointerPositionIsValid && !selected) {
            Vector3 position = Container.instance.pointerPosition.position;
            Quaternion rotation = Container.instance.pointerPosition.rotation;

            if (vertices.Count == 0) {
                fixedY = position.y;
                meshPrefabClone = Instantiate(meshPrefab);
                meshes.Add(meshPrefabClone);
            }

            position.y = fixedY;

            GameObject g = Instantiate(anchorPrefab, position, rotation);
            g.transform.GetChild(0).name = vertices.Count.ToString();

            vertices.Add(g);

            if (vertices.Count > 2) {
                List<Vector3> points = vertices.Select(v => v.transform.position).ToList();
                Mesh mesh = meshDrawer.GetMesh(points);

                meshes[0].GetComponent<MeshFilter>().sharedMesh = mesh;
                meshes[0].GetComponent<MeshCollider>().sharedMesh = mesh;
            }

            Container.instance.tableMeshes = meshes;
        } else if (Container.instance.pointerPositionIsValid && selected) {
            selected = false;

            Renderer renderer = vertices[selectedAnchorIndex].transform.gameObject.GetComponent<Renderer>();
            renderer.material = normalMaterial;
        }
    }

    public void RemovePoint() {
        if (selected) {
            if (deleting) {
                Destroy(vertices[selectedAnchorIndex]);
                vertices.RemoveAt(selectedAnchorIndex);

                List<Vector3> points = vertices.Select(v => v.transform.position).ToList();
                Mesh mesh = meshDrawer.GetMesh(points);

                meshes[0].GetComponent<MeshFilter>().sharedMesh = mesh;
                meshes[0].GetComponent<MeshCollider>().sharedMesh = mesh;

                deleting = false;

                Container.instance.tableMeshes = meshes;
            } else {
                deleting = true;

                //Renderer renderer = vertices[selectedAnchorIndex].transform.gameObject.GetComponent<Renderer>();
                //renderer.material = deletingAnchorMaterial;

                popup.ShowPopup(Container.instance.errorDictionary["del_anchor"]);
            }
        }
    }

    private void RemoveWalls() {
        for (int i = 1; i <= meshes.Count - 1;) {
            Destroy(meshes[i]);
            meshes.RemoveAt(i);
        }

        Container.instance.tableMeshes = meshes;
        wallBuilded = false;
    }

    public void BuildWalls() {
        RemoveWalls();

        if (vertices.Count > 2) {
            lineRenderer.positionCount = vertices.Count + 1;
            lineRenderer.SetPosition(vertices.Count + 1, vertices[0].transform.position);
        } else {
            // show error popup
            return;
        }

        List<Vector3> temp = new List<Vector3>();
        Vector3 p1, p2, p3, p4;
        Mesh mesh;

        for (int i = 0; i < vertices.Count; i++) {
            temp.Clear();

            if (i == vertices.Count - 1) {
                p1 = vertices[i].transform.position;
                p2 = vertices[0].transform.position;
                p3 = new Vector3(p2.x, p2.y + 0.2f, p2.z);
                p4 = new Vector3(p1.x, p1.y + 0.2f, p1.z);
            } else {
                p1 = vertices[i].transform.position;
                p2 = vertices[i + 1].transform.position;
                p3 = new Vector3(p2.x, p2.y + 0.2f, p2.z);
                p4 = new Vector3(p1.x, p1.y + 0.2f, p1.z);
            }
            
            temp.Add(p1);
            temp.Add(p2);
            temp.Add(p3);
            temp.Add(p4);

            mesh = meshDrawer.GetMesh(temp, false);

            meshPrefabClone = Instantiate(meshPrefab);
            meshPrefabClone.GetComponent<MeshFilter>().sharedMesh = mesh;
            meshPrefabClone.GetComponent<MeshCollider>().sharedMesh = mesh;
            
            meshes.Add(meshPrefabClone);
        }

        tempY = vertices[0].transform.position.y;
        
        wallBuilded = true;
        Container.instance.tableConstraint = true;
        Container.instance.tableMeshes = meshes;
        slider.SetActive(true);
    }
    
    private void SelectAnchor() {
        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began) {
            Vector2 p = t.position;

            Ray ray = Camera.current.ScreenPointToRay(p);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.tag.Equals("anchor")) {

                    for (int i = 0; i < vertices.Count; i++) {
                        if (vertices[i].transform.GetChild(0).name.Equals(hit.transform.gameObject.name)) {
                            selectedAnchorIndex = i;
                            break;
                        }
                    }
                    
                    Renderer renderer = vertices[selectedAnchorIndex].transform.gameObject.GetComponent<Renderer>();

                    if (selected) {
                        selected = false;
                        renderer.material = normalMaterial;
                    } else {
                        selected = true;
                        renderer.material = selectedAnchorMaterial;
                    }
                }
            }
        }
    }
    
    private void UpdateModifiedAnchorPosition() {
        if (Container.instance.pointerPositionIsValid) {
            Vector3 temp = Container.instance.pointerPosition.position;
            vertices[selectedAnchorIndex].transform.position = new Vector3(temp.x, fixedY, temp.z);

            if (vertices.Count > 2) {
                List<Vector3> points = vertices.Select(p => p.transform.position).ToList();
                Mesh mesh = meshDrawer.GetMesh(points);
                
                meshes[0].GetComponent<MeshFilter>().sharedMesh = mesh;
                meshes[0].GetComponent<MeshCollider>().sharedMesh = mesh;
            }

            Container.instance.tableMeshes = meshes;
        }
    }

    public void OnTrashButton() {
        for(int i = meshes.Count - 1; i >= 0; i--) {
            Destroy(meshes[i]);
            meshes.RemoveAt(i);
        }

        for(int i = vertices.Count - 1; i >= 0; i--) {
            Destroy(vertices[i]);
            vertices.RemoveAt(i);
        }

        vertices.Clear();
        meshes.Clear();
        lineRenderer.positionCount = 0;

        Container.instance.tableConstraint = false;
        Container.instance.tableMeshes = meshes;
    }

    public void SliderValueChange() {
        if(wallBuilded) {
            float newY = slider.GetComponent<Slider>().value * .002f;

            for (int i = 0; i < vertices.Count; i++) {
                vertices[i].transform.position = new Vector3(vertices[i].transform.position.x, tempY + newY, vertices[i].transform.position.z);
            }

            fixedY = tempY + newY;

            if (vertices.Count > 2) {
                List<Vector3> points = vertices.Select(p => p.transform.position).ToList();
                Mesh mesh = meshDrawer.GetMesh(points);
                
                meshes[0].GetComponent<MeshFilter>().sharedMesh = mesh;
                meshes[0].GetComponent<MeshCollider>().sharedMesh = mesh;
            }

            Container.instance.tableMeshes = meshes;
        }
    }
}
