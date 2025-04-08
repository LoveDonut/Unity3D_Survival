using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler, IDropHandler
{
    [SerializeField] bool isQuickSlot; // 퀵슬롯 여부 판단
    [SerializeField] int quickSlotNumber; // 퀵슬롯 번호호
    public Item item; // 획득한 아이템
    public int itemCount; // 획득한 아이템의 개수
    public Image itemImage; // 아이템의 이미지
    Vector3 originPos;
    // 필요한 컴포넌트
    [SerializeField] TextMeshProUGUI text_Count;
    [SerializeField] GameObject go_CountImage;
    [SerializeField] RectTransform quickSlotBaseRect; // 퀵슬롯 영역
    ItemEffectDatabase itemEffectDatabase;
    [SerializeField] RectTransform baseRect; // 인벤토리 영역
    InputNumber inputNumber;

    void Awake()
    {
        itemEffectDatabase = FindAnyObjectByType<ItemEffectDatabase>();
        inputNumber = FindAnyObjectByType<InputNumber>();
    }

    void Start()
    {
        originPos = transform.position;
    }

    // 이미지 투명도 조절
    void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 아이템 획득
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if(item.itemType != Item.ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            text_Count.SetText(itemCount.ToString());
        }
        else
        {
            text_Count.SetText("0");
            go_CountImage.SetActive(false);
        }

        SetColor(1f);
    }

    // 아이템 개수 조정
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.SetText(itemCount.ToString());

        if(itemCount <= 0)
        {
            ClearSlot();
        }
    }
    
    // 슬롯 초기화
    void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        go_CountImage.SetActive(false);
        text_Count.SetText("0");
    }

    // 마우스가 슬롯에 들어갈 때 발동
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
        {
            itemEffectDatabase.ShowToolTip(item, transform.position);
        }
    }
    // 마우스가 슬롯에서 나갈 때 발동동
    public void OnPointerExit(PointerEventData eventData)
    {
        if(item != null)
        {
            itemEffectDatabase.HideToolTip();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(item != null)
            {
                itemEffectDatabase.UseItem(item);
                if(item.itemType == Item.ItemType.Used)
                {
                    SetSlotCount(-1);
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);

            DragSlot.instance.transform.position = eventData.position;
        }        
    }

    // 드래그가 끝나기만 해도 호출됨
    public void OnEndDrag(PointerEventData eventData)
    {
        if(!((DragSlot.instance.transform.localPosition.x > baseRect.rect.xMin &&
            DragSlot.instance.transform.localPosition.x < baseRect.rect.xMax &&
            DragSlot.instance.transform.localPosition.y > baseRect.rect.yMin &&
            DragSlot.instance.transform.localPosition.y < baseRect.rect.yMax) ||
            (DragSlot.instance.transform.localPosition.x > quickSlotBaseRect.rect.xMin &&
            DragSlot.instance.transform.localPosition.x < quickSlotBaseRect.rect.xMax &&
            DragSlot.instance.transform.position.y > quickSlotBaseRect.transform.position.y - quickSlotBaseRect.rect.yMax &&
            DragSlot.instance.transform.position.y < quickSlotBaseRect.transform.position.y - quickSlotBaseRect.rect.yMin)))
            {
                if(DragSlot.instance.dragSlot != null)
                {
                    inputNumber.Call();
                }
            }
            else
            {
                DragSlot.instance.SetColor(0f);
                DragSlot.instance.dragSlot = null;
            }
    }

    // 슬롯위에 올라와야만 호출됨
    public void OnDrop(PointerEventData eventData)
    {
        if(DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();

            if(isQuickSlot) // 인벤토리에서 퀵슬롯으로 (혹은 퀵슬롯에서 퀵슬롯으로)
            {
                // 활성화된 퀵슬롯이면 교체 작업
                itemEffectDatabase.IsActivatedQuickSlot(quickSlotNumber);
            }
            else // 인벤토리 -> 인벤토리, 퀵슬롯 -> 인벤토리
            {
                if(DragSlot.instance.dragSlot.isQuickSlot) // 퀵슬롯 -> 인벤토리
                {
                    // 활성화된 퀵슬롯이면 교체 작업
                    itemEffectDatabase.IsActivatedQuickSlot(DragSlot.instance.dragSlot.quickSlotNumber);
                }
            }
        }
    }
    public int GetQuickSlotNumber()
    {
        return quickSlotNumber;
    }
    void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if(_tempItem != null)
        {
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        }
        else
        {
            DragSlot.instance.dragSlot.ClearSlot();
        }
    }
}
