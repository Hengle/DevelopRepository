using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombActiveEffectArea : MonoBehaviour
{
    public List<GameObject> onMyAreaObjects = new List<GameObject>();
    bool isExplode;

    // Enemy 及び Shuriken , Bombが範囲内にいれば取得
    void OnTriggerEnter2D(Collider2D other) {

        if ((other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Bomb") || other.gameObject.CompareTag("Friendly") || other.gameObject.CompareTag("Shuriken")) && !onMyAreaObjects.Contains(other.gameObject)) {
            onMyAreaObjects.Add(other.gameObject);
        }

    }

    // Enemy 及び Shuriken , Bombが範囲外に出れば削除
    void OnTriggerExit2D(Collider2D other) {

        if (isExplode) { return; }

        if ((other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Bomb") || other.gameObject.CompareTag("Friendly") || other.gameObject.CompareTag("Shuriken")) && onMyAreaObjects.Contains(other.gameObject)) {
            onMyAreaObjects.Remove(other.gameObject);
        }

    }

    public List<GameObject> GetObjectList() {
        isExplode = true;
        return onMyAreaObjects;

    }
}
