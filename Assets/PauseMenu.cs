using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PS
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu INSTANCE;
        public TMPro.TextMeshProUGUI YouWin;
        public TMPro.TextMeshProUGUI YouLose;
        public GameObject button;
        public GameObject continuePlaying;
        public bool gameOver = false;
        public bool didWin = false;

        private void Awake()
        {
            if (INSTANCE)
            {
                Destroy(INSTANCE);
            }

            INSTANCE = this;
        }

        private void Start()
        {
            button.SetActive(false);
            continuePlaying.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
            {
                if (!button.activeSelf)
                {
                    Cursor.lockState = CursorLockMode.None;
                    InputHandler.INSTANCE.enabled = false;
                    Time.timeScale = 0;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    InputHandler.INSTANCE.enabled = true;
                    Time.timeScale = 1;
                }
                button.SetActive(!button.activeSelf);
            }
            else if (gameOver)
            {
                Cursor.lockState = CursorLockMode.None;
                InputHandler.INSTANCE.enabled = false;
                // Time.timeScale = 0;
            }
        }

        public bool IsPaused()
        {
            return button.activeInHierarchy || continuePlaying.activeInHierarchy;
        }

        public void Win()
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Destroy(obj);
            }
            gameOver = true;
            YouWin.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            InputHandler.INSTANCE.enabled = false;
            CameraHandler.INSTANCE.enabled = false;
            // Time.timeScale = 0;
            // button.SetActive(true);
            didWin = true;
            continuePlaying.SetActive(true);
        }

        public void Lose()
        {
            gameOver = true;
            YouLose.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            InputHandler.INSTANCE.enabled = false;
            CameraHandler.INSTANCE.enabled = false;
            // Time.timeScale = 0;
            continuePlaying.SetActive(true);
        }

        public void ContinuePlaying()
        {
            Cursor.lockState = CursorLockMode.Locked;
            InputHandler.INSTANCE.enabled = true;
            Time.timeScale = 1;
            continuePlaying.SetActive(false);
            if (didWin)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                QuitToMenu();
            }
        }

        public void QuitToMenu()
        {
            if (!gameOver)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                // credits
                SceneManager.LoadScene(4);
            }
        }
    }
}
