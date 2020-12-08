using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    [SerializeField] GameObject lifePrehab;
    private int nowLife;
    private List<GameObject> lifes = new List<GameObject>();

    public void Initialize(int life)
    {
        nowLife = life;
        lifes.Clear();
        for (int i = 0; i < life; i++)
        {
            var position = lifePrehab.transform.position;
            var width = lifePrehab.GetComponent<RectTransform>().sizeDelta.x;
            position.x = (life * width / -2) + width * i + width / 2;
            lifePrehab.transform.position = position;
            var gameObject = Instantiate(lifePrehab, position, Quaternion.identity);
            gameObject.transform.SetParent(this.transform, false);
            lifes.Add(gameObject);
        }
    }

    public void Reduce()
    {
        nowLife--;
        var life = lifes[nowLife];
        life.GetComponent<Image>().color = new Color(204.0f/255.0f, 204.0f / 255.0f, 125.0f / 204.0f);
    }
}
