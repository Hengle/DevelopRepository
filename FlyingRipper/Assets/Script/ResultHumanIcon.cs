using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultHumanIcon : MonoBehaviour {

    [SerializeField] Color enemyColor;
    [SerializeField] Color bossColor;
    [SerializeField] Color friendColor;

    public enum Type {
        ENEMY,
        BOSS,
        FRIEND
    }



    public void SetColor(Type iconType) {
        switch (iconType) {
            case Type.ENEMY:
                GetComponent<Image>().color = enemyColor;
                break;
            case Type.BOSS:
                GetComponent<Image>().color = bossColor;
                break;
            case Type.FRIEND:
                GetComponent<Image>().color = friendColor;
                break;
        }
    }


}
