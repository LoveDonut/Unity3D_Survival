using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PickaxeController : CloseWeaponContoller
{
    public static bool isActivate = true; // 활성화 여부
    void Start()
    {
        WeaponManager.currentWeapon = currentCloseWeapon.transform;
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;        
    }
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
