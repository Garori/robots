using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator enemyAnimator;

    public IEnumerator AnimBattle(List<BattleStatus> battleStatuses) {
        foreach (BattleStatus battleStatus in battleStatuses) {
            IEnumerator roundCorountine = AnimateRound(battleStatus);
            yield return StartCoroutine(roundCorountine);
        }
    }

    IEnumerator AnimateRound(BattleStatus battleStatus) {
        if (battleStatus.playerAction == Commands.DEFEND) playerAnimator.SetBool("Defending", true);
        if (battleStatus.enemyAction == Commands.DEFEND) enemyAnimator.SetBool("Defending", true);
        yield return null;

        if (battleStatus.playerAction == Commands.DODGE) playerAnimator.SetTrigger("Dodge");
        if (battleStatus.enemyAction == Commands.DODGE) enemyAnimator.SetTrigger("Dodge");
        yield return null;

        if (battleStatus.playerAction == Commands.CHARGE) playerAnimator.SetTrigger("Charge");
        if (battleStatus.enemyAction == Commands.CHARGE) enemyAnimator.SetTrigger("Charge");
        yield return null;

        if (battleStatus.playerAction == Commands.ATTACK) playerAnimator.SetTrigger("Attack");
        if (battleStatus.enemyAction == Commands.ATTACK) enemyAnimator.SetTrigger("Attack");
        yield return null;

        // LEVAR DANO
        /*if (battleStatus.playerAction == Commands.DODGE) playerAnimator.SetTrigger("Dodge");
        if (battleStatus.enemyAction == Commands.DODGE) enemyAnimator.SetTrigger("Dodge");
        yield return null;*/

        if (battleStatus.playerAction == Commands.HEAL) playerAnimator.SetTrigger("Heal");
        if (battleStatus.enemyAction == Commands.HEAL) enemyAnimator.SetTrigger("Heal");
        yield return null;

        if (battleStatus.playerAction == Commands.DEFEND) playerAnimator.SetBool("Defending", false);
        if (battleStatus.enemyAction == Commands.DEFEND) enemyAnimator.SetBool("Defending", false);
        yield return null;
    }
}
