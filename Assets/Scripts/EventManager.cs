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
    public event Action OnPlayerStepsIntoTalkRange;
    public event Action OnPlayerLeavingTalkRange;
    public event Action OnRescuerEnteredStopTrigger;
    public event Action OnPlayerStartsSpeakingWithRescuer;
    public event Action OnPlayerCompletingDialogue;
    public event Action OnPlayerEntersDoorUnlockRange;
    public event Action OnPlayerLeavesDoorUnlockRange;
    public event Action OnLockTargetDeath;
    public event Action OnPlayerDeath;
    public event Action OnPlayerEnteringTutorialTrigger;
    public event Action OnPlayerClosesTutorial;
    public event Action OnPlayerEnteredSprintTrigger;
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
    public void PlayerStepsIntoTalkRange()
    {
        OnPlayerStepsIntoTalkRange?.Invoke();
    }
    public void PlayerLeftTalkRange()
    {
        OnPlayerLeavingTalkRange?.Invoke();
    }
    public void RescuerEnteredStopTrigger()
    {
        OnRescuerEnteredStopTrigger?.Invoke();
    }
    public void PlayerStartsSpeakingWithRescuer()
    {
        OnPlayerStartsSpeakingWithRescuer?.Invoke();
    }
    public void PlayerCompletedDialogue()
    {
        OnPlayerCompletingDialogue?.Invoke();
    }
    public void PlayerEnteredDoorUnlockRange()
    {
        OnPlayerEntersDoorUnlockRange?.Invoke();
    }
    public void PlayerLeftDoorUnlockRange()
    {
        OnPlayerLeavesDoorUnlockRange?.Invoke();
    }
    public void LockTargetDied()
    {
        OnLockTargetDeath?.Invoke();
    }
    public void PlayerDied()
    {
        OnPlayerDeath?.Invoke();
    }
    public void PlayerEnteredTutorialTrigger()
    {
        OnPlayerEnteringTutorialTrigger?.Invoke();
    }
    public void PlayerClosedTutorial()
    {
        OnPlayerClosesTutorial?.Invoke();
    }
    public void PlayerEnteredSprintTrigger()
    {
        OnPlayerEnteredSprintTrigger?.Invoke();
    }
    //Singleton getter---------------------
    public static EventManager GetInstance()
    {
        return s_Instance;
    }
}
