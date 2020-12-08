using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour {

    [SerializeField] ExplosiveBarrelEffectiveArea myArea;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            var targets = myArea.GetEffectiveObject();
            foreach(GameObject obj in targets) {
                obj.GetComponent<Enemy>().KillEnemy(gameObject.transform);
            }

            gameObject.SetActive(false);
        }
    }

}
