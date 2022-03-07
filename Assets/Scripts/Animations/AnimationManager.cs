using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator enemyAnimator;
    public float animationSpeed {get; set;}

    private void Start() {
        animationSpeed = 1;
    }

    public void StartAnimation(List<BattleStatus> battleStatuses) {
        StartCoroutine(CO_AnimateStatus(battleStatuses));
    }

    public IEnumerator CO_AnimateStatus(List<BattleStatus> battleStatuses) {
        Debug.Log("a");
        int i = 0;
        foreach (BattleStatus battleStatus in battleStatuses) {
            Debug.Log($"Animando round {i++}");
            IEnumerator roundCorountine = CO_AnimateRound(battleStatus);
            yield return StartCoroutine(roundCorountine);
        }
    }

    IEnumerator CO_AnimateRound(BattleStatus battleStatus) {
        float waitTime = 0f;

        if (AnimateMoveBool(battleStatus, "Defending", Commands.DEFEND, true)) yield return new WaitForSeconds((1.375f + 0.292f)*animationSpeed);

        if (AnimateMoveTrigger(battleStatus, "Charge", Commands.CHARGE)) yield return new WaitForSeconds((1.375f + 0.292f)*animationSpeed);

        if (AnimateMoveTrigger(battleStatus, "Dodge", Commands.DODGE)) waitTime = (1.375f + 0.292f)*animationSpeed;
        if (AnimateMoveTrigger(battleStatus, "Attack", Commands.ATTACK)) waitTime = (1.375f + 0.292f)*animationSpeed;
        yield return new WaitForSeconds(waitTime);

        if (AnimateMoveTrigger(battleStatus, "Heal", Commands.HEAL)) yield return new WaitForSeconds((1.375f + 0.292f)*animationSpeed);

        if (AnimateMoveBool(battleStatus, "Defending", Commands.DEFEND, false)) yield return new WaitForSeconds((1.375f + 0.292f)*animationSpeed);

        // LEVAR DANO
        /*if (battleStatus.playerAction == Commands.DODGE) playerAnimator.SetTrigger("Dodge");
        if (battleStatus.enemyAction == Commands.DODGE) enemyAnimator.SetTrigger("Dodge");
        yield return new WaitForSeconds(2f);*/
    }

    private bool AnimateMoveTrigger(BattleStatus battleStatus, string variableName, Commands action) {
        if (battleStatus.playerAction == action) playerAnimator.SetTrigger(variableName);
        if (battleStatus.enemyAction == action) enemyAnimator.SetTrigger(variableName);
        return (battleStatus.playerAction == action || battleStatus.enemyAction == action);
    }

    private bool AnimateMoveBool(BattleStatus battleStatus, string variableName, Commands action, bool value) {
        if (battleStatus.playerAction == action) playerAnimator.SetBool(variableName, value);
        if (battleStatus.enemyAction == action) enemyAnimator.SetBool(variableName, value);
        return (battleStatus.playerAction == action || battleStatus.enemyAction == action);
    }
}
