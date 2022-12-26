using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class OnCollideTrigger : MonoBehaviour
    {
        [SerializeField]
        public string message;

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                FeedbackStatusBehavior.INSTANCE.SetFeedback(message);
            }
        }
    }
}
