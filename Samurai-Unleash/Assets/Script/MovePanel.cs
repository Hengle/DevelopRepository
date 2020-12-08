using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePanel : MonoBehaviour
{
    [SerializeField] Sprite[] mySprites;
    [SerializeField] type direction;
    SpriteRenderer myRend;
    BoxCollider2D myCollider;

    enum type { UP,RIGHT,DOWN,LEFT};

    private void Start() {
        myRend = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();


        switch (direction) {
            case type.UP:
                myRend.sprite = mySprites[0];
                break;
            case type.RIGHT:
                myRend.sprite = mySprites[1];
                break;
            case type.DOWN:
                myRend.sprite = mySprites[2];
                break;
            case type.LEFT:
                myRend.sprite = mySprites[3];
                break;
        }
    }

    public Vector2 GetMoveVector(Vector2 playerVelocity) {

        

        Vector2 moveVec= Vector2.zero;
        var speed = Mathf.Abs(playerVelocity.x) >= Mathf.Abs(playerVelocity.y) ? Mathf.Abs(playerVelocity.x) : Mathf.Abs(playerVelocity.y);
        switch (direction) {
            case type.UP:
                playerVelocity.x = 0;
                playerVelocity.y = Mathf.Abs(speed); // 絶対値を取ってプラス化

                break;
            case type.RIGHT:
                playerVelocity.x = Mathf.Abs(speed);
                playerVelocity.y = 0; // 絶対値を取ってプラスにしてマイナスにする
                break;
            case type.DOWN:
                playerVelocity.x = 0;
                playerVelocity.y = Mathf.Abs(speed) * -1; // 絶対値を取ってプラスにしてマイナスにする
                break;
            case type.LEFT:
                playerVelocity.x = Mathf.Abs(speed) * -1;
                playerVelocity.y = 0; // 絶対値を取ってプラスにしてマイナスにする
                break;
        }

        return playerVelocity;
    }


    public void ChangeEnable(bool flag) {
        if (flag) {
            Color32 myColor = myRend.color;
            myColor.a = 255;
            myRend.color = myColor;
            myCollider.enabled = true;
        } else {
            Color32 myColor = myRend.color;
            myColor.a = 100;
            myRend.color = myColor;
            myCollider.enabled = false;

        }
    }
}
