using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame {
	public class GimmickBase : MonoBehaviour, IActionMonitorable {
		public virtual event Action<bool> OnActEnd;

		protected IActionRegistrable RegisterComponent { get; set; }
		protected bool IsMoveStart { get; set; }


		protected virtual void Awake () {
			FindRegisterComponent();

		}

		public virtual void Activate () {

		}

		public virtual void Dispose () {
			if (!IsMoveStart) return;

			RegisterComponent.Unregister(this);
			Destroy(gameObject);
		}

		public virtual void FindRegisterComponent () {
			RegisterComponent = GameObjectExtensions.FindComponentWithInterface<IActionRegistrable>();
			RegisterComponent.Register(this);
		}

		public virtual void SetFirstAct () {
		}
	}
}
