using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    float HP, maxHP;
    public Image healthSlider;

    // Start is called before the first frame update
    void Start()
    {
        HP = maxHP = GameManager.instance.playerHP;
        updatePlayerUI();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Damage(1);
            updatePlayerUI();
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(HP+1 <= maxHP)
            HP++;
            updatePlayerUI();
        }
    }

  
    public void Damage(float damage)
    {
        HP -= damage;
        StartCoroutine(flashScreenDamage());
        if (HP <= 0f)
        {
            GameManager.instance.YouLose();
        }
    }

    IEnumerator flashScreenDamage()
    {
        GameManager.instance.damageFlash.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.damageFlash.SetActive(false);
    }

    public void updatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP/maxHP;
        //GameManager.instance.playerHPBar.fillAmount = Mathf.Lerp(GameManager.
        //    instance.playerHPBar.fillAmount, (float)HP/maxHP, Time.deltaTime);
    }
}
