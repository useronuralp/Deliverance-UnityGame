using UnityEngine;
using System;

//The class that stores all the events in the game.
public class EventManager : MonoBehaviour
{
    private static EventManager s_Instance;
    //Events-------------------------------
    //General / tutorial events 
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

    //Tutorial pause menu events.
    public event Action OnLockOnButtonPressed;
    public event Action OnOffenseButtonPressed;
    public event Action OnDefenseButtonPressed;
    public event Action OnResourcesButtonPressed;
    public event Action OnComboButtonPressed;
    public event Action OnParryButtonPressed;
    public event Action OnGameResumed;
    public event Action OnSeperateWindowClosed;

    //Particle Sytem events.
    public event Action OnPlayerDisablesLeaves;
    public event Action OnPlayerEnablesLeaves;

    //Fog gate lock/unlock events.
    public event Action OnPlayerEnteringArea1;
    public event Action OnPlayerEnteringArea2;
    public event Action OnPlayerEnteringArea3;

    //WindSound enable/disable events.
    public event Action OnPlayerEnablesWindSound;
    public event Action OnPlayerDisablesWindSound;

    //Main menu event.
    public event Action OnPlayButtonPressed;

    //Fight music event.
    public event Action OnTriggerFightMusicFirstTime;

    //AI notices player event.
    public event Action OnEnemySeesPlayer;
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
    public void LockOnButtonPressed()
    {
        OnLockOnButtonPressed?.Invoke();
    }
    public void OffenseButtonPressed()
    {
        OnOffenseButtonPressed?.Invoke();
    }
    public void DefenseButtonPressed()
    {
        OnDefenseButtonPressed?.Invoke();
    }
    public void ResourcesButtonPressed()
    {
        OnResourcesButtonPressed?.Invoke();
    }
    public void ComboButtonPressed()
    {
        OnComboButtonPressed?.Invoke();
    }
    public void ParryButtonPressed()
    {
        OnParryButtonPressed?.Invoke();
    }
    public void GameResumed()
    {
        OnGameResumed?.Invoke();
    }
    public void SeperateWindowClosed()
    {
        OnSeperateWindowClosed?.Invoke();
    }
    public void DisableParticles()
    {
        OnPlayerDisablesLeaves?.Invoke();
    }
    public void EnableParticles()
    {
        OnPlayerEnablesLeaves?.Invoke();
    }
    public void LockArea1()
    {
        OnPlayerEnteringArea1?.Invoke();
    }
    public void LockArea2()
    {
        OnPlayerEnteringArea2?.Invoke();
    }
    public void LockArea3()
    {
        OnPlayerEnteringArea3?.Invoke();
    }
    public void EnableWindSound()
    {
        OnPlayerEnablesWindSound?.Invoke();
    }
    public void DisableWindSound()
    {
        OnPlayerDisablesWindSound?.Invoke();
    }
    public void PlayButtonPressed()
    {
        OnPlayButtonPressed?.Invoke();
    }
    public void TriggerFightMusicFirstTime()
    {
        OnTriggerFightMusicFirstTime?.Invoke();
    }
    public void EnemySawPlayer()
    {
        OnEnemySeesPlayer?.Invoke();
    }
    //Singleton getter---------------------
    public static EventManager GetInstance()
    {
        return s_Instance;
    }
}
