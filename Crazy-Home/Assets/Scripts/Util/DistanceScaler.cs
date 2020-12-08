using UnityEngine;

namespace Util {
	public class DistanceScaler : MonoBehaviour {
		Vector3 BaseScale { get; set; }

		void Start () {
			BaseScale = transform.localScale / GetDistance();
		}

		void Update () {
			transform.localScale = BaseScale * GetDistance();
		}

		// カメラからの距離を取得
		float GetDistance () {
			return (transform.position - Camera.main.transform.position).magnitude;
		}
	}
}
