using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AxeController : CloseWeaponContoller
{
    public static bool isActivate = false; // 활성화 여부
    protected override void TryAttack(InputAction.CallbackContext context)
    {
        if(!isAttack && isActivate) // 활성화 상태에서만 공격 가능
        {
            StartCoroutine(AttackCoroutine());
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
