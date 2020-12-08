using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Util {
	[RequireComponent(typeof(Button))]
	public class PlaySeButton : MonoBehaviour {
		[SerializeField] AudioManager.SeName seName;
		[SerializeField] bool isOneShot = true;

		bool IsRepeating { get; set; }
		bool IsOneShotEnd { get; set; }



		void Start () {
			AudioManager.Instance.LoadSe(seName);
			GetComponent<Button>().onClick.AddListener(() => {
				if (IsOneShotEnd || IsRepeating) return;

				AudioManager.Instance.PlaySe(seName);
				StartCoroutine("WaitInput");

				IsOneShotEnd |= isOneShot;
			});
		}

		IEnumerator WaitInput () {
			IsRepeating = true;
			yield return Statics.WaitQuarterSeconds;
			IsRepeating = false;
		}
	}
}
