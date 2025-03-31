using System.Collections;
using UnityEngine;

public class TreeComponent : MonoBehaviour
{
    [SerializeField] GameObject[] go_treePieces; // 깍일 나무 조각들
    [SerializeField] GameObject go_treeCenter;
    [SerializeField] GameObject go_Log_Prefabs; // 통나무
    [SerializeField] float force; // 쓰러질 때의 랜덤으로 가해질 힘의 세기
    [SerializeField] GameObject go_ChildTree; // 자식 나무
    [SerializeField] GameObject go_hit_effect_prefab; // 파편
    [SerializeField] float debrisDestroyTime; // 파편 제거 시간
    [SerializeField] float destrtoyTime; // 나무 제거 시간
    // 부모 트리 파괴되면 콜라이더 비활성화
    [SerializeField] CapsuleCollider parentCol;
    // 자식 트리 쓰러질 때 필요한 컴포넌트 활성화 및 중력 활성화
    [SerializeField] CapsuleCollider childCol;
    [SerializeField] Rigidbody childRigid; 
    // 필요한 사운드
    [SerializeField] string chop_sound;
    [SerializeField] string falldown_sound;
    [SerializeField] string logChange_sound;

    public void Chop(Vector3 pos, float angleY)
    {
        Hit(pos);

        AngleCalc(angleY);

        if(CheckTreePieces())
        {
            return;
        }

        FallDownTree();
    }

    // 적중 이펙트
    void Hit(Vector3 pos)
    {
        SoundManager.Instance.PlaySE(chop_sound);

    }

    void AngleCalc(float angleY)
    {
        Debug.Log(angleY);
        if(0f < angleY && angleY <= 70f)
        {
            DestroyPiece(2);
        }
        if(70f < angleY && angleY <= 140f)
        {
            DestroyPiece(3);
        }
        if(140f < angleY && angleY <= 210f)
        {
            DestroyPiece(4);
        }
        if(210f < angleY && angleY <= 280f)
        {
            DestroyPiece(0);
        }
        if(280f < angleY && angleY <= 360f)
        {
            DestroyPiece(1);
        }
    }

    void DestroyPiece(int num)
    {
        if(go_treePieces[num].gameObject != null)
        {
            GameObject clone = Instantiate(go_hit_effect_prefab, go_treePieces[num].transform.position, Quaternion.Euler(Vector3.zero));
            Destroy(clone, debrisDestroyTime);
            Destroy(go_treePieces[num].gameObject);
        }
    }

    bool CheckTreePieces()
    {
        for(int i=0; i<go_treePieces.Length; i++)
        {
            if(go_treePieces[i] != null)
            {
                return true;
            }
        }
        return false;
    }

    void FallDownTree()
    {
        SoundManager.Instance.PlaySE(falldown_sound);
        Destroy(go_treeCenter);

        parentCol.enabled = false;
        childCol.enabled = true;
        childRigid.useGravity = true;

        childRigid.AddForce(Random.Range(-force,force), 0f, Random.Range(-force,force));
        StartCoroutine(LogCoroutine());
    }
    IEnumerator LogCoroutine()
    {
        yield return new WaitForSeconds(destrtoyTime);

        SoundManager.Instance.PlaySE(logChange_sound);
        Instantiate(go_Log_Prefabs, go_ChildTree.transform.position + (go_ChildTree.transform.up * 3f), Quaternion.LookRotation(go_ChildTree.transform.up));
        Instantiate(go_Log_Prefabs, go_ChildTree.transform.position + (go_ChildTree.transform.up * 6f), Quaternion.LookRotation(go_ChildTree.transform.up));
        Instantiate(go_Log_Prefabs, go_ChildTree.transform.position + (go_ChildTree.transform.up * 9f), Quaternion.LookRotation(go_ChildTree.transform.up));

        Destroy(go_ChildTree.gameObject);
    }
    void Destruction()
    {

    }
}
