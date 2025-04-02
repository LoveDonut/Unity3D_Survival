using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionController : MonoBehaviour
{
    [SerializeField] float range; // 습득 가능한 최대 거리
    bool pickupActivated = false; // 습득 가능할 시 true
    RaycastHit hitInfo; // 충돌체 정보 저장
    [SerializeField] LayerMask layerMask; // 아이템 레이어에만 반응하도록 레이어 마스크를 설정
    // 필요한 컴포넌트
    [SerializeField] TextMeshProUGUI actionText;
    [SerializeField] Inventory inventory;
    void OnEnable()
    {
//        InputManager.Subscribe("Action", CheckItem,InputActionPhase.Performed);
        InputManager.Subscribe("Action", CanPickUp,InputActionPhase.Performed);
    }
    void OnDisable()
    {
//        InputManager.Unsubscribe("Action", CheckItem,InputActionPhase.Performed);        
        InputManager.Unsubscribe("Action", CanPickUp,InputActionPhase.Performed);        
    }
    void Update()
    {
        CheckItem();        
    }

    void CheckItem()
    {
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
        {
            if(hitInfo.transform.CompareTag("Item"))
            {
                ItemInfoAppear();
            }
        }
        else
        {
            InfoDisappear();
        }
    }
    void CanPickUp(InputAction.CallbackContext ctx)
    {
        if(pickupActivated)
        {
            if(hitInfo.transform != null)
            {
                Debug.Log($"{hitInfo.transform.GetComponent<ItemPickUp>().item.itemName} Get Succeed");
                inventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                InfoDisappear();
            }
        }
    }
    void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.SetText($"Get {hitInfo.transform.GetComponent<ItemPickUp>().item.itemName} <color=yellow> (E) </color>");
    }
    void InfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }
}
