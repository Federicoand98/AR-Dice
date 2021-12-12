using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingModeController : MonoBehaviour {

    public FallingModeController() {
        
    }

    public GameObject DropDie(GameObject currentDie) {
        GameObject res = null;
        float y = Container.instance.pointerPosition.position.y + 1.5f;

        Vector3 v = new Vector3(Container.instance.pointerPosition.position.x, y, Container.instance.pointerPosition.position.z);
        Vector3 torque = new Vector3();

        torque.x = Random.Range(-200, 200);
        torque.y = Random.Range(-200, 200);
        torque.z = Random.Range(-200, 200);

        res = Instantiate(currentDie, v, Container.instance.pointerPosition.rotation);
        res.transform.rotation = Quaternion.Euler(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));

        Rigidbody rb = res.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddTorque(torque);

        return res;
    }

    public List<GameObject> DropPreset(Preset preset) {
        List<GameObject> res = new List<GameObject>();
        float y = Container.instance.pointerPosition.position.y + 1.5f;

        for(int i = 0; i < Container.instance.dice.Count; i++) {
            for(int j = 0; j < preset.GetIndex(i); j++) {
                float dRange = 0.002f * (1 + j/5);
                float hRange = 0.08f * (1 + j/5);
                Vector3 v = new Vector3(Container.instance.pointerPosition.position.x + Random.Range(-dRange, dRange), y + Random.Range(-hRange, hRange), 
                                Container.instance.pointerPosition.position.z + Random.Range(-dRange, dRange));

                Vector3 torque = new Vector3();
                torque.x = Random.Range(-200, 200);
                torque.y = Random.Range(-200, 200);
                torque.z = Random.Range(-200, 200);

                GameObject g = Instantiate(Container.instance.dice[i], v, Container.instance.pointerPosition.rotation);
                Rigidbody body = g.GetComponent<Rigidbody>();
                body.AddTorque(torque);

                res.Add(g);
            }
        }

        return res;
    }
}
