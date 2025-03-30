using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] int hp; // 바위의 체력
    [SerializeField] float destroyTime; // 파편 제거 시간
    [SerializeField] SphereCollider col; // 구체 콜라이더
    // 필요한 게임 오브젝트
    [SerializeField] GameObject go_rock; // 일반 바위
    [SerializeField] GameObject go_debiris; // 깨진 바위
    [SerializeField] GameObject go_effect_prefabs; // 채굴 이펙트트
    // 필요한 사운드 이름
    [SerializeField] string strike_Sound;
    [SerializeField] string destroy_Sound;
    public void Mining()
    {
        SoundManager.Instance.PlaySE(strike_Sound);
        var clone = Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;
        if(hp <= 0)
        {
            Destruction();
        }
    }

    void Destruction()
    {
        SoundManager.Instance.PlaySE(destroy_Sound);
        col.enabled = false;
        Destroy(go_rock);

        go_debiris.SetActive(true);
        Destroy(go_debiris, destroyTime);
    }
}
