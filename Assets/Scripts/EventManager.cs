using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    /*
        ====================
                PANEL
        ====================
    */
    public delegate void InsertBlock(BlockController block, GameObject line);
    public static event InsertBlock BlockEnter;

    public delegate void RemoveBlock(BlockController block);
    public static event RemoveBlock BlockExit;

    public delegate void InsertCode(BlockController code, GameObject line);
    public static event InsertCode CodeEnter;

    public delegate void RemoveCode(BlockController code);
    public static event RemoveCode CodeExit;

    public delegate void InsertComparator(ComparatorController comparator, BlockSlotController blockCondition);
    public static event InsertComparator ComparatorEnter;

    public delegate void RemoveComparator(ComparatorController comparator);
    public static event RemoveComparator ComparatorExit;

    public delegate void InsertVariable(VariableController variable, BlockSlotController conditionVariable);
    public static event InsertVariable VariableEnter;

    public delegate void RemoveVariable(VariableController variable);
    public static event RemoveVariable VariableExit;

    public static void onBlockEnter(BlockController block, GameObject line)
    {
        if (BlockEnter != null) BlockEnter(block, line);
    }

    public static void onBlockExit(BlockController block)
    {
        if (BlockExit != null) BlockExit(block);
    }

    public static void onCodeEnter(BlockController block, GameObject line)
    {
        if (CodeEnter != null) CodeEnter(block, line);
    }

    public static void onCodeExit(BlockController block)
    {
        if (CodeExit != null) CodeExit(block);
    }

    public static void onComparatorEnter(ComparatorController comparator, BlockSlotController blockCondition)
    {
        if (ComparatorEnter != null) ComparatorEnter(comparator, blockCondition);
    }

    public static void onComparatorExit(ComparatorController comparator)
    {
        if (ComparatorExit != null) ComparatorExit(comparator);
    }

    public static void onVariableEnter(VariableController variable, BlockSlotController conditionVariable)
    {
        if (VariableEnter != null) VariableEnter(variable, conditionVariable);
    }

    public static void onVariableExit(VariableController variable)
    {
        if (VariableExit != null) VariableExit(variable);
    }
}
