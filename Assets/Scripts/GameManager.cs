using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject EnemyFirst;
    public GameObject EnemySecond;
    public GameObject EnemyThird;
    private int m_EnemyCounter;
    void Start()
    {
        m_EnemyCounter = 0;
        EventManager.GetInstance().OnLockTargetDeath += EnemyDied;
    }

    void Update()
    {
        
    }
    void EnemyDied()
    {
        m_EnemyCounter++;
        switch(m_EnemyCounter)
        {
            case 1: EnemySecond.SetActive(true);  break;
            case 2: EnemyThird.SetActive(true); break;
        }    
    }
}
