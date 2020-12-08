using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PostEffect;
using TMPro;

namespace MainGame.Tutorial {
	public class TutorialManager : MonoBehaviour, IActionRegistrable {
		[SerializeField] Character player;
		[SerializeField] Character enemy;
		[SerializeField] Result result;
		[SerializeField] Navigator navigator;
		[SerializeField] Util.TouchEventHandler touchEventHandler;
		[SerializeField] SpriteRenderer spLine;
		[SerializeField] ShojiAnimator shojiAnimator;
		[SerializeField] CameraMonoTone monoToneEffect;
		[SerializeField] DeadlyGauge deadlyGauge;
		[SerializeField] FinishAnnounce finishAnnounce;
		[SerializeField] Navigator navigator2;
		[SerializeField] GameObject throwLine2;
		[SerializeField] TextMeshPro deadlyText;

		bool IsPlayerDead { get; set; }
		bool IsEnemyDead { get; set; }
		bool IsEnemyHit { get; set; }
		bool IsDeadlyAnimationEnd { get; set; }
		int ActionMonitorCount { get; set; }
		int NowActionEndCount { get; set; }
		List<IActionMonitorable> MonitorableObjects { get; set; } = new List<IActionMonitorable>();


		void Awake () {
			touchEventHandler.OnTouchStartListener += pos => navigator.gameObject.SetActive(false);
			touchEventHandler.OnTouchReleaseListener += pos => spLine.enabled = false;

			player.OnReceiveDamageListener += OnPlayerReceiveDamage;
			enemy.OnReceiveDamageListener += OnEnemyReceiveDamage;

			player.OnDeadlyMoveStartListener += OnStartDeadlyMove;
			player.OnDeadlyMoveEndListener += OnEndDeadlyMove;
			enemy.OnDeadlyMoveStartListener += OnStartDeadlyMove;
			enemy.OnDeadlyMoveEndListener += OnEndDeadlyMove;

			deadlyText.DOFade(0, 0);
		}

		void Start () {
			navigator.PlayAnimation();
			throwLine2.SetActive(false);
			StartCoroutine(MainRoutine());
		}

		IEnumerator MainRoutine () {
			NowActionEndCount = 0;
			RefreshMonitarableObjects();

			player.Activate(false);
			enemy.Deactivate();

			yield return new WaitWhile(() => ActionMonitorCount != NowActionEndCount);

			if (!IsEnemyHit) {
				SceneManager.Instance.ChangeSceneFade(SceneManager.SceneNames.Tutorial);
				yield break;
			}

			NowActionEndCount = 0;
			RefreshMonitarableObjects();

			player.Activate(false);
			enemy.Deactivate();
			enemy.transform.position = enemy.transform.position.SetX(1.17f);

			IsEnemyHit = false;
			navigator2.PlayAnimation();
			throwLine2.SetActive(true);
			touchEventHandler.OnTouchStartListener += Navigator2Deactivate;
			touchEventHandler.OnTouchReleaseListener += ThrowLine2Deactivate;

			yield return new WaitWhile(() => ActionMonitorCount != NowActionEndCount);

			touchEventHandler.OnTouchStartListener -= Navigator2Deactivate;
			touchEventHandler.OnTouchReleaseListener -= ThrowLine2Deactivate;

			if (!IsEnemyHit) {
				SceneManager.Instance.ChangeSceneFade(SceneManager.SceneNames.Tutorial);
				yield break;
			}

			RefreshMonitarableObjects();
			finishAnnounce.PlayFadeAnimation(1, 0.85f, 0.0f, () => finishAnnounce.PlayDissolveAnimation(1, 1.75f, 1.0f));

			player.Deactivate();
			enemy.ResetPosition();
			enemy.PlayFluffyAnimation();

			yield return new WaitForSeconds(3.0f);

			bool? isGaugeSuccess = null;
			deadlyGauge.PlayAnimation(isSuccess => isGaugeSuccess = isSuccess);

			deadlyText.DOFade(1, 0.5f);
			yield return new WaitWhile(() => isGaugeSuccess == null);

			deadlyText.DOFade(0, 0.5f);
			yield return Statics.WaitHalfSeconds;

			if (!isGaugeSuccess.Value) {
				SceneManager.Instance.ChangeSceneFade(SceneManager.SceneNames.Tutorial);
				yield break;
			}

			player.PlayDeadlyAnimation();

			yield return new WaitWhile(() => !IsDeadlyAnimationEnd);

			PlayerDataManager.Instance.SetTutorialCleared();
			PlayerDataManager.Instance.Save();

			touchEventHandler.ChangeTouchable(false);
			TenjinManager.Instance.SendEvent("Tutorial Cleared");
			result.Open(true);
		}

		void RefreshMonitarableObjects () {
			// 動作済みの行動監視対象を破棄. 動いたかの判別はそれぞれで
			for (int i = MonitorableObjects.Count - 1; i >= 0; i--) {
				MonitorableObjects[i].Dispose();
			}

			// 行動監視対象の初期設定を実行. ギミックは動作するまで終了状態の想定
			foreach (var monitable in MonitorableObjects) {
				monitable.SetFirstAct();
			}
		}

		public void Register (IActionMonitorable monitorable) {
			ActionMonitorCount++;
			monitorable.OnActEnd += OnMonitarableActionEnd;
			MonitorableObjects.Add(monitorable);
		}

		public void Unregister (IActionMonitorable monitorable) {
			monitorable.OnActEnd -= OnMonitarableActionEnd;
			ActionMonitorCount--;
			MonitorableObjects.Remove(monitorable);
		}

		void OnMonitarableActionEnd (bool isActEnd) {
			NowActionEndCount += isActEnd ? 1 : -1;
		}

		void OnPlayerReceiveDamage (bool isDead) {
			IsPlayerDead = isDead;
		}

		void OnEnemyReceiveDamage (bool isDead) {
			IsEnemyDead = isDead;
			IsEnemyHit = true;
		}

		void OnStartDeadlyMove () {
			monoToneEffect.enabled = true;

			float threshold = 0.0f;
			DOTween.To(() => threshold, t => threshold = t, 1.0f, 1.0f).OnUpdate(() => {
				monoToneEffect.SetThreshold(threshold);
			});
		}

		void OnEndDeadlyMove () {
			StartCoroutine(PlayDeadlyAnimation());
		}

		void ResetCharacterPositions () {
			player.ResetPosition();
			enemy.ResetPosition();

			shojiAnimator.OpenStartOver();
		}

		IEnumerator PlayShojiAnimation () {
			bool isAnimating = true;
			shojiAnimator.CloseStartOver(() => isAnimating = false);
			yield return new WaitWhile(() => isAnimating);

			player.ResetPosition();
			enemy.ResetPosition();

			yield return Statics.WaitOneSeconds;

			isAnimating = true;
			shojiAnimator.OpenStartOver(() => isAnimating = false);
			yield return new WaitWhile(() => isAnimating);

			yield return Statics.WaitHalfSeconds;

		}

		IEnumerator PlayDeadlyAnimation () {
			yield return Statics.WaitQuarterSeconds;

			bool isAnimating = true;
			shojiAnimator.CloseBlood(() => isAnimating = false);

			yield return new WaitWhile(() => isAnimating);

			if (IsPlayerDead) {
				player.FallDown();
				enemy.Deactivate();
			} else {
				enemy.FallDown();
				player.Deactivate();
			}

			isAnimating = true;
			shojiAnimator.OpenBlood(() => isAnimating = false);

			yield return Statics.WaitHalfSeconds;

			yield return new WaitWhile(() => isAnimating);

			float threshold = 1.0f;
			DOTween.To(() => threshold, t => threshold = t, 0.0f, 0.25f).OnUpdate(() => {
				monoToneEffect.SetThreshold(threshold);
			});

			yield return new WaitForSeconds(2.5f);

			IsDeadlyAnimationEnd = true;
		}

		void ThrowLine2Deactivate (Vector2 pos) {
			throwLine2.SetActive(false);
		}

		void Navigator2Deactivate (Vector2 pos) {
			navigator2.gameObject.SetActive(false);
		}
	}
}
