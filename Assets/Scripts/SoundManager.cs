using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public string name; // 곡의 이름
    public AudioClip clip; // 곡
}
public class SoundManager : MonoBehaviour
{
#region Singleton
    // 싱글턴
    public static SoundManager Instance { get; private set; }
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
#endregion

    public AudioSource[] audioSourceEffects;
    public AudioSource audioSourceBgm;
    public string[] playSoundName;
    public Sound[] effectSounds;
    public Sound[] bgmSounds;

    void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];
    }

    public void PlaySE(string name)
    {
        for(int i=0; i<effectSounds.Length; i++)
        {
            if(name == effectSounds[i].name)
            {
                for(int j=0; j<audioSourceEffects.Length; j++)
                {
                    if(!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 AuidoSource가 사용 중입니다.");
                return;
            }
        }
        Debug.Log($"{name} 사운드가 SoundManager에 등록되지 않았습니다.");
    }
    public void StopAllSE()
    {
        for(int i=0; i<audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSE(string name)
    {
        for(int i=0; i<audioSourceEffects.Length; i++)
        {
            if(playSoundName[i] == name)
            {
                audioSourceEffects[i].Stop();
                return;
            }
        }
        Debug.Log($"{name} 사운드가 SoundManager에 등록되지 않았습니다.");
    }
}
