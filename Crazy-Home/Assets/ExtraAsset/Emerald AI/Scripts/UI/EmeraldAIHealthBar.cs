using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EmeraldAI.Utility
{
    public class EmeraldAIHealthBar : MonoBehaviour
    {
        public static string CameraTag = "MainCamera";
        Image HealthBar;
        [HideInInspector]
        public EmeraldAISystem EmeraldComponent;
        float AICurrentHealth;
        float AIMaxHealth;
        public static Camera m_Camera;
        float ObjectScaleDifference;
        [HideInInspector]
        public Canvas canvas;
        [HideInInspector]
        public float MaxScalingSize = 2f;
        CanvasGroup CG;
        TextMesh AIName;
        TextMesh AILevel;

        void Start()
        {
            if (m_Camera == null)
            {
                m_Camera = GameObject.FindGameObjectWithTag(CameraTag).GetComponent<Camera>();
            }

            InvokeRepeating("UpdateUI", 0f, 0.1f);
        }

        void UpdateUI()
        {
            if (HealthBar != null
                || HealthBar != null && EmeraldComponent.BehaviorRef == EmeraldAISystem.CurrentBehavior.Companion
                || HealthBar != null && EmeraldComponent.BehaviorRef == EmeraldAISystem.CurrentBehavior.Pet)
            {
                AICurrentHealth = (float)EmeraldComponent.CurrentHealth;
                HealthBar.fillAmount = (AICurrentHealth / AIMaxHealth);
            }
            else
            {
                CG = GetComponent<CanvasGroup>();
                AIMaxHealth = (float)EmeraldComponent.StartingHealth;
                HealthBar = transform.Find("AI Health Bar Background/AI Health Bar").GetComponent<Image>();
                AIName = transform.Find("AI Name Text").GetComponent<TextMesh>();
                AILevel = transform.Find("AI Level Text").GetComponent<TextMesh>();
            }
        }

        void Update()
        {
            CalculateUI();
        }

        public void CalculateUI()
        {
            if (m_Camera != null)
            {
                if (HealthBar != null
                    || HealthBar != null && EmeraldComponent.BehaviorRef == EmeraldAISystem.CurrentBehavior.Companion
                    || HealthBar != null && EmeraldComponent.BehaviorRef == EmeraldAISystem.CurrentBehavior.Pet
                    || HealthBar != null && EmeraldComponent.BehaviorRef == EmeraldAISystem.CurrentBehavior.Passive && EmeraldComponent.PassiveTargetRef != null)
                {
                    canvas.transform.parent.LookAt(canvas.transform.parent.position + m_Camera.transform.rotation * Vector3.forward,
                    m_Camera.transform.rotation * Vector3.up);

                    float dist = Vector3.Distance(m_Camera.transform.position, transform.position);
                    if (dist < 40)
                    {
                        canvas.transform.localScale = new Vector3(dist * 0.085f, dist * 0.085f, dist * 0.085f);
                    }
                    else
                    {
                        canvas.transform.localScale = new Vector3(40 * 0.085f, 40 * 0.085f, 40 * 0.085f);
                    }
                }
            }
        }

        public void FadeOut()
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(FadeTo(0.0f, 1.5f));
            }
        }

        void OnDisable()
        {
            if (CG != null)
            {
                Color newColor1 = new Color(1, 1, 1, 1);
                CG.alpha = newColor1.a;
                AIName.color = new Color(AIName.color.r, AIName.color.g, AIName.color.b, newColor1.a);
                AILevel.color = new Color(AILevel.color.r, AILevel.color.g, AILevel.color.b, newColor1.a);
            }
        }

        IEnumerator FadeTo(float aValue, float aTime)
        {
            AICurrentHealth = (float)EmeraldComponent.CurrentHealth;
            HealthBar.fillAmount = (AICurrentHealth / AIMaxHealth);

            float alpha = CG.alpha;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                Color newColor1 = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
                CG.alpha = newColor1.a;
                AIName.color = new Color(AIName.color.r, AIName.color.g, AIName.color.b, newColor1.a);
                AILevel.color = new Color(AILevel.color.r, AILevel.color.g, AILevel.color.b, newColor1.a);

                if (CG.alpha <= 0.08f)
                {
                    gameObject.SetActive(false);
                }

                yield return null;
            }
        }
    }
}