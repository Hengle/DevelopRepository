using UnityEngine;

namespace Data.Master {
	public abstract class MasterBase : ScriptableObject {
		protected abstract void CheckValidity ();

		protected virtual void Awake () {
			if (GameController.IsDebugMode) {
				CheckValidity();
			}
		}
	}
}
