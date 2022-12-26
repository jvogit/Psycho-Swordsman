using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PS
{
    public class FeedbackStatusBehavior : MonoBehaviour
    {
        public static FeedbackStatusBehavior INSTANCE;
        public int defaultDuration = 5;
        [SerializeField]
        private TMPro.TextMeshProUGUI feedbackText;
        [SerializeField]
        private TMPro.TextMeshProUGUI continuePromptText;

        private float showFeedbackUntil = 0;

        // Start is called before the first frame update
        void Awake()
        {
            if (INSTANCE)
            {
                Destroy(INSTANCE);
            }

            INSTANCE = this;
        }

        private void Start()
        {
            feedbackText.enabled = false;
            continuePromptText.enabled = false;
        }

        private void Update()
        {
            if (Time.time > showFeedbackUntil)
            {
                feedbackText.enabled = false;
                continuePromptText.enabled = false;
            }
        }

        public void SetFeedback(string feedback)
        {
            SetFeedback(feedback, defaultDuration);
        }

        public void SetFeedback(string feedback, float duration, bool showPrompt = false)
        {
            continuePromptText.enabled = showPrompt;
            if (feedback == null || feedback.Length == 0)
            {
                feedbackText.enabled = false;
                return;
            }

            showFeedbackUntil = Time.time + duration;
            feedbackText.enabled = true;
            feedbackText.text = feedback;
            continuePromptText.enabled = showPrompt;
        }
    }
}
