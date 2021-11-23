using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingModeController : MonoBehaviour {

    public FallingModeController() {
        
    }

    public GameObject DropDie(GameObject currentDie) {
        GameObject res = null;

        Vector3 v = new Vector3(Container.instance.pointerPosition.position.x, 1f, Container.instance.pointerPosition.position.z);
        Vector3 torque = new Vector3();

        torque.x = Random.Range(-200, 200);
        torque.y = Random.Range(-200, 200);
        torque.z = Random.Range(-200, 200);

        res = Instantiate(currentDie, v, Container.instance.pointerPosition.rotation);
        res.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

        Rigidbody rb = res.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddTorque(torque);

        return res;
    }

    public List<GameObject> DropPreset() {
        List<GameObject> res = new List<GameObject>();

        

        return res;
    }
}
