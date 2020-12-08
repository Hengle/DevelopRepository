using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SDKManager;

public class ListViewElement : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] Image disableImage;
    [SerializeField] Image activeImage;
    [SerializeField] Text priceText;

    WeaponData weaponData;
    Button button;

    public event Action OnChangeActiveWeaponListener;
    public event Action OnPurchaseListener;

    void Awake()
    {
        button = transform.GetComponent<Button>();
    }

    public void Initialize(WeaponData data)
    {
        weaponData = data;

        iconImage.sprite = weaponData.Icon;
        priceText.text = weaponData.Price + "＄";
        button.onClick.AddListener(() => OnClickElement());
        Refresh();
    }

    public void Refresh()
    {
        var isPossessed = DataManager.Instance.IsPossessedWeapon(weaponData.Id);
        var isActive = DataManager.Instance.IsActiveWeapon(weaponData.Id);
        if (isPossessed)
        {
            priceText.enabled = false;
            disableImage.enabled = false;
        }

        if (isActive)
        {
            activeImage.enabled = true;
        }
        else
        {
            activeImage.enabled = false;
        }
    }

    public void OnClickElement()
    {
        AudioManager.Instance.PlaySE("window_open");
        //所持していなかったら購入
        var id = weaponData.Id;
        var isPossessed = DataManager.Instance.IsPossessedWeapon(id);

        //所持していたらセット、していなかったら購入
        if (!isPossessed)
        {
            bool isConsumeSuccess = DataManager.Instance.ConsumeCoin(weaponData.Price);
            if (!isConsumeSuccess) return;
            FacebookManager.Instance.FacebookSkinUnlockEvent(id);
            DataManager.Instance.PurchaseWeapon(id);
            OnPurchaseListener?.Invoke();
        }

        if (DataManager.Instance.IsActiveWeapon(id)) return;
        DataManager.Instance.SetActiveWeapon(id);
        OnChangeActiveWeaponListener?.Invoke();

        if (DataManager.Instance.WeaponTutorial)
        {
            DataManager.Instance.ClearWeaponTutorial();
        }

        DataManager.Instance.Save();
    }
}
