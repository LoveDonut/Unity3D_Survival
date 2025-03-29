using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject go_CrosshairHUD; // 크로스 헤어 비활성화를 위한 부모 객체
    float gunAccuracy; // 크로스헤어 상태에 따른 총의 정확도
    public void WalkingAnimation(bool _flag)
    {
        animator.SetBool("Walking", _flag);
    }
    public void RunningAnimation(bool _flag)
    {
        animator.SetBool("Running", _flag);
    }
    public void CrouchingAnimation(bool _flag)
    {
        animator.SetBool("Crouching", _flag);
    }
    public void FireAnimation()
    {
        if(animator.GetBool("Walking"))
        {
            animator.SetTrigger("Walk_Fire");
        }
        else if(animator.GetBool("Crouching"))
        {
            animator.SetTrigger("Crouch_Fire");
        }
        else
        {
            animator.SetTrigger("Idle_Fire");
        }
    }


}
