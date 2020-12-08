using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualEffects;
using System;

public class BossImageDissolve : MonoBehaviour {

    [SerializeField] Dissolve dissolve;
    [SerializeField] float interval = 0f;
    [SerializeField] float duration = 1.0f;

    public void Active(Action callback) {
        dissolve.Play(true, duration, interval, callback);
    }

}
