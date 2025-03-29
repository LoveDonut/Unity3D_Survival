using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputManager
{
    private static InputSystem_Actions controls;

    // 플레이 모드 진입 시마다 강제로 초기화
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void OnSubsystemRegistration()
    {
        controls = null;
        actionCallbacks.Clear();
        isInitialized = false;
    }

    // 각 액션 이름에 대해 phase별로 콜백을 관리하는 중첩 딕셔너리
    private static Dictionary<string, Dictionary<InputActionPhase, List<System.Action<InputAction.CallbackContext>>>> actionCallbacks = 
        new Dictionary<string, Dictionary<InputActionPhase, List<System.Action<InputAction.CallbackContext>>>>();

    // 초기화 여부 확인용
    private static bool isInitialized = false;

    // 초기화 메서드 - 게임 시작 시 어디선가 한 번 호출해주어야 함
    public static void Initialize()
    {
        if(controls != null)
        {
            controls.Dispose();
        }
        Debug.Log("InputManager Initialize");
        controls = new InputSystem_Actions();
        actionCallbacks.Clear();

        // 모든 액션에 대해 콜백 관리를 위한 초기화
        foreach (var actionMap in controls.asset.actionMaps)
        {
            foreach (var action in actionMap.actions)
            {
                string actionId = action.name;
                actionCallbacks[actionId] = new Dictionary<InputActionPhase, List<System.Action<InputAction.CallbackContext>>>();
                
                // 각 phase별 콜백 리스트 초기화
                actionCallbacks[actionId][InputActionPhase.Started] = new List<System.Action<InputAction.CallbackContext>>();
                actionCallbacks[actionId][InputActionPhase.Performed] = new List<System.Action<InputAction.CallbackContext>>();
                actionCallbacks[actionId][InputActionPhase.Canceled] = new List<System.Action<InputAction.CallbackContext>>();
                
                // 각 phase별 이벤트 핸들러 등록
                action.started += ctx => InvokeCallbacks(action.name, InputActionPhase.Started, ctx);
                action.performed += ctx => InvokeCallbacks(action.name, InputActionPhase.Performed, ctx);
                action.canceled += ctx => InvokeCallbacks(action.name, InputActionPhase.Canceled, ctx);
            }
        }
        
        controls.Enable();
        isInitialized = true;
    }
    
    // 이 메서드를 Subscribe 등 다른 메서드 시작 부분에서 호출
    private static void EnsureInitialized()
    {
        if (isInitialized && controls != null) return;

        Initialize();
    }

    // 등록된 콜백 실행하는 메서드
    private static void InvokeCallbacks(string actionName, InputActionPhase phase, InputAction.CallbackContext context)
    {
        if (actionCallbacks.TryGetValue(actionName, out var phaseCallbacks) && 
            phaseCallbacks.TryGetValue(phase, out var callbacks))
        {
            // 안전한 순회를 위해 리스트 복사본 사용
            var callbacksCopy = new List<System.Action<InputAction.CallbackContext>>(callbacks);
            foreach (var callback in callbacksCopy)
            {
                callback?.Invoke(context);
            }
        }
    }

    // 특정 phase에 대한 콜백 추가
    public static void Subscribe(string actionName, System.Action<InputAction.CallbackContext> callback, InputActionPhase phase = InputActionPhase.Performed)
    {
        EnsureInitialized();
        
        if (!actionCallbacks.ContainsKey(actionName))
        {
            Debug.LogWarning($"Action '{actionName}' not found");
            return;
        }

        if (phase == InputActionPhase.Disabled)
        {
            // 모든 phase에 동일한 콜백 등록
            actionCallbacks[actionName][InputActionPhase.Started].Add(callback);
            actionCallbacks[actionName][InputActionPhase.Performed].Add(callback);
            actionCallbacks[actionName][InputActionPhase.Canceled].Add(callback);
        }
        else
        {
            actionCallbacks[actionName][phase].Add(callback);
        }
    }

    // 특정 phase에서 콜백 제거
    public static void Unsubscribe(string actionName, System.Action<InputAction.CallbackContext> callback, InputActionPhase phase = InputActionPhase.Performed)
    {
        if (!actionCallbacks.ContainsKey(actionName))
        {
            return;
        }

        if (phase == InputActionPhase.Disabled)
        {
            // 모든 phase에서 콜백 제거
            actionCallbacks[actionName][InputActionPhase.Started].Remove(callback);
            actionCallbacks[actionName][InputActionPhase.Performed].Remove(callback);
            actionCallbacks[actionName][InputActionPhase.Canceled].Remove(callback);
        }
        else
        {
            actionCallbacks[actionName][phase].Remove(callback);
        }
    }

    // 편의를 위한 일괄 구독 메서드
    public static void SubscribeToAllPhases(string actionName, System.Action<InputAction.CallbackContext> callback)
    {
        Subscribe(actionName, callback, InputActionPhase.Disabled);
    }

    // 특정 액션의 값을 직접 읽는 편의 메서드
    public static T GetActionValue<T>(string actionName) where T : struct
    {
        InputAction action = controls.asset.FindAction(actionName);
        if (action != null)
        {
            return action.ReadValue<T>();
        }
        
        Debug.LogWarning($"Action '{actionName}' not found");
        return default;
    }
    
    // 액션이 활성 상태인지 확인하는 메서드
    public static bool IsActionActive(string actionName)
    {
        InputAction action = controls.asset.FindAction(actionName);
        return action != null && action.phase != InputActionPhase.Disabled && action.phase != InputActionPhase.Waiting;
    }
    
    // 맵 활성화/비활성화 메서드
    public static void EnableActionMap(string mapName)
    {
        var map = controls.asset.FindActionMap(mapName);
        if (map != null)
        {
            map.Enable();
        }
    }
    
    public static void DisableActionMap(string mapName)
    {
        var map = controls.asset.FindActionMap(mapName);
        if (map != null)
        {
            map.Disable();
        }
    }
}