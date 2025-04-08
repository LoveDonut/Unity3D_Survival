using System;
using UnityEngine;

[Serializable]
public class ItemEffect
{
    public string itemName; // 아이템의 이름 (키값)
    [Tooltip("HP, SP, DP, HUNGRY, THIRSTY, SATISFY 중 하나여야 합니다")]
    public string[] part; // 부위
    public int[] num; // 수치
}

public class ItemEffectDatabase : MonoBehaviour
{
    const string HP = "HP", SP = "SP", DP = "DP", HUNGRY = "HUNGRY", THIRSTY = "THIRSTY", SATISFY = "SATISFY";
    [SerializeField] ItemEffect[] itemEffects;
    // 필요한 컴포넌트
    [SerializeField] StatusController statusController;
    [SerializeField] WeaponManager weaponManager;
    [SerializeField] SlotToolTip slotToolTip;
    [SerializeField] QuickSlotController quickSlotController;

    // QuickSlotController 징검다리
    public void IsActivatedQuickSlot(int _num)
    {
        quickSlotController.IsActivatedQucikSlot(_num);
    }
    // SlotToolTip 징검다리
    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        slotToolTip.ShowToolTip(_item, _pos);
    }
    // SlotToolTip 징검다리
    public void HideToolTip()
    {
        slotToolTip.HideToolTip();
    }
    public void UseItem(Item _item)
    {
        if(_item.itemType == Item.ItemType.Equipment)
        {
            StartCoroutine(weaponManager.ChangeWeaponCoroutine(_item.weaponType, _item.itemName));
        }
        else if(_item.itemType == Item.ItemType.Used)
        {
            for(int x=0; x<itemEffects.Length; x++)
            {
                if(itemEffects[x].itemName == _item.itemName)
                {
                    for(int y=0; y<itemEffects[x].part.Length; y++)
                    {
                        switch(itemEffects[x].part[y])
                        {
                            case HP:
                                statusController.IncreaseHP(itemEffects[x].num[y]);
                                break;
                            case SP:
                                statusController.IncreaseSP(itemEffects[x].num[y]);
                                break;
                            case DP:
                                statusController.IncreaseDP(itemEffects[x].num[y]);
                                break;
                            case HUNGRY:
                                statusController.IncreaseHungry(itemEffects[x].num[y]);
                                break;
                            case THIRSTY:
                                break;
                            default:
                                Debug.Log("잘못된 Status 부위. HP, SP, DP, HUNGRY, THIRSTY, SATISFY 중 하나여야 합니다.");
                                break;
                        }
                        Debug.Log($"{_item.itemName} 을 사용했습니다");
                    }
                    return;
                }
            }
            Debug.Log("ItemEffectDatabase에 일치하는 itemName이 없습니다");
        }
    }
}
