using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    private static EventManager s_Instance;
    //Events-------------------------------
    public event Action OnAIStartsWandering;
    public event Action OnAIStopsWandering;
    public event Action OnAIIsAttacking;
    public event Action OnAIIsNotAttacking;
    public event Action<GameObject> OnPlayerLockedOnToTarget;
    public event Action OnPlayerReleasedLockOnTarget;


    private void Awake()
    {
        s_Instance = this;
    }



    //Event Trigger functions--------------
    public void AIWantsToWander()
    {
        OnAIStartsWandering?.Invoke();
    }
    public void AIWantsToAttackPlayer()
    {
        OnAIStopsWandering?.Invoke();
    }
    public void AIIsAttacking()
    {
        OnAIIsAttacking?.Invoke();
    }
    public void AIIsNotAttacking()
    {
        OnAIIsNotAttacking?.Invoke();
    }
    public void PlayerLockedOnToTarget(GameObject lockTarget)
    {
        OnPlayerLockedOnToTarget?.Invoke(lockTarget);
    }
    public void PlayerReleasedLockOnTarget()
    {
        OnPlayerReleasedLockOnTarget?.Invoke();
    }
    //Singleton getter---------------------
    public static EventManager GetInstance()
    {
        return s_Instance;
    }
}
