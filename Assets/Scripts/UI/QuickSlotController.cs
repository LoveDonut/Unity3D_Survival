using System.Collections;
using UnityEngine;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] Slot[] quickSlots; // 퀵슬롯 배열
    [SerializeField] Transform tf_Parent; // 퀵슬롯의 부모 객체
    [SerializeField] Transform tf_ItemPos; // 아이템이 위치할 손 끝
    public static GameObject go_HandItem; // 손에 든 아이템템
    // 필요한 컴포넌트
    [SerializeField] GameObject go_SelectedImage; // 선택된 퀵슬롯의 이미지
    [SerializeField] WeaponManager weaponManager;
    int selectedSlot; // 선택된 퀵슬롯. 0 ~ 7
    void Start()
    {
        quickSlots = tf_Parent.GetComponentsInChildren<Slot>();
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
                ChangeHand(quickSlots[selectedSlot].item);
            }
            else
            {
                StartCoroutine(weaponManager.ChangeWeaponCoroutine("HAND", "맨손"));
            }
        }
        else
        {
            ChangeHand();
        }
    }
    void ChangeHand(Item _item = null)
    {
        StartCoroutine(weaponManager.ChangeWeaponCoroutine("HAND", "맨손"));

        if(_item != null)
        {
            StartCoroutine(HandItemCoroutine());
        }
    }
    IEnumerator HandItemCoroutine()
    {
        HandController.isActivate = false;
        yield return new WaitUntil(() => HandController.isActivate == true);
        go_HandItem = Instantiate(quickSlots[selectedSlot].item.itemPrefab, tf_ItemPos.position, tf_ItemPos.rotation);
        go_HandItem.GetComponent<Rigidbody>().isKinematic = true;
        go_HandItem.GetComponent<BoxCollider>().enabled = false;
        go_HandItem.tag = "Untagged";
        go_HandItem.layer = LayerMask.NameToLayer("Weapon");
        go_HandItem.transform.SetParent(tf_ItemPos);
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
    public void EatItem()
    {
        quickSlots[selectedSlot].SetSlotCount(-1);

        if(quickSlots[selectedSlot].itemCount <= 0)
        {
            Destroy(go_HandItem);
        }
    }
}
