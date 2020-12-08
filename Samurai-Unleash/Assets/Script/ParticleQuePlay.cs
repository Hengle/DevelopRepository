using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleQuePlay : MonoBehaviour
{
    [SerializeField] GameObject[] playList;
    [SerializeField] float span;

    public void Play() {
        StartCoroutine(PlayParticle());
    }

    IEnumerator PlayParticle() {
        for (int i = 0; i < playList.Length; i++) {
            playList[i].GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(span);
        }
    }

}
