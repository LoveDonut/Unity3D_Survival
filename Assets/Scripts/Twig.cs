using System.Collections;
using UnityEngine;

public class Twig : MonoBehaviour
{
    [SerializeField] int hp;
    [SerializeField] float destroyTime; // 이펙트 삭제 시간간
    [SerializeField] GameObject go_hit_effect_prefab; // 타격 이펙트
    [SerializeField] GameObject go_little_Twig; // 작은 나뭇가지 조각들
    // 회전값 변수
    Vector3 originRot;
    Vector3 wantedRot;
    Vector3 currentRot;
    // 필요한 사운드 이름
    [SerializeField] string hit_Sound;
    [SerializeField] string broken_Sound;

    void Start()
    {
        originRot = transform.rotation.eulerAngles;
        currentRot = originRot;
    }
    public void Damage(Transform playerTf)
    {
        hp--;

        Hit();

        StartCoroutine(HitSwayCoroutine(playerTf));

        if(hp <=0)
        {
            Destruction();
        }
    }
    void Hit()
    {
        SoundManager.Instance.PlaySE(hit_Sound);

        GameObject clone = Instantiate(go_hit_effect_prefab, 
                                        gameObject.GetComponent<BoxCollider>().bounds.center + Vector3.up * 0.5f, Quaternion.identity);
        

        Destroy(clone, destroyTime);
    }

    IEnumerator HitSwayCoroutine(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;

        Vector3 rotationDir = Quaternion.LookRotation(direction).eulerAngles;

        CheckDirection(rotationDir);

        // 피격 반대 방향으로 꺾임
        while(!CheckThreshold())
        {
            currentRot = Vector3.Lerp(currentRot, wantedRot, 0.25f);
            transform.rotation = Quaternion.Euler(currentRot);
            yield return null;
        }

        wantedRot = originRot;

        // 원래 방향으로 돌아옴
        while(!CheckThreshold())
        {
            currentRot = Vector3.Lerp(currentRot, wantedRot, 0.25f);
            transform.rotation = Quaternion.Euler(currentRot);
            yield return null;
        }
    }

    bool CheckThreshold()
    {
        if(Mathf.Abs(wantedRot.x - currentRot.x) <= 0.5f && Mathf.Abs(wantedRot.z - currentRot.z) <= 0.5f)
        {
            return true;
        }
        return false;
    }

    void CheckDirection(Vector3 rotationDir)
    {
        Debug.Log(rotationDir);

        if(rotationDir.y > 180f)
        {
            if(rotationDir.y > 300f)
            {
                wantedRot = new Vector3(-50f, 0f, -50f);
            }
            else if(rotationDir.y > 240f)
            {
                wantedRot = new Vector3(0f, 0f, -50f);
            }
            else
            {
                wantedRot = new Vector3(50f, 0f, -50f);
            }
        }
        else if (rotationDir.y <= 180f)
        {
            if(rotationDir.y < 60f)
            {
                wantedRot = new Vector3(-50f, 0f, 50f);
            }
            else if(rotationDir.y > 120f)
            {
                wantedRot = new Vector3(0f, 0f, 50f);
            }
            else
            {
                wantedRot = new Vector3(50f, 0f, 50f);
            }
        }
    }

    void Destruction()
    {
        SoundManager.Instance.PlaySE(broken_Sound);

        GameObject clone1 = Instantiate(go_little_Twig, 
                                        gameObject.GetComponent<BoxCollider>().bounds.center + Vector3.up * 0.5f, Quaternion.identity);
        GameObject clone2 = Instantiate(go_little_Twig, 
                                        gameObject.GetComponent<BoxCollider>().bounds.center + Vector3.up * 0.5f, Quaternion.identity);

        Destroy(clone1, destroyTime);
        Destroy(clone2, destroyTime);
        Destroy(gameObject);
    }
}
