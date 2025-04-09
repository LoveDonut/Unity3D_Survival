using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public static bool isChangeWeapon = false; // 무기 중복 교체 실행 방지
    public static Transform currentWeapon; // 현재 무기
    public static Animator currentWeaponAnim; // 현재 무기 애니메이션

    [SerializeField] float changeWeaponDelayTime;
    [SerializeField] float changeWeaponEndDelayTime;
    // 무기 종류 관리
    [SerializeField] Gun[] guns;
    [SerializeField] CloseWeapon[] hands;
    [SerializeField] CloseWeapon[] axes;
    [SerializeField] CloseWeapon[] pickaxes;
    // 관리 차원에서 쉽게 무기 접근이 가능하도록 만듦듦
    Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    Dictionary<string, CloseWeapon> handDictionary = new Dictionary<string, CloseWeapon>();
    Dictionary<string, CloseWeapon> axeDictionary = new Dictionary<string, CloseWeapon>();
    Dictionary<string, CloseWeapon> pickaxeDictionary = new Dictionary<string, CloseWeapon>();
    // 필요한 컴포넌트
    [SerializeField] GunController gunController;
    [SerializeField] HandController handController;
    [SerializeField] AxeController axeController;
    [SerializeField] PickaxeController pickaxeController;
    [SerializeField] string currentWeaponType; // 현재 무기의 타입
    void Start()
    {
        for(int i=0; i<guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]);
        }
        for(int i=0; i<hands.Length; i++)
        {
            handDictionary.Add(hands[i].closeWeaponName, hands[i]);
        }
        for(int i=0; i<axes.Length; i++)
        {
            axeDictionary.Add(axes[i].closeWeaponName, axes[i]);
        }        for(int i=0; i<axes.Length; i++)
        {
            pickaxeDictionary.Add(pickaxes[i].closeWeaponName, pickaxes[i]);
        }
    }

    public IEnumerator ChangeWeaponCoroutine(string type, string name)
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("WeaponOut");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction();
        WeaponChange(type , name);

        yield return new WaitForSeconds(changeWeaponEndDelayTime);

        currentWeaponType = type;
        isChangeWeapon = false;
    }

    void CancelPreWeaponAction()
    {
        switch(currentWeaponType)
        {
            case "GUN":
                gunController.CancelFineSight();
                gunController.CancelReload();
                GunController.isActivate = false;
                break;
            case "HAND":
                HandController.isActivate = false;
                if(QuickSlotController.go_HandItem != null)
                {
                    Destroy(QuickSlotController.go_HandItem);
                }
                break;
            case "AXE":
                AxeController.isActivate = false;
                break;
            case "PICKAXE":
                PickaxeController.isActivate = false;
                break;
        }
    }

    void WeaponChange(string type, string name)
    {
        if(type == "GUN")
        {
            gunController.GunChange(gunDictionary[name]);
        }
        else if(type == "HAND")
        {
            handController.CloseWeaponChange(handDictionary[name]);
        }
        else if(type == "AXE")
        {
            axeController.CloseWeaponChange(axeDictionary[name]);
        }        
        else if(type == "PICKAXE")
        {
            pickaxeController.CloseWeaponChange(pickaxeDictionary[name]);
        }
    }
}
