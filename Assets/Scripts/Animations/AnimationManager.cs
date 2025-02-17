using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private UnityArmatureComponent playerArmature;
    [SerializeField] private UnityArmatureComponent enemyArmature;

    private string[] GetAnimations(Commands command, bool isHit, int attackOrder)
    {
        string[] animations = new string[] { "Idle", "Idle", "Idle", "Idle" };
        int attackPosition = attackOrder + 1;
        int defendPosition = 2 - attackOrder;
        switch (command)
        {
            case Commands.DEFEND:
                animations[defendPosition] = isHit ? "Defense_onHit" : "Defense";
                return animations;
            case Commands.DODGE:
                animations[defendPosition] = "Dodge";
                return animations;
            case Commands.CHARGE:
                animations[0] = "Charge";
                if (isHit) animations[defendPosition] = "Damaged";
                return animations;
            case Commands.HEAL:
                animations[3] = "Heal";
                if (isHit) animations[defendPosition] = "Damaged";
                return animations;
            case Commands.ATTACK:
                animations[attackPosition] = "Attack_punch";
                if (isHit) animations[defendPosition] = "Damaged";
                return animations;
            default:
                if (isHit) animations[defendPosition] = "Damaged";
                return animations;
        }
    }

    public IEnumerator CO_PlayAnimation(UnityArmatureComponent armature, string animationName)
    {
        DragonBones.AnimationState animationState = armature.animation.Play(animationName, 1);
        yield return new WaitForSeconds(animationState.totalTime);
        armature.animation.Play("Idle", 0);
    }

    public IEnumerator CO_StartAnimation(List<BattleStatus> battleStatuses)
    {
        foreach (BattleStatus battleStatus in battleStatuses)
        {
            string[] playerAnimations = GetAnimations(battleStatus.playerAction, battleStatus.playerHit, 0);
            string[] enemyAnimations = GetAnimations(battleStatus.enemyAction, battleStatus.enemyHit, 1);
            // Debug.Log("Starting animations");

            for (int i = 0; i < playerAnimations.Length; i++)
            {
                if (playerAnimations[i] == "Idle" && enemyAnimations[i] == "Idle") continue;
                // Debug.Log("Player animation " + i + ": " + playerAnimations[i]);
                // Debug.Log("Enemy animation " + i + ": " + enemyAnimations[i]);
                Coroutine playerCoroutine = StartCoroutine(CO_PlayAnimation(playerArmature, playerAnimations[i]));
                Coroutine enemyCoroutine = StartCoroutine(CO_PlayAnimation(enemyArmature, enemyAnimations[i]));

                yield return playerCoroutine;
                yield return enemyCoroutine;

                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public void StartAnimation(List<BattleStatus> battleStatuses)
    {
        StartCoroutine(CO_StartAnimation(battleStatuses));
    }

    public void SkipAnimation()
    {
        StopAllCoroutines();
        playerArmature.animation.Play("Idle", 0);
        enemyArmature.animation.Play("Idle", 0);
    }
}
