using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Util;
using DG.Tweening;
using Cinemachine;

public enum GameType
{
    Sewing,
    Wappen,
    Washing,
    Decoration,
    Packing,
}

public enum AccessoryPosition
{
    Head,
    Eye,
    Neck,
    Wappen
}

public enum Sex
{
    male,
    female,
}

public class GameManager : MonoBehaviour
{

    [SerializeField] Needle needle;
    [SerializeField] GameObject bubble;
    [SerializeField] List<int> hitIndex;
    [SerializeField] LayerMask layerMask;

    [SerializeField] GameObject progressBar;
    [SerializeField] GameObject selectButton;
    [SerializeField] DecoratButton[] decoratButtons;
    [SerializeField] CanvasGroup textArea;
    [SerializeField] TextMeshProUGUI textAreaText;

    [SerializeField] GameObject CottonMachine;
    [SerializeField] GameObject water;
    [SerializeField] ParticleSystem smoke;

    [SerializeField] StageMaster stageMaster;
    [SerializeField] DecoretPatternMaster decoretPatternMaster;
    [SerializeField] Transform resultParticle;

    CinemachineVirtualCamera focusCamera;
    PlushDoll plushDoll;
    TouchEventHandler touchEventHandler;
    StageData stageData;
    DecoretPatternData decoretPatternData;
    Slider slider;
    GameType gameType;

    bool isSewingComplete;
    bool isWashingComplete;
    bool isPackingComplete;
    bool isDecoratComplete;
    bool isSelectWappen;

    //TODO:WashControllerの作成 
    private void Awake()
    {
        needle.OnSewingCompleteListener += OnSewingComplete;
        slider = GameObject.FindGameObjectWithTag("ProgressBar").GetComponent<Slider>();
        touchEventHandler = Camera.main.transform.GetComponent<TouchEventHandler>();
        stageData = stageMaster.GetStageData(DataManager.Instance.Day, DataManager.Instance.Task);
        plushDoll = Instantiate(stageData.PlushDollModel).GetComponent<PlushDoll>(); ;
        gameType = stageData.GameType;

    }

    void Start()
    {
        
        plushDoll.Initialize(stageData);

        switch (gameType)
        {
            case GameType.Sewing:
                needle.SetYarn(plushDoll.GetYarn());
                needle.SetStitches(plushDoll.GetStitches());
                focusCamera = plushDoll.GetFocusCamera();
                StartCoroutine(Sewing());
                break;
            case GameType.Wappen:
                decoretPatternData = decoretPatternMaster.GetDecoretPattern(stageData.AccessoryPattern);
                foreach (var decoratButton in decoratButtons)
                {
                    decoratButton.Initialize(decoretPatternData);
                    decoratButton.OnClickListener += CreateAccessory;
                }
                needle.SetYarn(plushDoll.GetYarn());
                needle.SetStitches(plushDoll.GetStitches());
                focusCamera = plushDoll.GetFocusCamera();
                selectButton.transform.GetComponent<CanvasGroup>().alpha = 1;
                StartCoroutine(Wappen());
                break;
            case GameType.Washing:
                progressBar.transform.GetComponent<CanvasGroup>().alpha = 1;
                plushDoll.OnWashListener += ProgressBarUpdate;
                StartCoroutine(Washing());
                break;
            case GameType.Decoration:
                decoretPatternData = decoretPatternMaster.GetDecoretPattern(stageData.AccessoryPattern);
                foreach (var decoratButton in decoratButtons)
                {
                    decoratButton.Initialize(decoretPatternData);
                    decoratButton.OnClickListener += CreateAccessory;
                }
                selectButton.transform.GetComponent<CanvasGroup>().alpha = 1;
                StartCoroutine(Decoration());
                break;
            case GameType.Packing:
                CottonMachine.SetActive(true);
                progressBar.transform.GetComponent<CanvasGroup>().alpha = 1;
                plushDoll.OnPackingListener += ProgressBarUpdate;
                StartCoroutine(Packing());
                break;
        }
    }

    IEnumerator Sewing()
    {
        textAreaText.text = "HOLD TO SEW";
        textArea.alpha = 0;

        yield return new WaitForSeconds(0.5f);

        focusCamera.Priority = 1;

        yield return new WaitForSeconds(0.5f);

        textArea.alpha = 1;
        TouchActivate();

        yield return new WaitWhile(() => !isSewingComplete);

        TouchDeActivate();

        yield return new WaitForSeconds(0.5f);

        SewingCompleteAnimation();

        yield return new WaitForSeconds(0.5f);

        resultParticle.Find("Sewing").position = Camera.main.transform.position;
        resultParticle.Find("Sewing").GetComponentInChildren<MultipleParticles>().Play();

        yield return new WaitForSeconds(1.0f);

        focusCamera.Priority = 0;
        if (stageData.ClientSex == Sex.female)
        {
            resultParticle.Find("Female").GetComponentInChildren<ParticleSystem>().Play();
        }
        else
        {
            resultParticle.Find("Male").GetComponentInChildren<ParticleSystem>().Play();
        }

        plushDoll.ResultAnimation();
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(Result());
    }

    IEnumerator Wappen()
    {
        textAreaText.text = "CHOOSE WAPPEN";

        yield return new WaitForSeconds(0.5f);

        selectButton.transform.GetComponent<CanvasGroup>().blocksRaycasts = true;

        yield return new WaitWhile(() => !isSelectWappen);

        focusCamera.Priority = 1;

        yield return new WaitForSeconds(0.5f);

        textAreaText.text = "HOLD TO SEW";
        textArea.alpha = 1.0f;

        TouchActivate();

        yield return new WaitWhile(() => !isSewingComplete);

        TouchDeActivate();

        yield return new WaitForSeconds(0.5f);

        SewingCompleteAnimation();

        yield return new WaitForSeconds(1.0f);

        resultParticle.Find("Wappen").position = Camera.main.transform.position;
        resultParticle.Find("Wappen").GetComponentInChildren<MultipleParticles>().Play();

        yield return new WaitForSeconds(0.5f);

        focusCamera.Priority = 0;
        if (stageData.ClientSex == Sex.female)
        {
            resultParticle.Find("Female").GetComponentInChildren<ParticleSystem>().Play();
        }
        else
        {
            resultParticle.Find("Male").GetComponentInChildren<ParticleSystem>().Play();
        }

        plushDoll.ResultAnimation();
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(Result());
    }

    IEnumerator Washing()
    {
        textAreaText.text = "SWIPE TO WASH";

        yield return new WaitForSeconds(0.5f);

        TouchActivate();

        yield return new WaitWhile(() => !isWashingComplete);

        TouchDeActivate();

        yield return new WaitForSeconds(0.5f);

        WashAnimation();

        yield return new WaitForSeconds(0.5f);

        plushDoll.WashSkin();

        yield return new WaitForSeconds(1.5f);

        resultParticle.Find("Washing").GetComponentInChildren<ParticleSystem>().Play();
        plushDoll.ResultAnimation();

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(Result());
    }

    IEnumerator Decoration()
    {
        textAreaText.text = "CHOOSE ITEM";

        yield return new WaitForSeconds(0.5f);

        selectButton.transform.GetComponent<CanvasGroup>().blocksRaycasts = true;

        yield return new WaitWhile(() => !isDecoratComplete);

        resultParticle.Find("Decoration").GetComponentInChildren<MultipleParticles>().Play();
        plushDoll.ResultAnimation();

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(Result());
    }

    IEnumerator Packing()
    {
        textAreaText.text = "HOLD TO STUFF";

        yield return new WaitForSeconds(0.5f);

        TouchActivate();

        yield return new WaitWhile(() => !isPackingComplete);

        TouchDeActivate();
        yield return new WaitForSeconds(1.0f);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(plushDoll.transform.DOMoveX(0.5f, 0.5f));
        sequence.Append(plushDoll.transform.DORotate(new Vector3(0, 180f, 0f), 0.5f));
        sequence.Join(plushDoll.transform.DOMove(new Vector3(plushDoll.defaultPosition.x, plushDoll.defaultPosition.y, -2.0f),0.5f));

        yield return new WaitForSeconds(0.5f);

        resultParticle.Find("Packing").GetComponentInChildren<MultipleParticles>().Play();
        plushDoll.ResultAnimation();

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(Result());
    }

    IEnumerator Result()
    {
        yield return new WaitForSeconds(0.5f);

        plushDoll.gameObject.tag = "Respawn";
        DontDestroyOnLoad(plushDoll.gameObject);
        DataManager.Instance.HomeType = "Result";
        SceneManager.Instance.ChangeScene("Home");
    }

    void OnSewingComplete()
    {
        isSewingComplete = true;
        touchEventHandler.OnTouchKeepListener -= TouchKeep;
    }

    void SewingCompleteAnimation()
    {
        needle.Tighten();
    }

    void WashAnimation()
    {
        water.transform.GetComponentInChildren<ParticleSystem>().Play();
        water.transform.GetComponentInChildren<Rigidbody>().useGravity = true;
    }

    void CreateAccessory(int index)
    {
        if (textArea.alpha == 1.0f) textArea.alpha = 0;
        selectButton.transform.GetComponent<CanvasGroup>().alpha = 0;
        selectButton.transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
        Transform parent;
        GameObject data;
        GameObject obj;

        switch (gameType)
        {
            case GameType.Wappen:
                parent = plushDoll.GetWappenPoint();
                data = decoretPatternData.Objects[index];
                obj = Instantiate(data, decoratButtons[index].transform.position, data.transform.rotation, parent);
                obj.transform.DOLocalMove(new Vector3(0.0f, 0.0f, 0.0f), 1.0f).OnComplete(() => isSelectWappen = true);
                Instantiate(smoke, decoratButtons[index].transform.position.SetZ(decoratButtons[index].transform.position.z - 0.5f), Quaternion.identity);
                break;
            case GameType.Decoration:
                parent = plushDoll.GetAccessoryPoint(decoretPatternData.AccessoryPosition);
                data = decoretPatternData.Objects[index];
                obj = Instantiate(data, decoratButtons[index].transform.position, parent.rotation, parent);
                obj.transform.DOLocalMove(data.transform.position, 1.0f).OnComplete(() => isDecoratComplete = true);
                Instantiate(smoke, decoratButtons[index].transform.position.SetZ(decoratButtons[index].transform.position.z - 0.5f), Quaternion.identity);
                break;
        }
        
    }

    void ProgressBarUpdate(float value)
    {
        slider.value = value;
        if (slider.value >= 1.0f)
        {
            switch (gameType)
            {

                case GameType.Washing:
                    isWashingComplete = true;
                    break;

                case GameType.Packing:
                    progressBar.transform.GetComponent<CanvasGroup>().alpha = 0;
                    isPackingComplete = true;
                    break;
            }
        }
    }

    void TouchActivate()
    {
        touchEventHandler.OnTouchKeepListener += TouchKeep;
    }

    void TouchDeActivate()
    {
        touchEventHandler.OnTouchKeepListener -= TouchKeep;
    }

    void TouchKeep(Vector3 pos)
    {
        if (textArea.alpha == 1.0f) textArea.alpha = 0;
        switch (gameType)
        {
            case GameType.Sewing:
            case GameType.Wappen:
                needle.Move();
                break;
            case GameType.Washing:
                Ray ray = Camera.main.ScreenPointToRay(pos);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100f, layerMask))
                {
                    if (hitIndex.Find(n => n <= hit.triangleIndex + 1 && n >= hit.triangleIndex - 1) >= 1) return;
                    hit.collider.gameObject.transform.GetComponent<DollCollider>().Hit();
                    Instantiate(bubble, hit.point, Quaternion.identity).transform.DOScale(Random.Range(1.0f, 3.0f), 0.3f);
                    hitIndex.Add(hit.triangleIndex);
                }
                break;
            case GameType.Decoration:
                break;
            case GameType.Packing:
                plushDoll.Packing(Time.deltaTime);
                break;
        }
    }

    void MoveNeedle()
    {
        //if (rift.localScale.x <= 0f) return;
        //if (stitchIndex >= stitches.Length)
        //{
        //    rift.localScale = rift.localScale.AddX(-0.01f);
        //    return;
        //};
    }
}
