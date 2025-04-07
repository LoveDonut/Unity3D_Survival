using TMPro;
using UnityEngine;

public class SlotToolTip : MonoBehaviour
{
    // 필요한 컴포넌트
    [SerializeField] GameObject go_Base;
    [SerializeField] TextMeshProUGUI text_ItemName;
    [SerializeField] TextMeshProUGUI text_ItemDesc;
    [SerializeField] TextMeshProUGUI text_ItemHowtoUsed;

    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        go_Base.SetActive(true);
        _pos += new Vector3(go_Base.GetComponent<RectTransform>().rect.width * 0.5f,
                            -go_Base.GetComponent<RectTransform>().rect.height, 0f);
        go_Base.transform.position = _pos;

        text_ItemName.SetText(_item.itemName);
        text_ItemDesc.SetText(_item.itemDesc);

        if(_item.itemType == Item.ItemType.Equipment)
        {
            text_ItemHowtoUsed.SetText("Right Click to Equip");
        }
        else if (_item.itemType == Item.ItemType.Used)
        {
            text_ItemHowtoUsed.SetText("Right Click to Use");
        }
        else
        {
            text_ItemHowtoUsed.SetText("");
        }
    }
    public void HideToolTip()
    {
        go_Base.SetActive(false);
    }
}
