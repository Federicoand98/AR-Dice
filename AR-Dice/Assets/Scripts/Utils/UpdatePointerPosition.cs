using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class UpdatePointerPosition : MonoBehaviour {

    private ARSessionOrigin origin;
    private ARRaycastManager raycastManager;
    
    void Start() {
        origin = FindObjectOfType<ARSessionOrigin>();
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }
    
    void Update() {
        UpdatePointerPose();
        UpdatePointer();
    }

    private void UpdatePointerPose() {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        Container.instance.pointerPositionIsValid = hits.Count > 0;

        if (Container.instance.pointerPositionIsValid) {
            Container.instance.pointerPosition = hits[0].pose;

            Vector3 cameraForward = Camera.current.transform.forward;
            Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

            Container.instance.pointerPosition.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void UpdatePointer() {
        if (Container.instance.pointerPositionIsValid) {
            gameObject.SetActive(true);
            gameObject.transform.SetPositionAndRotation(Container.instance.pointerPosition.position, Container.instance.pointerPosition.rotation);
            
            Debug.Log("\n--------------\n" + gameObject.transform.position + "\n-------------------\n");
        } else {
            gameObject.SetActive(false);
        }
    }
}
