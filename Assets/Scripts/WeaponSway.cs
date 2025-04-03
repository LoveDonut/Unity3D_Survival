using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSway : MonoBehaviour
{
    Vector3 originPos; // 기존 위치
    Vector3 currentPos; // 현재 위치
    [SerializeField] Vector3 limitPos; // sway 한계
    [SerializeField] Vector3 fineSightLimitPos; // 정조준 시 sway 한계
    [SerializeField] Vector3 smoothSway; // 부드러운 움직임 정도
    // 필요한 컴포넌트
    [SerializeField] GunController gunController;
    void OnEnable()
    {
        InputManager.Subscribe("Look", TrySway, InputActionPhase.Performed);
    }
    void OnDisable()
    {
        InputManager.Unsubscribe("Look", TrySway, InputActionPhase.Performed);        
    }
    void Start()
    {
        originPos = transform.localPosition;
    }
    void Update()
    {
        if(InputManager.GetActionValue<Vector2>("Look") == Vector2.zero)
        {
            BackToOriginPos();
        }
    }

    void TrySway(InputAction.CallbackContext ctx)
    {
        if(!Inventory.inventoryActivated)
        {
            Swaying();
        }
    }
    void SwayStop()
    {
        BackToOriginPos();
    }
    void Swaying()
    {
        float moveX = InputManager.GetActionValue<Vector2>("Look").x;
        float moveY = InputManager.GetActionValue<Vector2>("Look").y;

        if(!gunController.IsFineSightMode())
        {
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -moveX, smoothSway.x), -limitPos.x, limitPos.x),
                            Mathf.Clamp(Mathf.Lerp(currentPos.y, -moveY, smoothSway.x), -limitPos.y, limitPos.y),
                            originPos.z);
        }
        else
        {
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -moveX, smoothSway.y), -fineSightLimitPos.x, fineSightLimitPos.x),
                            Mathf.Clamp(Mathf.Lerp(currentPos.y, -moveY, smoothSway.y), -fineSightLimitPos.y, fineSightLimitPos.y),
                            originPos.z);
        }
        transform.localPosition = currentPos;
    }
    void BackToOriginPos()
    {
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x * 10f);
        transform.localPosition = currentPos;
    }
}
