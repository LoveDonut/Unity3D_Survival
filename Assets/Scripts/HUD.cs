using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] GunController gunController;
    Gun currentGun;

    [SerializeField] GameObject goBulletHUD; // 필요하면 HUD 호출, 필요없으면 HUD 비활성화

    [SerializeField] TextMeshProUGUI[] textBullet; // 총알 개수 텍스트에 반영

    void Update()
    {
        CheckBullet();
    }

    void CheckBullet()
    {
        currentGun = gunController.GetGun();
        textBullet[0].SetText(currentGun.carryBulletCount.ToString());
        textBullet[1].SetText(currentGun.reloadBulletCount.ToString());
        textBullet[2].SetText(currentGun.currentBulletCount.ToString());
    }
}
