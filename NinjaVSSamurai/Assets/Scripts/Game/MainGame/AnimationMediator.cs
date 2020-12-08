using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame {
	public class AnimationMediator : MonoBehaviour {
		public event Action OnThrowListener;
		public event Action OnDeadlyMoveStartListener;
		public event Action OnDeadlyMoveEndListener;

		void OnThrow () {
			OnThrowListener?.Invoke();
		}

		void OnDeadlyMoveStart () {
			OnDeadlyMoveStartListener?.Invoke();
		}

		void OnDeadlyMoveEnd () {
			OnDeadlyMoveEndListener?.Invoke();
		}
	}
}
