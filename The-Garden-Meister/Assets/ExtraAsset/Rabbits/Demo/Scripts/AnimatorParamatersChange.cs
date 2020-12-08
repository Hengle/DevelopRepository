using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiveRabbitsDemo
{
    public class AnimatorParamatersChange : MonoBehaviour
    {

        private string[] animatione = new string[] { "Idle", "Run", "Dead" };

        private Animator myAnimator;

        // Use this for initialization
        void Start()
        {

            myAnimator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnGUI()
        {
            GUI.BeginGroup(new Rect(0, 0, 150, 1000));

            for (int i = 0; i < animatione.Length; i++)
            {
                if (GUILayout.Button(animatione[i], GUILayout.Width(150)))
                {
                    myAnimator.SetInteger("AnimIndex", i);
                    myAnimator.SetTrigger("Next");
                }
            }

            GUI.EndGroup();
        }
    }
}
