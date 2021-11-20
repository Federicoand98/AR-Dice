using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TableModeController : MonoBehaviour {

    [SerializeField] private GameObject anchorPrefab;
    [SerializeField] private GameObject meshPrefab;
    [SerializeField] private GameObject slider;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material selectedAnchorMaterial;
    [SerializeField] private Material deletingAnchorMaterial;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;

    private List<GameObject> anchors;
    private List<GameObject> meshes;
    private List<Vector3> points;
    private MeshDrawer meshDrawer;
    private LineRenderer lineRenderer;
    private GameObject meshPrefabClone;
    private bool selected = false;
    private bool deleting = false;
    private float fixedY;
    private float tempY;
    private int selectedAnchorIndex;

    void Start() {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 0;

        meshDrawer = new MeshDrawer();
        anchors = new List<GameObject>();
        meshes = new List<GameObject>();
        points = new List<Vector3>();
    }
    
    void Update() {
        if (anchors.Count > 0) {
            lineRenderer.positionCount = anchors.Count;

            for (int i = 0; i < anchors.Count; i++) {
                lineRenderer.SetPosition(i, anchors[i].transform.position);
            }
        }

        if (Input.touchCount > 0) {
            SelectAnchor();
        }

        if (selected) {
            UpdateModifiedAnchorPosition();
        }
    }
    
    // ottimizzare liste, invece che anchors e points utilizzare una sola lista

    public void AddPoint() {
        if (Container.instance.pointerPositionIsValid && !selected) {
            Vector3 position = Container.instance.pointerPosition.position;
            Quaternion rotation = Container.instance.pointerPosition.rotation;

            if (anchors.Count == 0) {
                fixedY = position.y;
                meshPrefabClone = Instantiate(meshPrefab);
                meshes.Add(meshPrefabClone);
            }

            position.y = fixedY;

            GameObject g = Instantiate(anchorPrefab, position, rotation);
            g.name = anchors.Count.ToString();

            anchors.Add(g);
            points.Add(position);

            if (points.Count > 2) {
                Mesh mesh = meshDrawer.GetMesh(points);

                meshes[0].GetComponent<MeshFilter>().sharedMesh = mesh;
                meshes[0].GetComponent<MeshCollider>().sharedMesh = mesh;
            }
        } else if (Container.instance.pointerPositionIsValid && selected) {
            selected = false;

            Renderer renderer = anchors[selectedAnchorIndex].transform.gameObject.GetComponent<Renderer>();
            renderer.material = normalMaterial;
        }
    }

    public void RemovePoint() {
        if (selected) {
            if (deleting) {
                Destroy(anchors[selectedAnchorIndex]);
                anchors.RemoveAt(selectedAnchorIndex);
                points.RemoveAt(selectedAnchorIndex);

                Mesh mesh = meshDrawer.GetMesh(points);

                meshes[0].GetComponent<MeshFilter>().sharedMesh = mesh;
                meshes[0].GetComponent<MeshCollider>().sharedMesh = mesh;

                deleting = false;
            } else {
                deleting = true;

                Renderer renderer = anchors[selectedAnchorIndex].transform.gameObject.GetComponent<Renderer>();
                renderer.material = deletingAnchorMaterial;
            }
        }
    }

    private void RemoveWalls() {
        for (int i = 1; i <= meshes.Count - 1;) {
            Destroy(meshes[i]);
            meshes.RemoveAt(i);
        }
    }

    public void BuildWalls() {
        RemoveWalls();

        if (anchors.Count > 2) {
            lineRenderer.positionCount = anchors.Count + 1;
            lineRenderer.SetPosition(anchors.Count + 1, anchors[0].transform.position);
        } else {
            // show error popup
            return;
        }

        List<Vector3> temp = new List<Vector3>();
        Vector3 p1, p2, p3, p4;
        Mesh mesh;

        for (int i = 0; i < points.Count; i++) {
            temp.Clear();

            if (i == points.Count - 1) {
                p1 = points[i];
                p2 = points[0];
                p3 = new Vector3(p2.x, p2.y + 0.2f, p2.z);
                p4 = new Vector3(p1.x, p1.y + 0.2f, p1.z);
            } else {
                p1 = points[i];
                p2 = points[i + 1];
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
    }
    
    private void SelectAnchor() {
        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began) {
            Vector2 p = t.position;

            Ray ray = Camera.current.ScreenPointToRay(p);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.tag.Equals("anchor")) {
                    
                    for (int i = 0; i < anchors.Count; i++) {
                        if (anchors[i].transform.gameObject.name.Equals(hit.transform.gameObject.name)) {
                            selectedAnchorIndex = i;
                            break;
                        }
                    }
                    
                    Renderer renderer = anchors[selectedAnchorIndex].transform.gameObject.GetComponent<Renderer>();

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
            anchors[selectedAnchorIndex].transform.position = new Vector3(temp.x, fixedY, temp.z);
            points[selectedAnchorIndex] = new Vector3(temp.x, fixedY, temp.z);

            if (points.Count > 2) {
                Mesh mesh = meshDrawer.GetMesh(points);
                
                meshes[0].GetComponent<MeshFilter>().sharedMesh = mesh;
                meshes[0].GetComponent<MeshCollider>().sharedMesh = mesh;
            }
        }
    }
    
    /*
    public void Collimazione() {
        if(!collimazioneOn) {
            collSlider.SetActive(true);
            collimazioneOn = true;

            tempY = points[0].y;

            collSlider.GetComponent<Slider>().value = 0;
        }
        else {
            collSlider.SetActive(false);
            collimazioneOn = false;
        }
    }

    public void SliderValueChange() {
        if(collimazioneOn) {
            float newY = collSlider.GetComponent<Slider>().value * .002f;

            for (int i = 0; i < anchors.Count; i++) {
                anchors[i].transform.position = new Vector3(points[i].x, tempY + newY, points[i].z);
                points[i] = new Vector3(points[i].x, tempY + newY, points[i].z);
            }
            fixedY = tempY + newY;

            if (points.Count > 2) {
                Mesh mesh = meshDrawer.GetMesh(points);
                
                meshes[0].GetComponent<MeshFilter>().sharedMesh = mesh;
                meshes[0].GetComponent<MeshCollider>().sharedMesh = mesh;
            }
        }
    }
    */
}
