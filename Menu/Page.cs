﻿using System.Collections;
using UnityEngine;

namespace UnityCore {
    
    namespace Menu {

        public class Page : MonoBehaviour
        {
            public static readonly string FLAG_ON = "On";
            public static readonly string FLAG_OFF = "Off";
            public static readonly string FLAG_NONE = "None";

            public PageType type;
            public bool useAnimation;
            public string targetState {get;private set;}

            /*
             * Animaton Requirements...
             *  - This class uses certain controls to determine page state
             *  - Pages have three core states:
             *      1. Resting
             *      2. Turning On
             *      3. Turning Off
             *  - The animator must have a control boolean called 'on'. Otherwise the animator will not work.
             */
            private Animator m_Animator;

#region Unity Functions
            private void OnEnable() {
                CheckAnimatorIntegrity();
            }
#endregion

#region Public Functions
            /// <summary>
            /// Call this to turn the page on or off by setting the control '_on'
            /// </summary>
            public void Animate(bool _on) {
                if (useAnimation) {
                    m_Animator.SetBool("on", _on);

                    StopCoroutine("AwaitAnimation");
                    StartCoroutine("AwaitAnimation", _on);
                }
            }
#endregion

#region Private Functions
            private IEnumerator AwaitAnimation(bool _on) {
                targetState = _on ? FLAG_ON : FLAG_OFF;

                while (!m_Animator.GetCurrentAnimatorStateInfo(0).IsName(targetState)) {
                    yield return null;
                }
                while (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) {
                    yield return null;
                }

                targetState = FLAG_NONE;

                Log("Page ["+type+"] finished transitioning to "+(_on ? "<color=#0f0>on</color>." : "<color=#f00>off</color>."));

                if (!_on) {
                    gameObject.SetActive(false);
                }
            }

            private void CheckAnimatorIntegrity() {
                if (useAnimation) {
                    // try to get animator
                    m_Animator = GetComponent<Animator>();
                    if (!m_Animator) {
                        LogWarning("You opted to animate page ["+type+"], but no Animator component exists on the object.");
                    }
                }
            }

            private void Log(string _msg) {
                Debug.Log("[Page]: "+_msg);
            }

            private void LogWarning(string _msg) {
                Debug.LogWarning("[Page]: "+_msg);
            }
#endregion
        }
    }
}