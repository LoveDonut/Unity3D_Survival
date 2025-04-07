using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputNumber : MonoBehaviour
{
    bool activated;
    // 필요한 컴포넌트
    [SerializeField] TMP_InputField text_Preview;
    [SerializeField] TextMeshProUGUI text_Input;
    [SerializeField] GameObject go_Base;
    [SerializeField] ActionController player;
    Action<InputAction.CallbackContext> OKCallback;
    Action<InputAction.CallbackContext> CancelCallback;
    void OnEnable()
    {
        OKCallback = (ctx) => OK();
        CancelCallback = (ctx) => Cancel();
        InputManager.Subscribe("OK", OKCallback);
        InputManager.Subscribe("Cancel", CancelCallback);
    }
    void OnDisable()
    {
        InputManager.Unsubscribe("OK", OKCallback);
        InputManager.Unsubscribe("Cancel", CancelCallback);
    }
    public void Call()
    {
        activated = true;
        go_Base.SetActive(true);
        text_Input.SetText("");
        text_Preview.text = DragSlot.instance.dragSlot.itemCount.ToString();
    }
    public void Cancel()
    {
        if(!activated) return;
        activated = false;
        go_Base.SetActive(false);
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }
    public void OK()
    {
        if(!activated) return;
        DragSlot.instance.SetColor(0);
        int num;
        string inputText = text_Input.text.Trim((char)8203);
        if(inputText != "")
        {
            if(CheckNumber(inputText))
            {
                num = int.Parse(inputText);
                if(num > DragSlot.instance.dragSlot.itemCount)
                {
                    num = DragSlot.instance.dragSlot.itemCount;
                }
            }
            else
            {
                num = 1;
            }
        }
        else
        {
            num = int.Parse(text_Preview.text);
        }
        StartCoroutine(DropItemCoroutine(num));
    }
    IEnumerator DropItemCoroutine(int _num)
    {
        for(int i=0; i<_num; i++)
        {
            
            if(DragSlot.instance.dragSlot.item.itemPrefab != null)
            {
                Instantiate(DragSlot.instance.dragSlot.item.itemPrefab, player.transform.position + player.transform.forward, Quaternion.identity);
            }
            DragSlot.instance.dragSlot.SetSlotCount(-1);
            yield return new WaitForSeconds(0.05f);
        }
        DragSlot.instance.dragSlot = null;
        go_Base.SetActive(false);
        activated = false;
    }
    bool CheckNumber(string _argString)
    {
        char[] _tempCharArray = _argString.ToCharArray();
        Debug.Log(_tempCharArray);
        for(int i=0; i<_tempCharArray.Length - 1; i++)
        {
            if(_tempCharArray[i] >= '0' && _tempCharArray[i] <= '9')
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}
