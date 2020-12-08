using UnityEngine;
using System.Collections;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T> {
	protected static T instance;
	protected bool isCreated = false;

	public static T Instance {
		get {
			if (instance == null) {
				instance = (T)FindObjectOfType(typeof(T));
				
				if (instance == null) {
					Debug.LogWarning(typeof(T) + "is nothing");
				}
			}
			return instance;
		}
	}

	protected virtual void Awake () {
		CheckInstance();
	}

	protected bool CheckInstance () {
		if (instance == null) {
			instance = (T)this;
			return true;
		} else if(Instance == this) {
			return true;
		}
		Destroy(this.gameObject);
		isCreated = true;
		return false;
	}
}