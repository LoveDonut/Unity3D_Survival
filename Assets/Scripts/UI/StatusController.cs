using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // 체력
    [SerializeField] int hp;
    int currentHp;

    // 스태미나
    [SerializeField] int sp;
    int currentSp;

    // 스태미나 증가량
    [SerializeField] int spIncreaseSpeed;

    // 스태미나 재회복 딜레이
    [SerializeField] int spRechargeTime;
    int currentSpRechargeTime;
    bool spUsed; // 스태미나 감소 여부
    
    // 방어력
    [SerializeField] int dp;
    int currentDp;

    // 배고픔
    [SerializeField] int hungry;
    int currentHungry;
    
    // 배고픔이 줄어드는 속도
    [SerializeField] int hungryDecreaseTime;
    int currentHungryDecreaseTime;

    // 목마름
    [SerializeField] int thirsty;
    int currentThirsty;

    // 목마름이 줄어드는 속도
    [SerializeField] int thirstyDecreaseTime;
    int currentThirstyDecreaseTime;

    // 만족도
    [SerializeField] int satisfy;
    int currentSatisfy;

    [SerializeField] Image[] images_Gauge;

    const int HP = 0, DP = 1, SP = 2, HUNGRY = 3, THIRSTY = 4, SATISFY = 5;

    void Start()
    {
        currentHp = hp;
        currentDp = dp;
        currentSp = sp;
        currentHungry = hungry;
        currentThirsty = thirsty;
        currentSatisfy = satisfy;
    }

    void Update()
    {
        Hungry();
        Thirsty();
        GaugeUpdate();
        SPRechargeTime();
        SPRecover();
    }

    void SPRechargeTime()
    {
        if(currentSpRechargeTime < spRechargeTime)
        {
            currentSpRechargeTime++;
        }
        else
        {
            spUsed = false;
        }
    }
    void SPRecover()
    {
        if(!spUsed && currentSp < sp)
        {
            currentSp += spIncreaseSpeed;
        }
    }
    void Hungry()
    {
        if(currentHungry > 0)
        {
            if(currentHungryDecreaseTime <= hungryDecreaseTime)
            {
                currentHungryDecreaseTime++;
            }
            else
            {
                currentHungry--;
                currentHungryDecreaseTime = 0;
            }
        }
        else
        {
            Debug.Log("배고픔 수치가 0이 되었습니다");
        }
    }

    void Thirsty()
    {
        if(currentThirsty > 0)
        {
            if(currentThirstyDecreaseTime <= thirstyDecreaseTime)
            {
                currentThirstyDecreaseTime++;
            }
            else
            {
                currentThirsty--;
                currentThirstyDecreaseTime = 0;
            }
        }
        else
        {
            Debug.Log("목마름 수치가 0이 되었습니다");
        }
    }

    void GaugeUpdate()
    {
        images_Gauge[HP].fillAmount = (float)currentHp / hp;
        images_Gauge[DP].fillAmount = (float)currentDp / dp;
        images_Gauge[SP].fillAmount = (float)currentSp / sp;
        images_Gauge[HUNGRY].fillAmount = (float)currentHungry / hungry;
        images_Gauge[THIRSTY].fillAmount = (float)currentThirsty / thirsty;
        images_Gauge[SATISFY].fillAmount = (float)currentSatisfy / satisfy;
    }

    public void DecreaseStamina(int count)
    {
        spUsed = true;
        currentSpRechargeTime = 0;

        if(currentSp - count > 0)
        {
            currentSp -= count;
        }
        else
        {
            currentSp = 0;
        }
    }
}
