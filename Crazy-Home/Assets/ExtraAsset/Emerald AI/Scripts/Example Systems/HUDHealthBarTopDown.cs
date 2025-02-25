﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EmeraldAI.Example
{
    public class HUDHealthBarTopDown : MonoBehaviour
    {
        public string AITag = "Respawn";
        public int DetectDistance = 40;
        public int DeactiveSeconds = 3;

        private GameObject HUDObject;
        private Text AINameText;
        private Image AIHealthBar;
        private RaycastHit hit;
        private Coroutine DeactiveHUDRef;
        private bool DeactivatedInitialized;
        private EmeraldAISystem EmeraldComponent;
        private float DeactiveTimer;

        private void Start()
        {
            HUDObject = Instantiate(Resources.Load("HUD Canvas") as GameObject, Vector3.zero, Quaternion.identity);
            AINameText = GameObject.Find("HUD - AI Name").GetComponent<Text>();
            AIHealthBar = GameObject.Find("HUD - AI Health Bar").GetComponent<Image>();
            HUDObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (HUDObject.activeSelf)
            {
                DeactiveTimer += Time.deltaTime;

                if (DeactiveTimer >= DeactiveSeconds)
                {
                    EmeraldComponent = null;
                    HUDObject.SetActive(false);
                    DeactiveTimer = 0;
                }
            }

            if (EmeraldComponent != null)
            {
                HUDObject.SetActive(true);
                AINameText.text = EmeraldComponent.AIName;
                AIHealthBar.fillAmount = (float)EmeraldComponent.CurrentHealth / EmeraldComponent.StartingHealth;

                if (AIHealthBar.fillAmount <= 0)
                {
                    if (HUDObject.activeSelf)
                    {
                        EmeraldComponent = null;
                    }
                }
            }
        }
    }
}