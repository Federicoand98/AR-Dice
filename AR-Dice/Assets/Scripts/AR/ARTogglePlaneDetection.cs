using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class ARTogglePlaneDetection : MonoBehaviour {

    private ARPlaneManager planeManager;

    private void Awake() {
        planeManager = GetComponent<ARPlaneManager>();
    }

    public void TogglePlaneDetection() {
        planeManager.enabled = !planeManager.enabled;

        if(planeManager.enabled) {
            SetAllPlanesActive(true);
        } else {
            SetAllPlanesActive(false);
        }
    }

    private void SetAllPlanesActive(bool enabled) {
        foreach (var plane in planeManager.trackables) {
            plane.gameObject.SetActive(enabled);
        }
    }
}
