using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    [SerializeField] Hand currentHand; // 현재 장착된 Hand형 타입 무기
    bool isAttack; // 공격중?
    bool isSwing; // 팔 휘두르는 중?
    RaycastHit hitInfo;

    void Start()
    {
        InputManager.Subscribe("Fire", TryAttack);        
    }

    void OnDestroy()
    {
        InputManager.Unsubscribe("Fire", TryAttack);        
    }

    void TryAttack(InputAction.CallbackContext context)
    {
        if(!isAttack)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttack = true;
        currentHand.anim.SetTrigger("Attack");

        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);
        isAttack = false;
    }

    IEnumerator HitCoroutine()
    {
        while(isSwing)
        {
            if(CheckObject())
            {
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    bool CheckObject()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range))
        {
            return true;
        }
        return false;
    }
}
