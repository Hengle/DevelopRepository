using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopWindow : MonoBehaviour
{
    [SerializeField] Button openButton;
    [SerializeField] Button backButton;
    [SerializeField] CanvasGroup myCanvasGroup;
    [SerializeField] WeaponMaster weaponMaster;
    [SerializeField] GameObject listViewElement;
    [SerializeField] Transform listViewElementParent;
    [SerializeField] GameObject demoCharacterHand;

    List<ListViewElement> elements = new List<ListViewElement>();
    public event Action OnChangeActiveWeaponListener;

    void Awake()
    {
        Back();
        Initialize();
    }

    void Initialize()
    {
        var weaponList = weaponMaster.GetWeaponList();
        foreach (var weaponData in weaponList)
        {
            var element = Instantiate(listViewElement, listViewElementParent).GetComponent<ListViewElement>();
            element.Initialize(weaponData);
            element.OnChangeActiveWeaponListener += ChangeWepon;
            element.OnPurchaseListener += RefreshList;
            elements.Add(element);
        }
        Instantiate(weaponMaster.GetWeaponData(DataManager.Instance.WeaponId).Prefab, demoCharacterHand.transform).layer = 12;
    }

    void ChangeWepon()
    {
        foreach (Transform t in demoCharacterHand.transform)
        {
            if (t.gameObject.tag == "Weapon")
            {
                Destroy(t.gameObject);
            }
        }
        Instantiate(weaponMaster.GetWeaponData(DataManager.Instance.WeaponId).Prefab, demoCharacterHand.transform).layer = 12;
        RefreshList();
    }

    void RefreshList()
    {
        foreach(var element in elements)
        {
            element.Refresh();
        }
    }

    public void Open()
    {
        AudioManager.Instance.PlaySE("window_open");
        myCanvasGroup.alpha = 1;
        myCanvasGroup.interactable = true;
        myCanvasGroup.blocksRaycasts = true;
    }

    public void Back()
    {
        AudioManager.Instance.PlaySE("window_close");
        myCanvasGroup.alpha = 0;
        myCanvasGroup.interactable = false;
        myCanvasGroup.blocksRaycasts = false;
    }


}
