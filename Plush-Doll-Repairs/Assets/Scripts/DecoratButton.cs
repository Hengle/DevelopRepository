using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecoratButton : MonoBehaviour
{
    [SerializeField] int index;
    Image image;
    public event Action<int> OnClickListener;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Initialize(DecoretPatternData decoretPattern)
    {
        image.sprite = decoretPattern.Sprites[index];
    }

    public void OnClickButton()
    {
        OnClickListener?.Invoke(index);
    }
}
