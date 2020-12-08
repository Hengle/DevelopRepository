using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class PlayerController : MonoBehaviour {

    [SerializeField] AudioClip slashSoundEffect;
    [SerializeField] int attackPower = 1; // 攻撃力
    public bool isActiveMode;                       // プレイヤー操作は有効か？
    public bool strike;                            // 発射するかどうかのbool値
    public bool isMoving;
    public bool isEnableShoot;
    public bool isGameEnd; // ゲーム終了していたら当たり判定を行わない
    float pushPower;
    public Rigidbody2D myrb;                        // 自身のRigidbody2D 参照
    private Vector2 startpos, nowpos, df;           // 発射用の入力値とその結果を保管する変数
    private float x, y, radian, touchDis, sizey;    // 矢印調整用の変数
    public GameObject allowParent, allow;
    GameDirector GameDirector;

    public event Action OnPlayerShootListener; // プレイヤーが発射したイベント通知
    public event Action OnPlayerStopListener; // プレイヤーが静止したイベント通知
    public event Action OnPlayerDeathListener; // プレイヤーが静止したイベント通知
    public event Action<Vector3> OnPlayerTouchWallListener; // 壁にぶつかったイベント通知

    // Start is called before the first frame update
    void Start() {

        strike = false; // 発射フラグ初期化

    }

    // Update is called once per frame
    void Update() {

        if (isActiveMode) {

            if (Input.GetMouseButtonDown(0)) {
                // Rayを発射！
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

                // Objectが無い or あったとしてもUIオブジェクトではなければ画面クリックとして認識
                if (hit2d && hit2d.transform.gameObject.tag == "IngameUI") {
                    Debug.Log("hoge");
                    return;
                }

                startpos = Input.mousePosition;
                isEnableShoot = true;

            }

            if (Input.GetMouseButton(0) && isEnableShoot) {
                nowpos = Input.mousePosition;
            }

            df = nowpos - startpos;
            x = (df.x / Screen.width) * 1f;
            y = (df.y / Screen.height) * -1f;
            radian = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
            touchDis = Vector2.Distance(startpos, nowpos);

            allowParent.transform.position = transform.position; // 矢印の位置を同期

            if (touchDis > 50) {
                //方向の回転
                allowParent.transform.rotation = Quaternion.Euler(0, 0, radian);
                //矢印のサイズ変更
                sizey = touchDis * 0.01f;
                //矢印の伸縮限度
                if (sizey < 0.6f) {
                    sizey = 0.6f;
                }
                if (sizey > 3.6f) {
                    sizey = 3.6f;
                }
                allow.transform.localScale = new Vector3(0.6f, sizey, 1.1f);
            }
            //離した瞬間に力を加える
            if (Input.GetMouseButtonUp(0) && isEnableShoot) {
                // 2点の距離が近すぎる場合は戻す
                if (touchDis < 50) { return; }

                strike = true;
                isMoving = true;
            }


            //GetMousePosition();
            //ArrowControll();
            Strike();
        }

        CheckPlayerSpeed();
    }

    /// <summary>
    /// マウスの入力を取得
    /// </summary>
    void GetMousePosition() {

        if (Input.GetMouseButtonDown(0)) {

            startpos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0)) {
            nowpos = Input.mousePosition;
        }
        df = nowpos - startpos;
        x = (df.x / Screen.width) * 1f;
        y = (df.y / Screen.height) * -1f;
        radian = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
        touchDis = Vector2.Distance(startpos, nowpos);
    }

    /// <summary>
    /// 発射のために矢印の大きさと回転を管理
    /// </summary>
    void ArrowControll() {

        allowParent.transform.position = transform.position; // 矢印の位置を同期

        if (touchDis > 50) {
            //方向の回転
            allowParent.transform.rotation = Quaternion.Euler(0, 0, radian);
            //矢印のサイズ変更
            sizey = touchDis * 0.01f;
            //矢印の伸縮限度
            if (sizey < 0.6f) {
                sizey = 0.6f;
            }
            if (sizey > 3.6f) {
                sizey = 3.6f;
            }
            allow.transform.localScale = new Vector3(0.6f, sizey, 1.1f);
        }
        //離した瞬間に力を加える
        if (Input.GetMouseButtonUp(0)) {
            strike = true;
            isMoving = true;
        }
    }

    /// <summary>
    /// 入力中の値に従ってプレイヤーを発射する
    /// </summary>
    void Strike() {


        //arrowを基準に飛ばす。
        if (strike == true) {


            strike = false;
            myrb.AddForce(allowParent.transform.up * (sizey * 5), ForceMode2D.Impulse);
            //Debug.Log(allowParent.transform.up * (sizey * 5));

            SetIsActive(false);//発射可能モードをオフ
            allowParent.SetActive(false);
            startpos = Vector3.zero;
            nowpos = Vector3.zero;

            allow.transform.localScale = Vector3.one;
            OnPlayerShootListener?.Invoke(); // 発射したイベントを通知
            isEnableShoot = false;
        }
    }

    void CheckPlayerSpeed() {

        myrb.velocity *= 0.98f; // 発射済みなら減速処理

        if (isMoving) {

            if (Mathf.Abs(myrb.velocity.x) <= 0.4f && Mathf.Abs(myrb.velocity.y) <= 0.4f) {
                myrb.velocity = Vector2.zero;
                isMoving = false;
                OnPlayerStopListener?.Invoke();
            }

        }


    }


    private void OnTriggerEnter2D(Collider2D collision) {
        var target = collision.gameObject;

        // ゲーム終了していたら当たり判定を行わない
        if (isGameEnd) { return; }

        if (target.tag == "Enemy") {

            if (DataManager.GetPlayerSettingVibration()) {
                IOSUtil.PlaySystemSound(1519);
                AndroidUtil.Vibrate(100);
            }

            var isBoss = target.GetComponent<Enemy>().GetIsBoss();
            if (DataManager.GetPlayerSettingSound() && !isBoss) {
                Debug.Log("Hit Sound");
                GetComponent<AudioSource>().PlayOneShot(slashSoundEffect);
            }

            target.GetComponent<Enemy>()?.Damage(attackPower);
        } else if (target.tag == "Friendly") {
            target.GetComponent<Friendly>()?.Damage();
            if (DataManager.GetPlayerSettingSound()) {
                Debug.Log("Hit Sound");
                GetComponent<AudioSource>().PlayOneShot(slashSoundEffect);
            }
        } else if (target.tag == "SpeedChange") {
            target.GetComponent<SpeedChange>().ChangeEnable(false);
            var power = target.GetComponent<SpeedChange>().GetSpeedChangeVal();

            myrb.velocity *= power;
        } else if (target.tag == "DeathBlock") {
            OnPlayerDeathListener?.Invoke();
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
            GetComponent<CircleCollider2D>().enabled = false;
            myrb.velocity = Vector3.zero;
        } else if (target.tag == "MovePanel") {
            target.GetComponent<MovePanel>().ChangeEnable(false);
            // プレイヤーをパネル中央に移動
            //gameObject.transform.DOMove(target.transform.position,0f);
            var moveVec = target.GetComponent<MovePanel>().GetMoveVector(myrb.velocity);
            myrb.velocity = moveVec;
            //myrb.AddForce(moveVec, ForceMode2D.Impulse);

        }else if (target.tag == "Switch") {
            target.GetComponent<SwitchPanel>().ChangeStatus();
        } else if (target.tag == "Bomb") {

            target.GetComponent<Bomb>().Explode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        var target = collision.gameObject;

        // ゲーム終了していたら当たり判定を行わない
        if (isGameEnd) { return; }


        if (target.tag == "Wall") {

            Vector3 hitPos = Vector3.zero;
            foreach (ContactPoint2D point in collision.contacts) {
                hitPos = point.point;
            }
            OnPlayerTouchWallListener?.Invoke(hitPos);

        }else if(target.tag == "Shuriken") {

            target.GetComponent<Rigidbody2D>().isKinematic = false;
            target.GetComponent<Rigidbody2D>().AddForce(myrb.velocity * 2 * -1, ForceMode2D.Impulse);

            //Vector3 hitPos = Vector3.zero;
            //foreach (ContactPoint2D point in collision.contacts) {
            //    hitPos = point.point;
            //}
            //var pushPos = hitPos - transform.position;
            //pushPos.Normalize(); // 押し返す方向

            ////加速度の大きい方を取る

            //myrb.AddForce(pushPos * 4 *-1f, ForceMode2D.Impulse);

        }
    }

    public void SetGameEnd(bool newVal) {
        isGameEnd = newVal;
    }

    public void SetDirectorEvent(GameDirector gameDirector) {
        GameDirector = gameDirector;
        GameDirector.OnEnablePlayerShootMode += flag => SetIsActive(flag);
    }

    public void SetIsActive(bool flag) {
        isActiveMode = flag;
        if (flag) { allowParent.SetActive(true); }
    }
}