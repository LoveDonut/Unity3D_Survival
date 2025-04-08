using UnityEngine;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] Slot[] quickSlots; // 퀵슬롯 배열
    [SerializeField] Transform tf_parent; // 퀵슬롯의 부모 객체
    // 필요한 컴포넌트
    [SerializeField] GameObject go_SelectedImage; // 선택된 퀵슬롯의 이미지
    [SerializeField] WeaponManager weaponManager;
    int selectedSlot; // 선택된 퀵슬롯. 0 ~ 7
    void Start()
    {
        quickSlots = tf_parent.GetComponentsInChildren<Slot>();
        selectedSlot = 0;
    }
    void OnEnable()
    {
        for(int i=0; i<8; i++)
        {
            int slotNum = i;
            InputManager.Subscribe($"QuickSlot{i+1}", (ctx) => ChangeSlot(slotNum));
        }
    }
    void OnDisable()
    {
        for(int i=0; i<8; i++)
        {
            InputManager.UnsubscribeAll($"QuickSlot{i+1}");
        }
    }
    void ChangeSlot(int _num)
    {
        SelectedSlot(_num);

        Execute();
    }

    void SelectedSlot(int _num)
    {
        // 선택된 슬롯
        selectedSlot = _num;
        
        go_SelectedImage.transform.position = quickSlots[selectedSlot].transform.position; // 선택된 슬롯으로 이미지 이동
    }
    void Execute()
    {
        if(quickSlots[selectedSlot].item != null)
        {
            if(quickSlots[selectedSlot].item.itemType == Item.ItemType.Equipment)
            {
                StartCoroutine(weaponManager.ChangeWeaponCoroutine(quickSlots[selectedSlot].item.weaponType, quickSlots[selectedSlot].item.itemName));
            }
            else if(quickSlots[selectedSlot].item.itemType == Item.ItemType.Used)
            {
                StartCoroutine(weaponManager.ChangeWeaponCoroutine("HAND", "맨손"));
            }
            else
            {
                StartCoroutine(weaponManager.ChangeWeaponCoroutine("HAND", "맨손"));
            }
        }
        else
        {
            StartCoroutine(weaponManager.ChangeWeaponCoroutine("HAND", "맨손"));
        }
    }
    public void IsActivatedQucikSlot(int _num)
    {
        if(selectedSlot == _num)
        {
            Execute();
            return;
        }
        if(DragSlot.instance != null && DragSlot.instance.dragSlot != null)
        {
            if(DragSlot.instance.dragSlot.GetQuickSlotNumber() == selectedSlot)
            {
                Execute();
                return;
            }
        }
    }
}
