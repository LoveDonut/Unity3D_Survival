using UnityEngine;

public static class Util
{
    public static void MakeSingleton<T>(ref T instance, T gameobject) where T : MonoBehaviour
    {
        if(instance == null)
        {
            instance = gameobject;
            GameObject.DontDestroyOnLoad(instance);
        }
        else
        {
            GameObject.Destroy(gameobject);
        }
    }    
}
