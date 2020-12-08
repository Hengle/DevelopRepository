using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace MainGame {
	public class Character : MonoBehaviour, IActionMonitorable {
		[SerializeField] Animator animator;
		[SerializeField] Rigidbody2D rootBoneRigid;
		[SerializeField] GameObject objThrowDirectionIK;
		[SerializeField] TouchEventHandler touchEventHandler;
		[SerializeField] Weapon weaponPrefab;
		[SerializeField] AnimationMediator animationMediator;
		[SerializeField] LineRenderer lineRenderer;
		[SerializeField] Transform armPosition;
		[SerializeField] HitpointGauge hitpointGauge;
		[SerializeField] List<CharacterHitTrigger> hitTriggers;
		[SerializeField] Type myType;
		[SerializeField] bool isFlip;
		[SerializeField] GameObject deadBody;
		[SerializeField] List<Rigidbody2D> deadBodyRigids = new List<Rigidbody2D>();
		[SerializeField] GameObject meshes;

		const int MaxHitpoint = 9;

		public event Action<bool> OnReceiveDamageListener;
		public event Action<bool> OnActEnd;
		public event Action OnDeadlyMoveStartListener;
		public event Action OnDeadlyMoveEndListener;

		public enum Type { Player, Enemy };

		List<CharacterBone> MyBones { get; set; } = new List<CharacterBone>();
		IActionRegistrable RegisterComponent { get; set; }
		Weapon NowWeapon { get; set; }
		Vector2 DefaultPosition { get; set; }
		Vector2 ThrowStartPosition { get; set; }
		bool IsMoving { get; set; }
		bool IsReceiveDamage { get; set; }
		bool IsDead { get; set; }
		int NowHitpoint { get; set; }



		void Awake () {
			FindRegisterComponent();

			objThrowDirectionIK.SetActive(false);
			DefaultPosition = transform.position;

			animationMediator.OnThrowListener += ThrowWeapon;
			animationMediator.OnDeadlyMoveStartListener += OnDeadlyMoveStart;
			animationMediator.OnDeadlyMoveEndListener += OnDeadlyMoveEnd;

			foreach (Transform bone in rootBoneRigid.GetComponentsInChildren<Transform>()) {
				var myBone = bone.gameObject.AddComponent<CharacterBone>();
				MyBones.Add(myBone);
			}

			foreach (var trigger in hitTriggers) {
				trigger.OnDamageListener += ReceiveDamage;
			}

			NowHitpoint = MaxHitpoint;
		}

		void Start () {
			AudioManager.Instance.LoadSe(AudioManager.SeName.Hit_Voice);
			AudioManager.Instance.LoadSe(AudioManager.SeName.Hit_Voice_2);
			AudioManager.Instance.LoadSe(AudioManager.SeName.Hit_Voice_3);
			AudioManager.Instance.LoadSe(AudioManager.SeName.Hit_Weapon);

			if (isFlip) Flip(true);
		}

		void Update () {
			if (IsMoving && rootBoneRigid.velocity.magnitude <= 0.05f) {
				IsMoving = false;
				StartCoroutine(DelayInvoke(1.75f, () => {
					IsReceiveDamage = false;
					ResetPosture();
					OnActEnd?.Invoke(true);
				}));
			}
		}

		void OnTouchStart (Vector2 touchPos) {
			animator.enabled = false;
			objThrowDirectionIK.SetActive(true);

			// 子のLimbが上手く働かない. ON/OFFを切り替えで治るよう
			objThrowDirectionIK.transform.GetChild(0).gameObject.SetActive(false);
			objThrowDirectionIK.transform.GetChild(0).gameObject.SetActive(true);

			touchEventHandler.OnTouchKeepListener += OnTouchKeep;
			touchEventHandler.OnTouchReleaseListener += OnTouchRelease;
		}

		void OnTouchKeep (Vector2 touchPos) {
			touchPos.y = touchPos.y + 2;

			var normalDistance = (touchPos - (Vector2)armPosition.position).normalized * 10;
			Flip(normalDistance.x <= 0.0f);

			objThrowDirectionIK.transform.position = (Vector2)armPosition.position + normalDistance;

			lineRenderer.enabled = true;
			lineRenderer.SetPosition(0, armPosition.position);
			lineRenderer.SetPosition(1, (Vector2)armPosition.position + normalDistance);
		}

		void OnTouchRelease (Vector2 releasePos) {
			ThrowStartPosition = armPosition.position;
			DisabledPhysics();
			animator.enabled = true;
			animator.Play("Throw");
			objThrowDirectionIK.SetActive(false);
			lineRenderer.enabled = false;
			touchEventHandler.OnTouchStartListener -= OnTouchStart;
			touchEventHandler.OnTouchKeepListener -= OnTouchKeep;
			touchEventHandler.OnTouchReleaseListener -= OnTouchRelease;
		}

		public void FindRegisterComponent () {
			RegisterComponent = GameObjectExtensions.FindComponentWithInterface<IActionRegistrable>();
			if (RegisterComponent != null) {
				RegisterComponent.Register(this);
			}
		}

		public void Activate (bool isAI = true) {
			Refresh();
			CreateWeapon();

			if (isAI) {
				AutomaticThrow();
			} else {
				touchEventHandler.OnTouchStartListener += OnTouchStart;
			}
		}

		public void Deactivate () {
			Refresh();
			OnActEnd?.Invoke(true);
			touchEventHandler.OnTouchStartListener -= OnTouchStart;
			touchEventHandler.OnTouchKeepListener -= OnTouchKeep;
			touchEventHandler.OnTouchReleaseListener -= OnTouchRelease;
		}

		public void ResetPosition () {
			transform.position = DefaultPosition;
		}

		public void FallDown () {
			deadBody.SetActive(true);
			meshes.SetActive(false);

			foreach (var rigid in deadBodyRigids) {
				rigid.AddForce(new Vector2(UnityEngine.Random.Range(-5, 10), UnityEngine.Random.Range(4, 7)), ForceMode2D.Impulse);
				rigid.AddTorque(UnityEngine.Random.Range(-2, 2), ForceMode2D.Impulse);
			}

			AudioManager.Instance.PlaySe(AudioManager.SeName.Hit_Voice_3);
			AudioManager.Instance.PlaySe(AudioManager.SeName.Hit_Weapon);
		}

		// Debug用
		//void AutomaticThrow () {
		//	StartCoroutine(AutomaticThrow2());
		//}

		void AutomaticThrow () {
			var basePos = armPosition.position;
			OnTouchStart(Vector2.zero);

			// Rayを阻害するため一時的にTriggerをOFF
			DisabledTriggers();

			// 着弾位置をずらす幅を設定 ... 0fだと確実に当たる
			float throwRange = 0.0f;
			if (NowHitpoint > 6) {
				throwRange = 0.5f;
			} else if (NowHitpoint > 3) {
				throwRange = 0.25f;
			} else {
				throwRange = 0.1f;
			}

			float time = 0.0f;
			float limitTime = 10.0f;
			while (true) {
				var rotatePos = new Vector2(basePos.x + Mathf.Cos(time) * -2, basePos.y + Mathf.Sin(time) * 2);
				var direction = (rotatePos - (Vector2)armPosition.position).normalized;

				Flip(direction.x <= 0.0f);
				objThrowDirectionIK.transform.position = rotatePos;

				bool isHit = false;
				NowWeapon.transform.position = (Vector2)armPosition.position; // 中心に近い位置から発射
				NowWeapon.CheckRayHit(direction, () => isHit = true);

				if (isHit) {
					var randomVector = new Vector2(UnityEngine.Random.Range(-throwRange, throwRange), UnityEngine.Random.Range(-throwRange, throwRange));
					objThrowDirectionIK.transform.position = (Vector2)objThrowDirectionIK.transform.position + randomVector;
					break;
				}

				if (time > limitTime) {
					// 失敗時はランダムに投げる
					var randomVector = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(0.25f, 1.0f));
					Flip(randomVector.x <= 0.0f);
					objThrowDirectionIK.transform.position = (Vector2)armPosition.position + randomVector;
					break;
				}

				time += 0.01f;
			}

			EnabledTriggers();

			ThrowStartPosition = armPosition.position;
			OnTouchRelease(objThrowDirectionIK.transform.position);
		}

		void ReceiveDamage (int damage) {
			if (!IsReceiveDamage) {
				IsReceiveDamage = true;
				OnActEnd?.Invoke(false);
				animator.enabled = false;
				EnabledPhysics();

				if (IsDead) {
					foreach (var bone in MyBones) {
						bone.DetachJoint();
					}
					StartCoroutine(DelayInvoke(1.0f, () => OnActEnd?.Invoke(true)));
				} else {
					StartCoroutine(DelayInvoke(1.0f, () => IsMoving = true));
				}

				if (damage > 3) {
					AudioManager.Instance.PlaySe(AudioManager.SeName.Hit_Voice_2);
				} else {
					AudioManager.Instance.PlaySe(AudioManager.SeName.Hit_Voice);
				}
			}


			NowHitpoint -= damage > NowHitpoint ? NowHitpoint : damage;
			IsDead |= !IsDead && NowHitpoint < 1;
			hitpointGauge.PlayFillTween(NowHitpoint, MaxHitpoint);
			OnReceiveDamageListener?.Invoke(IsDead);

		}

		public void PlayFluffyAnimation () {
			DisabledPhysics();
			animator.enabled = true;
			animator.Play("Fluffy");
			SyncRootPosition();
		}

		public void PlayDeadlyAnimation () {
			DisabledPhysics();
			animator.enabled = true;
			animator.Play("Deadly");
			SyncRootPosition();
		}

		void Refresh () {
			ResetPosture();
			IsMoving = false;
			IsReceiveDamage = false;
		}

		void ThrowWeapon () {
			var direction = ((Vector2)objThrowDirectionIK.transform.position - ThrowStartPosition).normalized;
			NowWeapon.transform.position = ThrowStartPosition;
			NowWeapon.Throw(direction);
			OnActEnd?.Invoke(true);
		}

		void CreateWeapon () {
			NowWeapon = null;
			NowWeapon = Instantiate(weaponPrefab);
			NowWeapon.transform.position = weaponPrefab.transform.position;
			NowWeapon.transform.localScale = weaponPrefab.transform.localScale;
			NowWeapon.SetOwnerType(myType);
		}

		//void DisposeWeapon () {
		//	if (NowWeapon == null) return;

		//	NowWeapon.Dispose();
		//	NowWeapon = null;
		//}

		void OnDeadlyMoveStart () {
			OnDeadlyMoveStartListener?.Invoke();
		}

		void OnDeadlyMoveEnd () {
			OnDeadlyMoveEndListener?.Invoke();
		}

		void ResetPosture () {
			DisabledPhysics();
			animator.enabled = true;
			animator.Play("Idle");
			SyncRootPosition();
		}

		void SyncRootPosition () {
			transform.position = transform.position.SetX(rootBoneRigid.transform.position.x);
			rootBoneRigid.transform.localPosition = Vector3.zero;
		}

		void EnabledTriggers () {
			foreach (var trigger in hitTriggers) {
				trigger.gameObject.SetActive(true);
			}
		}

		void DisabledTriggers () {
			foreach (var trigger in hitTriggers) {
				trigger.gameObject.SetActive(false);
			}
		}

		void EnabledPhysics () {
			foreach (var bone in MyBones) {
				bone.ActivatePhysics();
			}
		}

		void DisabledPhysics () {
			foreach (var bone in MyBones) {
				bone.DeactivatePhysics();
			}
		}

		void Flip (bool isRight) {
			var boneEulerAngle = transform.localRotation.eulerAngles;
			boneEulerAngle.y = isRight ? 180f : 0f;
			transform.localRotation = Quaternion.Euler(boneEulerAngle);
		}

		IEnumerator DelayInvoke (float time, Action onInvoke) {
			yield return new WaitForSeconds(time);
			onInvoke.Invoke();
		}

		public void SetFirstAct () {

		}

		public void Dispose () {
		}
	}
}
