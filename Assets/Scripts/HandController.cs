using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : CloseWeaponContoller
{
    public static bool isActivate = true; // 활성화 여부
    
    protected override void TryAttack(InputAction.CallbackContext context)
    {
        if(!isAttack && isActivate) // 활성화 상태에서만 공격 가능
        {
            StartCoroutine(AttackCoroutine("Attack",currentCloseWeapon.attackDelayA, currentCloseWeapon.attackDelayB, currentCloseWeapon.attackDelay));
        }
    }
    protected override IEnumerator HitCoroutine()
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
    public override void CloseWeaponChange(CloseWeapon cloaseWeapon)
    {
        base.CloseWeaponChange(cloaseWeapon);
        isActivate = true;
    }
}
