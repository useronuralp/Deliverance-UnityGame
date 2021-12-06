using UnityEngine;

//This class is responsible for locking / unlocking fog gates when player enters the corresponding fight areas.
public class FogGateLock : MonoBehaviour
{
    void Start()
    {
        //Subscribe to the events. The following events are fired by triggers. This class is a listener. 
        EventManager.GetInstance().OnLockTargetDeath += OnAIDeath;
        EventManager.GetInstance().OnPlayerEnteringArea1 += OnPlayerEnteringArea1;
        EventManager.GetInstance().OnPlayerEnteringArea2 += OnPlayerEnteringArea2;
        EventManager.GetInstance().OnPlayerEnteringArea3 += OnPlayerEnteringArea3;
    }
    public void OnAIDeath()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    public void OnPlayerEnteringArea1() //Enable Area1 gates.
    {
        if(gameObject.name.Contains("Area1"))
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    public void OnPlayerEnteringArea2()//Enable Area2 gates.
    {

        if (gameObject.name.Contains("Area2"))
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    public void OnPlayerEnteringArea3()//Enable Area3 gates.
    {

        if (gameObject.name.Contains("Area3"))
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
