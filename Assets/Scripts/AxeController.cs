using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class AxeController : CloseWeaponContoller
{
    public static bool isActivate = false; // 활성화 여부
    protected override void TryAttack(InputAction.CallbackContext context)
    {
        if(!isAttack && isActivate) // 활성화 상태에서만 공격 가능
        {
            if(CheckObject() && hitInfo.transform.CompareTag("Tree"))
            {
                StartCoroutine(AttackCoroutine("Chop",currentCloseWeapon.workDelayA, currentCloseWeapon.workDelayB, currentCloseWeapon.workDelay));                
            }
            else
            {
                StartCoroutine(AttackCoroutine("Attack",currentCloseWeapon.attackDelayA, currentCloseWeapon.attackDelayB, currentCloseWeapon.attackDelay));
            }
        }
    }
    protected override IEnumerator HitCoroutine()
    {
        while(isSwing)
        {
            if(CheckObject())
            {
                if(hitInfo.transform.CompareTag("Grass"))
                {
                    hitInfo.transform.GetComponent<Grass>().Damage();
                }
                else if(hitInfo.transform.CompareTag("Tree"))
                {
                    hitInfo.transform.GetComponent<TreeComponent>().Chop(hitInfo.point, transform.eulerAngles.y);
                }
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
