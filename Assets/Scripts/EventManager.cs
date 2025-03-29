using System;
using System.Collections.Generic;
using UnityEngine;

using static Util;

/// <summary>
/// Enum 타입을 이용한 이벤트 매니저 - 게임 내 다양한 이벤트를 관리하고 전달하는 싱글톤 클래스
/// </summary>
public static class EventManager
{
    #region 필드

    // 파라미터가 없는 이벤트를 위한 딕셔너리
    static Dictionary<Enum, Action> eventDictionary = new Dictionary<Enum, Action>();

    // 제네릭 파라미터가 있는 이벤트를 위한 딕셔너리
    static Dictionary<string, object> eventDictionaryWithParam = new Dictionary<string, object>();

    #endregion

    #region 파라미터 없는 이벤트 메서드

    /// <summary>
    /// 이벤트에 리스너 추가하기
    /// </summary>
    /// <param name="state">이벤트 이름</param>
    /// <param name="listener">콜백 메서드</param>
    public static void AddListener<TEnum>(TEnum state, Action listener) where TEnum : Enum
    {
        if (eventDictionary.TryGetValue(state, out Action thisEvent))
        {
            thisEvent += listener;
            eventDictionary[state] = thisEvent;
        }
        else
        {
            eventDictionary.Add(state, listener);
        }
    }

    /// <summary>
    /// 이벤트에서 리스너 제거하기
    /// </summary>
    /// <param name="state">이벤트 이름</param>
    /// <param name="listener">콜백 메서드</param>
    public static void RemoveListener<TEnum>(TEnum state, Action listener) where TEnum : Enum
    {
        if (eventDictionary.TryGetValue(state, out Action thisEvent))
        {
            thisEvent -= listener;
            
            if (thisEvent == null)
            {
                eventDictionary.Remove(state);
            }
            else
            {
                eventDictionary[state] = thisEvent;
            }
        }
    }

    /// <summary>
    /// 이벤트 발생시키기
    /// </summary>
    /// <param name="state">이벤트 이름</param>
    public static void TriggerEvent<TEnum>(TEnum state) where TEnum : Enum
    {
        if (eventDictionary.TryGetValue(state, out Action thisEvent))
        {
            thisEvent?.Invoke();
        }
    }

    #endregion

    #region 파라미터가 있는 이벤트 메서드

    /// <summary>
    /// 파라미터가 있는 이벤트에 리스너 추가하기
    /// </summary>
    /// <typeparam name="T">파라미터 타입</typeparam>
    /// <param name="state">이벤트 이름</param>
    /// <param name="listener">콜백 메서드</param>
    public static void AddListener<TEnum, T>(TEnum state, Action<T> listener) where TEnum : Enum
    {
        string key = GetParamEventKey<TEnum, T>(state);
        
        if (eventDictionaryWithParam.TryGetValue(key, out object thisEvent))
        {
            eventDictionaryWithParam[key] = (Action<T>)thisEvent + listener;
        }
        else
        {
            eventDictionaryWithParam.Add(key, listener);
        }
    }

    /// <summary>
    /// 파라미터가 있는 이벤트에서 리스너 제거하기
    /// </summary>
    /// <typeparam name="T">파라미터 타입</typeparam>
    /// <param name="state">이벤트 이름</param>
    /// <param name="listener">콜백 메서드</param>
    public static void RemoveListener<TEnum, T>(TEnum state, Action<T> listener) where TEnum : Enum
    {
        string key = GetParamEventKey<TEnum, T>(state);
        
        if (eventDictionaryWithParam.TryGetValue(key, out object thisEvent))
        {
            Action<T> currentEvent = (Action<T>)thisEvent;
            currentEvent -= listener;
            
            if (currentEvent == null)
            {
                eventDictionaryWithParam.Remove(key);
            }
            else
            {
                eventDictionaryWithParam[key] = currentEvent;
            }
        }
    }

    /// <summary>
    /// 파라미터가 있는 이벤트 발생시키기
    /// </summary>
    /// <typeparam name="T">파라미터 타입</typeparam>
    /// <param name="state">이벤트 이름</param>
    /// <param name="param">이벤트 파라미터</param>
    public static void TriggerEvent<TEnum, T>(TEnum state, T param) where TEnum : Enum
    {
        string key = GetParamEventKey<TEnum, T>(state);
        
        if (eventDictionaryWithParam.TryGetValue(key, out object thisEvent))
        {
            Action<T> currentEvent = (Action<T>)thisEvent;
            currentEvent?.Invoke(param);
        }
    }

    /// <summary>
    /// 파라미터가 있는 이벤트의 고유 키 생성
    /// </summary>
    private static string GetParamEventKey<TEnum, T>(TEnum state) where TEnum : Enum
    {
        return $"{typeof(TEnum).Name}_{state.ToString()}_{typeof(T).Name}";
    }

    #endregion

    #region 다중 파라미터 이벤트

    /// <summary>
    /// 두 개의 파라미터가 있는 이벤트에 리스너 추가하기
    /// </summary>
    public static void AddListener<TEnum, T1, T2>(TEnum state, Action<T1, T2> listener) where TEnum : Enum
    {
        string key = $"{typeof(TEnum).Name}_{state.ToString()}_{typeof(T1).Name}_{typeof(T2).Name}";
        
        if (eventDictionaryWithParam.TryGetValue(key, out object thisEvent))
        {
            eventDictionaryWithParam[key] = (Action<T1, T2>)thisEvent + listener;
        }
        else
        {
            eventDictionaryWithParam.Add(key, listener);
        }
    }

    /// <summary>
    /// 두 개의 파라미터가 있는 이벤트에서 리스너 제거하기
    /// </summary>
    public static void RemoveListener<TEnum, T1, T2>(TEnum state, Action<T1, T2> listener) where TEnum : Enum
    {
        string key = $"{typeof(TEnum).Name}_{state.ToString()}_{typeof(T1).Name}_{typeof(T2).Name}";
        
        if (eventDictionaryWithParam.TryGetValue(key, out object thisEvent))
        {
            Action<T1, T2> currentEvent = (Action<T1, T2>)thisEvent;
            currentEvent -= listener;
            
            if (currentEvent == null)
            {
                eventDictionaryWithParam.Remove(key);
            }
            else
            {
                eventDictionaryWithParam[key] = currentEvent;
            }
        }
    }

    /// <summary>
    /// 두 개의 파라미터가 있는 이벤트 발생시키기
    /// </summary>
    public static void TriggerEvent<TEnum, T1, T2>(TEnum state, T1 param1, T2 param2) where TEnum : Enum
    {
        string key = $"{typeof(TEnum).Name}_{state.ToString()}_{typeof(T1).Name}_{typeof(T2).Name}";
        
        if (eventDictionaryWithParam.TryGetValue(key, out object thisEvent))
        {
            Action<T1, T2> currentEvent = (Action<T1, T2>)thisEvent;
            currentEvent?.Invoke(param1, param2);
        }
    }

    /// <summary>
    /// 세 개의 파라미터가 있는 이벤트에 리스너 추가하기
    /// </summary>
    public static void AddListener<TEnum, T1, T2, T3>(TEnum state, Action<T1, T2, T3> listener) where TEnum : Enum
    {
        string key = $"{typeof(TEnum).Name}_{state.ToString()}_{typeof(T1).Name}_{typeof(T2).Name}_{typeof(T3).Name}";
        
        if (eventDictionaryWithParam.TryGetValue(key, out object thisEvent))
        {
            eventDictionaryWithParam[key] = (Action<T1, T2, T3>)thisEvent + listener;
        }
        else
        {
            eventDictionaryWithParam.Add(key, listener);
        }
    }

    /// <summary>
    /// 세세 개의 파라미터가 있는 이벤트에서 리스너 제거하기
    /// </summary>
    public static void RemoveListener<TEnum, T1, T2, T3>(TEnum state, Action<T1, T2, T3> listener) where TEnum : Enum
    {
        string key = $"{typeof(TEnum).Name}_{state.ToString()}_{typeof(T1).Name}_{typeof(T2).Name}_{typeof(T3).Name}";
        
        if (eventDictionaryWithParam.TryGetValue(key, out object thisEvent))
        {
            Action<T1, T2, T3> currentEvent = (Action<T1, T2, T3>)thisEvent;
            currentEvent -= listener;
            
            if (currentEvent == null)
            {
                eventDictionaryWithParam.Remove(key);
            }
            else
            {
                eventDictionaryWithParam[key] = currentEvent;
            }
        }
    }

    /// <summary>
    /// 세세 개의 파라미터가 있는 이벤트 발생시키기
    /// </summary>
    public static void TriggerEvent<TEnum, T1, T2, T3>(TEnum state, T1 param1, T2 param2, T3 param3) where TEnum : Enum
    {
        string key = $"{typeof(TEnum).Name}_{state.ToString()}_{typeof(T1).Name}_{typeof(T2).Name}_{typeof(T3).Name}";
        
        if (eventDictionaryWithParam.TryGetValue(key, out object thisEvent))
        {
            Action<T1, T2, T3> currentEvent = (Action<T1, T2, T3>)thisEvent;
            currentEvent?.Invoke(param1, param2, param3);
        }
    }

    #endregion

    #region 특정 이벤트를 위한 도우미 메서드

    /// <summary>
    /// 모든 이벤트 리스너 제거하기
    /// </summary>
    public static void ClearAllListeners()
    {
        eventDictionary.Clear();
        eventDictionaryWithParam.Clear();
        Debug.Log("모든 이벤트 리스너가 제거되었습니다.");
    }

    /// <summary>
    /// 특정 이벤트의 모든 리스너 제거하기
    /// </summary>
    /// <param name="state">이벤트 이름</param>
    public static void ClearEventListeners<TEnum>(TEnum state) where TEnum : Enum
    {
        // 파라미터 없는 이벤트 제거
        if (eventDictionary.ContainsKey(state))
        {
            eventDictionary.Remove(state);
        }

        // 파라미터 있는 이벤트들 제거 (이름으로 시작하는 모든 키)
        List<string> keysToRemove = new List<string>();
        
        foreach (var key in eventDictionaryWithParam.Keys)
        {
            if (key.StartsWith(typeof(TEnum).Name + "_"))
            {
                keysToRemove.Add(key);
            }
        }
        
        foreach (var key in keysToRemove)
        {
            eventDictionaryWithParam.Remove(key);
        }
    }
    #endregion
}