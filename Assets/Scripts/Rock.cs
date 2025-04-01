using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] int hp; // 바위의 체력
    [SerializeField] float destroyTime; // 파편 제거 시간
    [SerializeField] int count = 5; // 생성되는 돌맹이 아이템 개수
    [SerializeField] SphereCollider col; // 구체 콜라이더
    // 필요한 게임 오브젝트
    [SerializeField] GameObject go_rock; // 일반 바위
    [SerializeField] GameObject go_debiris; // 깨진 바위
    [SerializeField] GameObject go_effect_prefabs; // 채굴 이펙트
    [SerializeField] GameObject go_rock_tiem_prefab;
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
        for(int i=0; i<count; i++)
        {
            Instantiate(go_rock_tiem_prefab, go_rock.transform.position, Quaternion.identity);
        }
        Destroy(go_rock);

        go_debiris.SetActive(true);
        Destroy(go_debiris, destroyTime);
    }
}
