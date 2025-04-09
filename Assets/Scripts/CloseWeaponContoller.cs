using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public abstract class CloseWeaponContoller : MonoBehaviour
{
    [SerializeField] protected CloseWeapon currentCloseWeapon; // 현재 장착된 Hand형 타입 무기
    // 필요한 컴포넌트
    protected PlayerController playerController; 
    protected bool isAttack; // 공격중?
    protected bool isSwing; // 팔 휘두르는 중?
    protected RaycastHit hitInfo;

    void Awake()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }
    protected virtual void OnEnable()
    {
        InputManager.Subscribe("Fire", TryAttack);        
    }

    protected virtual void OnDisable()
    {
        InputManager.Unsubscribe("Fire", TryAttack);        
    }

    protected abstract void TryAttack(InputAction.CallbackContext context);
    protected IEnumerator AttackCoroutine(string swingType, float delayA, float delayB, float delayC)
    {
        isAttack = true;
        currentCloseWeapon.anim.SetTrigger(swingType);

        yield return new WaitForSeconds(delayA);
        isSwing = true;

        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(delayB);
        isSwing = false;

        yield return new WaitForSeconds(delayC - delayB - delayA);
        isAttack = false;
    }
    protected abstract IEnumerator HitCoroutine();

    protected bool CheckObject()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range))
        {
            return true;
        }
        return false;
    }
    public virtual void CloseWeaponChange(CloseWeapon hand)
    {
        if(WeaponManager.currentWeapon != null)
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }
        currentCloseWeapon = hand;
        WeaponManager.currentWeapon = currentCloseWeapon.transform;
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;

        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true);
    }

}
