using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PS
{
    public class DialogueCharacter : MonoBehaviour
    {
        public bool hasVisited = false;
        public bool inCutScene = false;
        public Cinemachine.CinemachineVirtualCamera cutSceneCam;
        public string[] dialogue = { "Hi, put some dialogue here!" };
        public int at = 0;
        private PlayerManager pm;
        private bool pressNextDialogue = false;
        public bool goToNextScene = false;

        private void Start()
        {
            cutSceneCam.enabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && inCutScene)
            {
                pressNextDialogue = true;
            }
        }

        private void FixedUpdate()
        {
            if (pressNextDialogue)
            {
                pressNextDialogue = false;
                if (at == dialogue.Length)
                {
                    inCutScene = false;
                    InputHandler.INSTANCE.enabled = true;
                    pm.GetComponent<Rigidbody>().isKinematic = false;
                    cutSceneCam.enabled = false;
                    FeedbackStatusBehavior.INSTANCE.SetFeedback(null);
                    at = 0;
                    if (goToNextScene)
                    {
                        Cursor.lockState = CursorLockMode.None;
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    }
                }
                else
                {
                    ShowFeedback();
                }
            }
        }

        private void ShowFeedback()
        {
            FeedbackStatusBehavior.INSTANCE.SetFeedback(dialogue[at++], 100, true);
        }

        private void OnTriggerEnter(Collider other)
        {
            pm = other.GetComponent<PlayerManager>();
            if (!pm) return;
            if ((!hasVisited))
            {
                InputHandler.INSTANCE.enabled = false;
                pm.GetComponent<Rigidbody>().isKinematic = true;
                cutSceneCam.enabled = true;
                hasVisited = true;
                inCutScene = true;
                at = 0;
                ShowFeedback();
            }
        }
    }
}
