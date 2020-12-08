using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	public static class RandomUtility {
		/// <summary>
		/// 重み付き抽選
		/// </summary>
		public static int ChooseByWeight (Dictionary<int, int> chooseTargets) {
			int totalWeight = 0;
			foreach (var map in chooseTargets) {
				totalWeight += map.Value;
			}

			var randomValue = Random.Range(0, totalWeight);
			foreach (var map in chooseTargets) {
				if (randomValue < map.Value) {
					return map.Key;
				}
				randomValue -= map.Value;
			}

			Debug.LogError("抽選対象の確率が不正です");
			return -1;
		}
	}
}
