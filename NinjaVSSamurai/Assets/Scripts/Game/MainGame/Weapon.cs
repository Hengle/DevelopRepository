using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame {
	[RequireComponent(typeof(BoxCollider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(SpriteRenderer))]
	public class Weapon : MonoBehaviour, IActionMonitorable {
		[SerializeField] float defaultForce = 90.0f;
		[SerializeField] DamageEffect efDamagePrefab;
		[SerializeField] Transform tfLeftRayPointFirst;
		[SerializeField] Transform tfLeftRayPointSecond;
		[SerializeField] Transform tfRightRayPointFirst;
		[SerializeField] Transform tfRightRayPointSecond;

		public event Action<bool> OnActEnd;

		IActionRegistrable RegisterComponent { get; set; }
		SpriteRenderer MyRenderer { get; set; }
		BoxCollider2D MyCollider { get; set; }
		Rigidbody2D MyRigidbody { get; set; }
		DamageEffect MyDamageEffect { get; set; }
		Vector2 BeforePos { get; set; }
		GameObject IgnoreDamagableObject { get; set; }
		string OwnerTag { get; set; }
		string HitCharacterTag { get; set; }
		bool IsSendDamaged { get; set; }
		bool IsThrowEnd { get; set; }

		const int DefaultDamage = 3;


		void Awake () {
			FindRegisterComponent();
			OnActEnd(true);

			MyRenderer = GetComponent<SpriteRenderer>();
			MyCollider = GetComponent<BoxCollider2D>();
			MyRigidbody = GetComponent<Rigidbody2D>();

			MyRigidbody.gravityScale = 0;
			MyRigidbody.simulated = false;
			MyRenderer.enabled = false;

			BeforePos = transform.position;

			MyDamageEffect = Instantiate(efDamagePrefab);
			MyDamageEffect.transform.localScale = efDamagePrefab.transform.localScale;
		}

		void Start () {
			AudioManager.Instance.LoadSe(AudioManager.SeName.Throw);
		}

		void OnCollisionEnter2D (Collision2D collision) {
			if (IsThrowEnd) return;

			var iBreakable = collision.gameObject.GetComponent<IBreakable>();
			if (iBreakable != null) {
				iBreakable.Break();
				Stop();
				MyRenderer.enabled = false;
				StartCoroutine(DelayInvoke(1.0f, () => OnActEnd?.Invoke(true)));
			}

			if (collision.gameObject.tag == "PlayerBone") {
				transform.SetParent(collision.gameObject.transform);
				//MyDamageEffect.Play(collision.contacts[0].point);
				Stop();
				OnActEnd?.Invoke(true);
			} else if (collision.gameObject.tag == "PhysicsStopper") {
				Stop();
				StartCoroutine(DelayInvoke(1.0f, () => OnActEnd?.Invoke(true)));
			} else {
				transform.up = Util.VectorUtility.Reflect(transform.up, collision.contacts[0].normal);
			}
		}

		void OnTriggerEnter2D (Collider2D collision) {
			if (IsThrowEnd) return;
			if (IsSendDamaged) return;
			if (collision.gameObject != null && collision.gameObject.tag == OwnerTag) return;

			var iReceiveDamagable = collision.gameObject.GetComponent<IReceiveDamagable>();
			if (iReceiveDamagable != null) {
				iReceiveDamagable.ReceiveDamage(DefaultDamage);
				MyDamageEffect.Play(transform.position, iReceiveDamagable.IsCritical());
				IsSendDamaged = true;
			}
		}

		public void FindRegisterComponent () {
			RegisterComponent = GameObjectExtensions.FindComponentWithInterface<IActionRegistrable>();
			RegisterComponent.Register(this);
		}

		public void SetOwnerType (Character.Type type) {
			switch (type) {
				case Character.Type.Player:
					OwnerTag = "PlayerHitTrigger";
					HitCharacterTag = "EnemyHitTrigger";
					break;
				case Character.Type.Enemy:
					OwnerTag = "EnemyHitTrigger";
					HitCharacterTag = "PlayerHitTrigger";
					break;
			}
		}

		public void Throw (Vector2 direction) {
			AudioManager.Instance.PlaySe(AudioManager.SeName.Throw);
			OnActEnd(false);
			MyRenderer.enabled = true;
			MyRigidbody.simulated = true;
			MyRigidbody.gravityScale = 0;
			MyRigidbody.AddForce(direction * defaultForce, ForceMode2D.Impulse);
			transform.up = direction;
		}

		public void CheckRayHit (Vector2 direction, Action onHit) {
			transform.up = direction;

			// 武器の当たり判定と同サイズのレイでチェック
			//var isCenterHit = IsRayHit(transform.position, transform.up);
			var isLeftHitFirst = RayCast(tfLeftRayPointFirst.position, transform.up);
			var isRightHitFirst = RayCast(tfRightRayPointFirst.position, transform.up);

			// 判定の角への反射を考慮し,遊びをもたせたレイで再チェック
			if (isLeftHitFirst != null && isRightHitFirst != null) {
				if (direction.x <= 0.0f) {
					if (RayCast(tfLeftRayPointSecond.position, transform.up) != null) {
						onHit.Invoke();
					}
				} else {
					if (RayCast(tfRightRayPointSecond.position, transform.up) != null) {
						onHit.Invoke();
					}
				}
			}
		}

		void Stop () {
			MyRigidbody.simulated = false;
			MyRigidbody.velocity = Vector2.zero;
			IsThrowEnd = true;
		}

		RaycastHit2D? RayCast (Vector2 forward, Vector2 direction) {
			var hit = Physics2D.Raycast(forward, direction, Mathf.Infinity, LayerMask.GetMask(new string[] { "Wall", "Reflector", "Character" }));

			Debug.DrawRay(forward, direction * 100, Color.red);
			if (hit.collider != null && hit.collider.tag != "PhysicsStopper") {
				if (hit.collider.tag == HitCharacterTag) {
					return hit;
				}

				var inDirection = Util.VectorUtility.Reflect(transform.up, hit.normal);

				Debug.DrawRay(hit.point, inDirection * 100, Color.green);
				hit = Physics2D.Raycast(hit.point + hit.normal * 0.01f, inDirection, Mathf.Infinity, LayerMask.GetMask(new string[] { "Wall", "Reflector", "Character" }));
				if (hit.collider != null && hit.collider.tag != "PhysicsStopper") {
					if (hit.collider.tag == HitCharacterTag) {
						return hit;
					}

					inDirection = Util.VectorUtility.Reflect(inDirection, hit.normal);
					Debug.DrawRay(hit.point, inDirection * 100, Color.yellow);

					hit = Physics2D.Raycast(hit.point + hit.normal * 0.01f, inDirection, Mathf.Infinity, LayerMask.GetMask(new string[] { "Wall", "Reflector", "Character" }));
					if (hit.collider != null && hit.collider.tag == HitCharacterTag) {
						return hit;
					}
				}
			}
			return null;
		}

		IEnumerator DelayInvoke (float time, Action onInvoke) {
			yield return new WaitForSeconds(time);
			onInvoke.Invoke();
		}

		public void SetFirstAct () {
		}

		public void Dispose () {
			if (!IsThrowEnd) return;

			MyCollider.enabled = false;
			MyRigidbody.simulated = false;
			RegisterComponent.Unregister(this);

			Destroy(MyDamageEffect);
			Destroy(gameObject);
		}

	}
}
