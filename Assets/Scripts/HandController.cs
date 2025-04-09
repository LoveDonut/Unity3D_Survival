using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : CloseWeaponContoller
{
    [SerializeField] QuickSlotController quickSlotController;
    public static bool isActivate = true; // 활성화 여부

    protected override void OnEnable()
    {
        base.OnEnable();
        InputManager.Subscribe("Eat", TryEat);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        InputManager.Unsubscribe("Eat", TryEat);
    }

    protected override void TryAttack(InputAction.CallbackContext context)
    {
        if(Inventory.inventoryActivated) return;
        if(!isAttack && isActivate) // 활성화 상태에서만 공격 가능
        {
            StartCoroutine(AttackCoroutine("Attack",currentCloseWeapon.attackDelayA, currentCloseWeapon.attackDelayB, currentCloseWeapon.attackDelay));
        }
    }
    protected void TryEat(InputAction.CallbackContext ctx)
    {
        if(Inventory.inventoryActivated) return;
        if(!isAttack && isActivate && QuickSlotController.go_HandItem != null) // 활성화 상태에서만 공격 가능
        {
            currentCloseWeapon.anim.SetTrigger("Eat");
            quickSlotController.EatItem();
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

