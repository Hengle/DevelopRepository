using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Util;
using LionStudios;

public class GameManager : MonoBehaviour
{
    public enum GameMode
    {
        FPS,
        TPS
    }

    [SerializeField] CinemachineVirtualCameraBase characterCamera;
    [SerializeField] CinemachineVirtualCameraBase handCamera;
    [SerializeField] CinemachineVirtualCameraBase helicopterCamera;
    [SerializeField] GameObject characterObj;
    [SerializeField] GameObject handObj;
    [SerializeField] GameObject helicopterObj;
    [SerializeField] GameObject goalObj;
    [SerializeField] MultipleParticles resultParticle;

    [SerializeField] CanvasGroup home;
    [SerializeField] CanvasGroup guideLine;
    [SerializeField] CanvasGroup result;

    [SerializeField] GameMode gameMode;

    Character character;
    HandController hand;
    Helicopter helicopter;
    bool isGrab, isGameClear, isGameOver, waitPhase;

    private void Awake()
    {
        character = characterObj.transform.GetComponent<Character>();
        hand = handObj.transform.GetComponent<HandController>();
        helicopter = helicopterObj.GetComponent<Helicopter>();
    }

    private void Start()
    {
        waitPhase = true;

        hand.OnGrabListener += Grab;
        character.OnGrabListener += Grab;
        character.OnGoalListener += Goal;
        helicopter.OnDeathListener += Death;
        StartCoroutine(MainRoutine());

        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        if (gameMode == GameMode.FPS)
        {
            eventParams["GameMode"] = "FPS";
            eventParams["Stage"] = DataManager.Instance.LoopCount + 1;
        }
        else
        {
            eventParams["GameMode"] = "TPS";
            eventParams["Stage"] = DataManager.Instance.LoopCount + 1;
        }
        Analytics.Events.LevelStarted(eventParams);
    }

    private IEnumerator MainRoutine()
    {
        yield return new WaitWhile(() => !Input.GetMouseButtonDown(0));

        home.alpha = 0f;
        character.StartAnimation();

        yield return new WaitForSeconds(2.0f);

        if (gameMode == GameMode.FPS)
        {
            handCamera.Priority = 2;
        }
        else
        {
            helicopterCamera.Priority = 2;
        }

        yield return new WaitForSeconds(0.5f);

        if (gameMode == GameMode.FPS)
        {
            //掴みシーン
            handObj.SetActive(true);
            hand.Activate();
            guideLine.alpha = 1f;
        }
        else
        {
            guideLine.alpha = 1f;
            helicopter.Activate();
            character.Rescue();
        }

        yield return new WaitWhile(() => !Input.GetMouseButtonDown(0));

        guideLine.alpha = 0f;
        helicopter.SetMoveFlg(true);

        if (gameMode == GameMode.FPS)
        {
            //掴むか指定ラインを超えると次のフェイズへ
            yield return new WaitWhile(() => waitPhase && hand.transform.position.z - helicopter.transform.position.z > 5f);
        }
        else
        {
            yield return new WaitWhile(() => !isGrab && (character.transform.position.z + 5f)  - helicopter.transform.position.z > 5f);

            if (!isGrab)
            {
                isGameOver = true;
            }
        }


        if (gameMode == GameMode.FPS)
        {
            //掴みシーン削除
            handObj.SetActive(false);
            helicopter.Stop();
            var pos = helicopter.transform.position;
            pos.z = character.transform.position.z - 1f;
            helicopter.transform.position = pos;
            helicopterCamera.Priority = 3;

            if (isGrab)
            {
                character.Rescue();

                yield return new WaitForSeconds(0.5f);

                helicopter.SetMoveFlg(true);
                helicopter.Activate();
            }
            else
            {
                Dictionary<string, object> eventParams = new Dictionary<string, object>();
                eventParams["FailedType"] = "Grab";
                Analytics.Events.LevelFailed(eventParams);
                isGameOver = true;
            }
        }

        //成功、失敗判定
        yield return new WaitWhile(() => !isGameClear && !isGameOver && (goalObj.transform.position.z + 5f) - helicopter.transform.position.z > 0f);

        if (isGameClear)
        {
            characterCamera.Priority = 4;
            helicopter.Stop();
            resultParticle.Play();
            result.alpha = 1f;
            result.blocksRaycasts = true;
        }else if(isGameOver)
        {
            Invoke("Retry", 1.5f);
        }
        else
        {
            Dictionary<string, object> eventParams = new Dictionary<string, object>();
            eventParams["FailedType"] = "OverRun";
            Analytics.Events.LevelFailed(eventParams);
            Invoke("Retry", 1.5f);
        }
    }

    void Grab(bool isGrab)
    {
        waitPhase = false;
        this.isGrab = isGrab;
    }

    void Goal()
    {
        isGameClear = true;
    }

    void Death()
    {
        isGameOver = true;
    }

    public void Next()
    {
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        if (gameMode == GameMode.FPS)
        {
            eventParams["GameMode"] = "FPS";
            eventParams["Stage"] = DataManager.Instance.LoopCount + 1;
            Analytics.Events.LevelComplete(eventParams);
            DataManager.Instance.ClearFPS();
            DataManager.Instance.Save();
            SceneManager.Instance.ChangeScene("TPS");
        }
        else
        {
            eventParams["GameMode"] = "TPS";
            eventParams["Stage"] = DataManager.Instance.LoopCount + 1;
            Analytics.Events.LevelComplete(eventParams);
            DataManager.Instance.ClearTPS();
            DataManager.Instance.Save();
            SceneManager.Instance.ChangeScene("FPS");
        }
    }

    void Retry()
    {
        if (gameMode == GameMode.FPS)
        {
            SceneManager.Instance.ChangeScene("FPS");
        }
        else
        {
            SceneManager.Instance.ChangeScene("TPS");
        }
    }
}
