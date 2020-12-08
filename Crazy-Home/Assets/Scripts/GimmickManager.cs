using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GimmickManager : MonoBehaviour
{
    List<Gimmick> gimmicks = new List<Gimmick>();

    private void Awake()
    {
        var gObjects = GameObject.FindGameObjectsWithTag("Gimmick");
        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        var enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();

        foreach (GameObject gObject in gObjects)
        {
            var gScript = gObject.GetComponent<Gimmick>();
            //gScript.OnGimmickTrigger += () =>
            //{
            //    enemy.Stop();
            //};

            //gScript.OnGimmickEnd += () => 
            //{
            //    player.Activate();
            //    enemy.Play();
            //};

            gimmicks.Add(gScript);
        }
    }
}