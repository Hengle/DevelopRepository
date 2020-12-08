using System;
using System.Collections;
using System.Collections.Generic;

namespace Data.Model {
	[Serializable]
	public class Player {
		public int coin;
		public bool isTutorialCleared;
		//public int skinId;
		//public int nowStageId;
		//public List<ArrivalStageParam> arrivalStages = new List<ArrivalStageParam>();
		//public List<FailureStageParam> failureStages = new List<FailureStageParam>();
		//public List<int> acquiredSkinIds = new List<int>();
	}

	[Serializable]
	public class ArrivalStageParam {
		public int stageId;
		public int rank;
	}

	[Serializable]
	public class FailureStageParam {
		public int stageId;
		public int count;
	}
}
