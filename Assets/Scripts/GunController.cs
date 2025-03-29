using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    [SerializeField] Gun currentGun; // 현재 장착된 Gun형 타입 무기
    float currentFireRate; // 현재 연사속도
    AudioSource audioSource; // 효과음 재생생
    [SerializeField] Vector3 originPos; // 본래 포지션 값

    RaycastHit hitInfo; // 레이저 충돌 정보 받아옴
    [SerializeField] Camera theCam; // 카메라 정보
    [SerializeField] GameObject hitFXPrefab; // 피격 이펙트
    Crosshair crosshair;
    // 상태 변수
    bool isReload = false; // 재장전 중인지 판별
    bool isShooting = false; // 발사 중인지 판별
    bool isFindSightMode; // 정조준 모드인지 판별
    void Awake()
    {
        crosshair = FindAnyObjectByType<Crosshair>();
    }
    void Start()
    {
        originPos = Vector3.zero;
        audioSource = GetComponent<AudioSource>();

        InputManager.Subscribe("Fire", StartFire, InputActionPhase.Started);
        InputManager.Subscribe("Fire", CancelFire, InputActionPhase.Canceled);
        InputManager.Subscribe("Reload", TryReload);
        InputManager.Subscribe("FineSight", TryFineSight);
    }

    void OnDestroy()
    {
        InputManager.Subscribe("Fire", StartFire, InputActionPhase.Started);
        InputManager.Subscribe("Fire", CancelFire, InputActionPhase.Canceled);
        InputManager.Unsubscribe("Reload", TryReload);
        InputManager.Unsubscribe("FineSight", TryFineSight);
    }

    void Update()
    {
        GunFireRateCalc();
        Fire();
    }

    // 연사속도 계산
    void GunFireRateCalc()
    {
        if(currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime;
        }
    }

    void StartFire(InputAction.CallbackContext ctx)
    {
        if(currentFireRate <= 0 && !isReload)
        {
            isShooting = true;
        }
    }

    void CancelFire(InputAction.CallbackContext ctx)
    {
        isShooting = false;
    }
    // 재장전 시도
    void TryReload(InputAction.CallbackContext ctx)
    {
        if(!isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }
    // 정조준 시도
    void TryFineSight(InputAction.CallbackContext ctx)
    {
        if(!isReload)
        {
            FineSight();
        }
    }
    // 정조준 취소
    public void CancelFineSight()
    {
        if(isFindSightMode)
        {
            FineSight();
        }
    }
    // 발사 전 계산
    void Fire()
    {
        if(currentFireRate > 0 || isReload || !isShooting) return;
        if(currentGun.currentBulletCount > 0)
        {
            Shoot();
        }
        else
        {
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }
    // 발사 후 계산산
    void Shoot()
    {
        crosshair.FireAnimation();
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; // 연사 속도 재계산
        PlaySE(currentGun.fireSound);
        currentGun.muzzleFlash.Play();
        Hit();
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
    }

    void Hit()
    {
        if(Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hitInfo, currentGun.range))
        {
            var clone = Instantiate(hitFXPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2f);
        }
    }
    // 정조준 로직 가동
    void FineSight()
    {
        isFindSightMode = !isFindSightMode;
        currentGun.anim.SetBool("FineSightMode", isFindSightMode);

        if(isFindSightMode)
        {
            StopAllCoroutines();
            StartCoroutine(FindSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FindSightDeActivateCoroutine());
        }
    }
    // 재장전
    IEnumerator ReloadCoroutine()
    {
        if(currentGun.carryBulletCount > 0)
        {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");

            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if(currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }

            isReload = false;
        }
        else
        {
            Debug.Log("소유한 총알이 없습니다.");
        }
    }
    // 정조준 활성화
    IEnumerator FindSightActivateCoroutine()
    {
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }
    // 정조준 비활성화
    IEnumerator FindSightDeActivateCoroutine()
    {
        while(currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }
    // 반동 코루틴
    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoliBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);

        if(!isFindSightMode)
        {
            currentGun.transform.localPosition = originPos;

            // 반동 시작
            while(currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoliBack, 0.4f);
                yield return null;
            }

            // 원위치
            while(currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            // 반동 시작
            while(currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            // 원위치
            while(currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }
    // 효과음 재생
    void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public Gun GetGun()
    {
        return currentGun;
    }
}
