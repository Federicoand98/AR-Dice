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

    public void EnablePlaneDetection(bool enabled) {
        if(enabled) {
            planeManager.enabled = true;
        } else {
            planeManager.enabled = false;
        }

        SetAllPlanesActive(enabled);
    }

    private void SetAllPlanesActive(bool enabled) {
        foreach (var plane in planeManager.trackables) {
            plane.gameObject.SetActive(enabled);
        }
    }
}
