using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Data.Master {
	[CreateAssetMenu(fileName = "StageMaster", menuName = "ScriptableObject/StageMaster")]
	public class StageMaster : MasterBase {
		[SerializeField] List<StageParam> stageParams;

		public const int FirstId = 1;
		public const int RankMin = 1;
		public const int RankMax = 3;
		public const string ClassPath = "Data/StageMaster";
		const int MirrorMin = 1;

		[System.Serializable]
		public class StageParam {
			public int id;
		}

		public int StageCount { get { return Params.Count; } }
		List<StageParam> Params { get { return stageParams; } }
		StageParam SendParamCache { get; set; }


		protected override void CheckValidity () {
			Assert.IsTrue(Params.Select(p => p.id).Min() == FirstId, "IDの最小値は" + FirstId + "です");
			Assert.IsTrue(Params.Count == Params.Select(p => p.id).Distinct().Count(), "IDが重複しています");

			//foreach (var p in Params) {
			//	Assert.IsTrue(p.mirrorNum > MirrorMin, "鏡の最小値は"+MirrorMin+"です");
			//}
		}

		//public StageParam GetStageParam (int stageId) {
		//	var param = Params.First(p => p.id == stageId);

		//	if (SendParamCache == null) {
		//		SendParamCache = new StageParam {
		//			id = param.id,
		//			mirrorNum = param.mirrorNum,
		//			useColorLens = param.useColorLens,
		//		};
		//	} else {
		//		SendParamCache.id = param.id;
		//		SendParamCache.mirrorNum = param.mirrorNum;
		//		SendParamCache.useColorLens = param.useColorLens;
		//	}

		//	return SendParamCache;
		//}

		public string GetPrefabPath (int stageId) {
			return "Stage/Stage_"+stageId;
		}

		public List<int> GetStageIdList () {
			return Params.Select(p => p.id).ToList();
		}

		public bool IsStageExists (int stageId) {
			return Params.Find(p => p.id == stageId) != null;
		}

		public int EvaluteRank (int lastScore) {
			if (lastScore >= RankMax) {
				return RankMax;
			}

			if (lastScore <= RankMin) {
				return RankMin;
			}

			return lastScore;
		}
	}
}
