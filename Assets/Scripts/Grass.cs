using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField] int hp; // 풀 체력 (보통 1)
    [SerializeField] float destroyTime; // 이펙트 제거 시간
    [SerializeField] float force; 
    [SerializeField] GameObject go_hit_effect_prefab; // 타격 효과

    Rigidbody[] rigidbodies;
    BoxCollider[] boxColiders;

    [SerializeField] string hit_sound;

    void Start()
    {
        rigidbodies = transform.GetComponentsInChildren<Rigidbody>();
        boxColiders = transform.GetComponentsInChildren<BoxCollider>();
    }

    public void Damage()
    {
        Debug.Log("Hit!");
        hp--;

        Hit();

        if(hp <=0)
        {
            Destruction();
        }
    }

    void Hit()
    {
            SoundManager.Instance.PlaySE(hit_sound);

            var clone = Instantiate(go_hit_effect_prefab, transform.position + Vector3.up, Quaternion.identity);

            Destroy(clone, destroyTime);
    }

    void Destruction()
    {
        for(int i=0; i<rigidbodies.Length; i++)
        {
            rigidbodies[i].useGravity = true;
            rigidbodies[i].AddExplosionForce(force, transform.position, 1f);
            boxColiders[i].enabled = true;
        }

        Destroy(gameObject, destroyTime);
    }
}
