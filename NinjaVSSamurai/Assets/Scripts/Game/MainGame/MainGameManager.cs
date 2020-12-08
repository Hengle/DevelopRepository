using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Util;
using PostEffect;

namespace MainGame {
	public class MainGameManager : MonoBehaviour, IActionRegistrable {
		[SerializeField] Character player;
		[SerializeField] Character enemy;
		[SerializeField] ShojiAnimator shojiAnimator;
		[SerializeField] Result result;
		[SerializeField] DeadlyGauge deadlyGauge;
		[SerializeField] FinishAnnounce finishAnnounce;
		[SerializeField] CameraMonoTone monoToneEffect;
		[SerializeField] List<GameObject> stagePatterns = new List<GameObject>();
		[SerializeField] DeadlyGauge gauge;

		const int ThrowFailureToleranceCount = 4;

		bool IsPlayerDead { get; set; }
		bool IsEnemyDead { get; set; }
		bool IsDeadlyAnimationEnd { get; set; }
		int NowPatternId { get; set; }
		int ThrowFailureCount { get; set; } = ThrowFailureToleranceCount;
		int ActionMonitorCount { get; set; }
		int NowActionEndCount { get; set; }
		List<IActionMonitorable> MonitorableObjects { get; set; } = new List<IActionMonitorable>();


		void Awake () {
			monoToneEffect.enabled = false;
			if (!PlayerDataManager.Instance.IsTutorialCleared) {
				SceneManager.Instance.ChangeScene(SceneManager.SceneNames.Tutorial);
				return;
			}

			// ランダムでステージ選択
			// PatternIdはイベント名として使用するため, Listのindexプラス1
			NowPatternId = UnityEngine.Random.Range(0, stagePatterns.Count) + 1;
			for (int i=1; i<stagePatterns.Count + 1; i++) {
				int key = i - 1;
				stagePatterns[key].SetActive(i == NowPatternId);
			}

			player.OnReceiveDamageListener += OnPlayerReceiveDamage;
			enemy.OnReceiveDamageListener += OnEnemyReceiveDamage;

			player.OnDeadlyMoveStartListener += OnStartDeadlyMove;
			player.OnDeadlyMoveEndListener += OnEndDeadlyMove;
			enemy.OnDeadlyMoveStartListener += OnStartDeadlyMove;
			enemy.OnDeadlyMoveEndListener += OnEndDeadlyMove;

			// 画面作成に邪魔なので消していたオブジェクトをActivite
			shojiAnimator.gameObject.SetActive(true);
			result.gameObject.SetActive(true);
			finishAnnounce.gameObject.SetActive(true);
		}

		void Start () {
			gauge.SetGaugePercent(UnityEngine.Random.Range(0.08f, 0.25f));
			if (!PlayerDataManager.Instance.IsTutorialCleared) return;

			TenjinManager.Instance.SendEvent("Pattern - " + NowPatternId + " : Played");
			StartCoroutine(MainRoutine());
		}

		//void Update () {
		//	Debug.Log("ActionMonitorCount:" + ActionMonitorCount);
		//	Debug.Log("NowActionEndCount:" + NowActionEndCount);
		//}

		IEnumerator MainRoutine () {
			bool isPlayerTurn = true;
			while (true) {
				NowActionEndCount = 0;
				RefreshMonitarableObjects();

				if (isPlayerTurn) {
					enemy.Deactivate();
					player.Activate(false);
				} else {
					player.Deactivate();
					enemy.Activate(true);
				}

				ThrowFailureCount--;

				yield return new WaitWhile(() => ActionMonitorCount != NowActionEndCount);

				// 両方死んでいたらプレイヤーの勝ち
				if (IsPlayerDead && IsEnemyDead) {
					IsPlayerDead = false;
					break;
				}

				if (IsPlayerDead || IsEnemyDead) {
					break;
				}

				if (ThrowFailureCount < 1) {
					yield return StartCoroutine(PlayShojiAnimation());
					ThrowFailureCount = ThrowFailureToleranceCount;
					TenjinManager.Instance.SendEvent("Pattern - " + NowPatternId + " : Start Overed");
				}

				isPlayerTurn = !isPlayerTurn;
			}

			RefreshMonitarableObjects();
			finishAnnounce.PlayFadeAnimation(1, 0.85f, 0.0f, () => finishAnnounce.PlayDissolveAnimation(1, 1.75f, 1.0f));

			if (IsPlayerDead) {
				enemy.Deactivate();
				player.PlayFluffyAnimation();
			} else {
				player.Deactivate();
				enemy.PlayFluffyAnimation();
			}

			yield return new WaitForSeconds(3.0f);

			bool? isGaugeSuccess = null;
			deadlyGauge.PlayAnimation(isSuccess => isGaugeSuccess = isSuccess);

			// 敵が勝った場合は自動で必殺技発動
			if (IsPlayerDead) {
				deadlyGauge.ForceStopIndicator();
			}

			yield return new WaitWhile(() => isGaugeSuccess == null);
			yield return Statics.WaitHalfSeconds;

			if (isGaugeSuccess.Value) {
				if (IsPlayerDead) {
					enemy.PlayDeadlyAnimation();
				} else {
					TenjinManager.Instance.SendEvent("Pattern - " + NowPatternId + " : DealdyMoveSuccess");
					player.PlayDeadlyAnimation();
				}

				yield return new WaitWhile(() => !IsDeadlyAnimationEnd);
			} else {
				yield return Statics.WaitHalfSeconds;
			}

			if (IsEnemyDead) {
				TenjinManager.Instance.SendEvent("Pattern - " + NowPatternId + " : Won");
				PlayerDataManager.Instance.AddCoin(100);
				PlayerDataManager.Instance.Save();
			}

			result.Open(IsEnemyDead);
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
			ThrowFailureCount = ThrowFailureToleranceCount;
			IsPlayerDead = isDead;
		}

		void OnEnemyReceiveDamage (bool isDead) {
			ThrowFailureCount = ThrowFailureToleranceCount;
			IsEnemyDead = isDead;
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

	}
}
