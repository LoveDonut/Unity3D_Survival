using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;

    // 필요한 컴포넌트
    [SerializeField] GameObject go_InventoryBase; 
    [SerializeField] GameObject go_SlotsParent;
    Slot[] slots; // 슬롯들

    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
    }

    void OnEnable()
    {
        InputManager.Subscribe("Inventory", TryOpenInventory, InputActionPhase.Performed);
    }

    void OnDisable()
    {
        InputManager.Unsubscribe("Inventory", TryOpenInventory, InputActionPhase.Performed);        
    }

    void TryOpenInventory(InputAction.CallbackContext ctx)
    {
        inventoryActivated = !inventoryActivated;
        if(inventoryActivated)
        {
            OpenInventory();
        }
        else
        {
            CloseInventory();
        }
    }

    void OpenInventory()
    {
        go_InventoryBase.SetActive(true);
    }
    void CloseInventory()
    {
        go_InventoryBase.SetActive(false);
    }
    public void AcquireItem(Item _item, int _count = 1)
    {
        if(Item.ItemType.Equipment != _item.itemType)
        {
            for(int i=0; i<slots.Length; i++)
            {
                if(slots[i].item != null && slots[i].item.itemName == _item.itemName)
                {
                    slots[i].SetSlotCount(_count);
                    return;
                }
            }
        }

        for(int i=0; i<slots.Length; i++)
        {
            if(slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
    }
}
