using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class AnimatorHandler : MonoBehaviour
    {
        public CharacterManager cm;
        public Animator anim;

        private int vertical;
        private int horizontal;

        protected virtual void Start()
        {
            cm = GetComponentInParent<CharacterManager>();
            anim = GetComponent<Animator>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting = true, bool canRotate = false)
        {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("canRotate", canRotate);
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f);
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
        {
            #region Vertical
            float v = 0;

            /* if (verticalMovement > 0 && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if (verticalMovement > 0.55f)
            {
                v = 1f;
            }
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (verticalMovement < -0.55f)
            {
                v = -1f;
            }
            else
            {
                v = 0;
            }*/
            #endregion Vertical

            #region Horizontal
            float h = 0;

            /* if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                h = 1f;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                h = -1f;
            }
            else
            {
                h = 0;
            } */
            #endregion

            anim.SetFloat(vertical, verticalMovement, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, horizontalMovement, 0.1f, Time.deltaTime);
        }

        public void CanRotate()
        {
            anim.SetBool("canRotate", true);
        }

        public void StopRotate()
        {
            anim.SetBool("canRotate", false);
        }

        public void EnableCombo()
        {
            anim.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            anim.SetBool("canDoCombo", false);
        }

        public void EnableInvulnerable()
        {
            anim.SetBool("isInvulnerable", true);
        }

        public void DisableInvulnerable()
        {
            anim.SetBool("isInvulnerable", false);
        }

        public void EnableParrying()
        {
            anim.SetBool("isParrying", true);
        }

        public void DisableParrying()
        {
            anim.SetBool("isParrying", false);
        }

        public void EnableParried()
        {
            anim.SetBool("isParried", true);
        }

        public void DisableParried()
        {
            anim.SetBool("isParried", false);
        }

        public void Trigger_OnQuickAttack()
        {
            cm.Trigger_OnQuickAttack();
        }

        public void Trigger_QuickAttack3()
        {
            cm.Trigger_OnQuickAttack3();
        }

        public void Trigger_ChargeAttack()
        {
            cm.Trigger_OnChargeAttack();
        }
    }
}
