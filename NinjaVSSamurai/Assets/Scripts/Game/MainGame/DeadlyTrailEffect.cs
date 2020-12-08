using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeadlyTrailEffect : MonoBehaviour {
	[SerializeField] Type type;
	[SerializeField] float delayTime;
	public enum Type { Right, Left }

	Vector3[] path1 = { new Vector3(5.2f, 2.67f, 0), new Vector3(3.13f, -0.94f, 0), new Vector3(-5.62f, -2.88f, 0) };
	Vector3[] path2 = { new Vector3(-2.78f, 1.61f, 0), new Vector3(0.18f, -1.35f, 0), new Vector3(3.67f, -1.41f, 0) };


	void Start () {
		AudioManager.Instance.LoadSe(AudioManager.SeName.Slash);
	}

	public void Play () {
		switch (type) {
			case Type.Right:
				transform.DOMove(new Vector3(-12.89f, -4.84f, 0), 0.025f).SetDelay(delayTime).OnStart(() => {
					AudioManager.Instance.PlaySe(AudioManager.SeName.Slash);
				});
				break;
			case Type.Left:
				transform.DOMove(new Vector3(9.63f, -3.3f, 0), 0.025f).SetDelay(delayTime).OnStart(() => {
					AudioManager.Instance.PlaySe(AudioManager.SeName.Slash);
				});
				break;
		}
	}
}
