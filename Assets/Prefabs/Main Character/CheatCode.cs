using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class CheatCode : MonoBehaviour
    {
        public static CheatCode INSTANCE;
        private KeyCode[] cheatCode = {
        KeyCode.UpArrow,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.B,
        KeyCode.A,
        KeyCode.Return
    };
        private int current = 0;
        private float duration = 5f;
        private float lastInputted = 0f;
        public bool unlocked = false;

        private void Awake()
        {
            if (INSTANCE)
            {
                Destroy(INSTANCE);
            }

            INSTANCE = this;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(cheatCode[current]))
            {
                // Debug.Log("Correct " + current);
                lastInputted = Time.time + duration;
                if (++current == cheatCode.Length)
                {
                    if (!unlocked)
                    {
                        FeedbackStatusBehavior.INSTANCE.SetFeedback("Cheat Code Active!");
                        current = 0;
                        unlocked = true;
                    }
                    else
                    {
                        FeedbackStatusBehavior.INSTANCE.SetFeedback("Cheat Code Not Active!");
                        unlocked = false;
                        current = 0;
                    }
                }
            }
            else if (Time.time > lastInputted)
            {
                // Debug.Log("Expired");
                current = 0;
            }
        }
    }
}
