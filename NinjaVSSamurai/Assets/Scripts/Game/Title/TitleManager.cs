using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Title {
	public class TitleManager : MonoBehaviour {
		[SerializeField] MainGame.Character character;
		[SerializeField] TextMeshProUGUI coinLabel;

		void Start () {
			character.Deactivate();
			coinLabel.SetText(PlayerDataManager.Instance.Coin.ToString());
		}
	}
}
