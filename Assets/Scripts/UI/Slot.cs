using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Item item; // 획득한 아이템
    public int itemCount; // 획득한 아이템의 개수
    public Image itemImage; // 아이템의 이미지
    // 필요한 컴포넌트
    [SerializeField] TextMeshProUGUI text_Count;
    [SerializeField] GameObject go_CountImage;

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
}
