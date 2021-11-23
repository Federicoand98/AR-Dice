using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeModeController {

    private GameObject _die;
    private Rigidbody rigidbody;

    private Vector3 startPosition = Vector3.zero;
    private Vector3 endPosition = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    private Vector3 torque = Vector3.zero;
    
    private float throwForceY = 0.2f;
    private float throwForceZ = 10f;
    float touchStartTime = 0f;
    float touchEndTime = 0f;
    float timeInterval = 0f;
    
    private bool _isThrowed;
    private bool _throwable;
    bool onTouchHold = false;

    public SwipeModeController() {
        this._isThrowed = false;
        this._throwable = true;
    }

    public void SwipeDie() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            
            if(touch.phase == TouchPhase.Began) {
                startPosition = touch.position;
                touchStartTime = Time.time;

                Ray ray = Camera.current.ScreenPointToRay(startPosition);
                RaycastHit hitObject;

                if (Physics.Raycast(ray, out hitObject)) {
                    onTouchHold = true;
                    rigidbody.isKinematic = true;
                }
                
            }

            if (onTouchHold && touch.phase == TouchPhase.Ended) {
                onTouchHold = false;
                // Force Detected
                touchEndTime = Time.time;
                timeInterval = touchEndTime - touchStartTime;
                endPosition = touch.position;
                direction = endPosition - startPosition;
                
                // Random Torque
                torque.x = Random.Range(-200, 200);
                torque.y = Random.Range(-200, 200);
                torque.z = Random.Range(-200, 200);
                
                // Camera Rotation and Reset
                float xCameraRotation = Camera.current.transform.forward.x;
                float zCameraRotation = Camera.current.transform.forward.z;
                float xForce = (throwForceZ / timeInterval) * xCameraRotation;
                float zForce = (throwForceZ / timeInterval) * zCameraRotation;
                float yForce = throwForceY * direction.y;

                rigidbody.isKinematic = false;
                rigidbody.AddForce(xForce, yForce, zForce);
                rigidbody.AddTorque(torque);

                _isThrowed = true;
                _throwable = false;
            }
        }
    }

    public void PickAndSet() {
        if(Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began) {
                Vector2 pos = touch.position;

                Ray ray = Camera.current.ScreenPointToRay(pos);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit)) {
                    if(hit.transform.tag.Equals("dice")) {
                        Debug.Log("\n------------\n" + hit.transform.tag + "\n------------\n");

                        UpdateDiePosition();

                        _isThrowed = false;
                        _throwable = true;
                    }
                }
            }
        }
    }

    public void UpdateDiePosition() {
        _die.transform.position = Camera.current.ViewportToWorldPoint(new Vector3(0.5f, 0.2f, 0.5f));
        _die.transform.rotation = Camera.current.transform.rotation;
    }

    public GameObject Die {
        get => _die;
        set {
            _die = value;
            rigidbody = _die.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;

            Vector3 v = new Vector3(_die.transform.position.x, _die.transform.position.y, 1f);
            _die.transform.position = v;
        }
    }

    public bool IsThrowed {
        get => _isThrowed;
        set {
            _isThrowed = value;
        }
    }

    public bool IsThrowable {
        get => _throwable;
        set {
            _throwable = value;
        }
    }
}
