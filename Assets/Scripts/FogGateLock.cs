using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogGateLock : MonoBehaviour
{
    void Start()
    {
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
    public void OnPlayerEnteringArea1()
    {
        if(gameObject.name.Contains("Area1"))
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    public void OnPlayerEnteringArea2()
    {

        if (gameObject.name.Contains("Area2"))
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    public void OnPlayerEnteringArea3()
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
