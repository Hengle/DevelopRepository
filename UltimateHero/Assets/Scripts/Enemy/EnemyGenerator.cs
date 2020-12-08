using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] MultipleParticles zone;
    [SerializeField] float startTime = 0.0f;
    [SerializeField] float endTime = 10.0f;
    [SerializeField] float interval = 3.0f;
    [SerializeField] bool isBoss;

    public event Action<Enemy> OnGeneratEnemy;
    public event Action OnEndEnemyGenerator;
    bool isActive = true;
    float time = 0;

    IEnumerator StartUp()
    {
        zone.Stop(ParticleSystemStopBehavior.StopEmitting);

        yield return new WaitForSeconds(startTime);

        //zone.Play();

        if (isBoss)
        {
            var enemyScript = Instantiate(enemy, transform.position, transform.rotation).GetComponent<Enemy>();
            enemyScript.Appearance();
            OnGeneratEnemy?.Invoke(enemyScript);
            yield return new WaitForSeconds(0.5f);
            Stop();
        }
        else
        {
            Invoke("Stop", endTime - startTime);
            while (isActive)
            {
                var enemyScript = Instantiate(enemy, transform.position.SetY(5.0f), transform.rotation).GetComponent<Enemy>();
                enemyScript.Appearance();
                OnGeneratEnemy?.Invoke(enemyScript);
                yield return new WaitForSeconds(interval);
            }
        }
    }

    public void Activate()
    {
        StartCoroutine(StartUp());
    }

    public void Stop()
    {
        zone.Stop(ParticleSystemStopBehavior.StopEmittingAndClear);
        isActive = false;
        OnEndEnemyGenerator?.Invoke();
    }

    public bool GetActive()
    {
        return isActive;
    }
}
