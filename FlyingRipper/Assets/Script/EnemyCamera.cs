using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCamera : MonoBehaviour {

    [SerializeField] GameObject[] targets;

	void Update() {
        Vector3 center = (targets[0].transform.position + targets[1].transform.position) * 0.5f;
        gameObject.transform.LookAt(center);
	}

}
