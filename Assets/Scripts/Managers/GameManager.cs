using UnityEngine;

//A simple class that is responsible for the spawn of the enemies in the game. If the game was bigger, this class would have lot more functionality in it. It is a humble simple class for now.
//It basically sets the next enemy active whenever an enemy dies.
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
