using UnityEngine;
using System.Collections;

public class DisableParticle : MonoBehaviour {

    void OnEnable() {
        StartCoroutine(ParticleWorking());
    }


    IEnumerator ParticleWorking() {
        var particle = GetComponent<ParticleSystem>();

        yield return new WaitWhile(() => particle.IsAlive(true));
        gameObject.SetActive(false);
    }
}