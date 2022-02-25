using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {
    /*
        ====================
                PANEL
        ====================
    */
    public delegate void InsertBlock(GameObject block, GameObject line);
    public static event InsertBlock BlockEnter;

    public delegate void RemoveBlock(GameObject block);
    public static event RemoveBlock BlockExit;

    public delegate void InsertComparator(GameObject comparator, GameObject blockCondition);
    public static event InsertComparator ComparatorEnter;

    public delegate void RemoveComparator(GameObject comparator);
    public static event RemoveComparator ComparatorExit;

    public delegate void InsertVariable(GameObject variable, GameObject conditionVariable);
    public static event InsertVariable VariableEnter;

    public delegate void RemoveVariable(GameObject variable);
    public static event RemoveVariable VariableExit;

    public static void onBlockEnter(GameObject block, GameObject line) {
        if (BlockEnter != null) BlockEnter(block, line);
    }

    public static void onBlockExit(GameObject block) {
        if (BlockExit != null) BlockExit(block);
    }

    public static void onComparatorEnter(GameObject comparator, GameObject blockCondition) {
        if (ComparatorEnter != null) ComparatorEnter(comparator, blockCondition);
    }

    public static void onComparatorExit(GameObject comparator) {
        if (ComparatorExit != null) ComparatorExit(comparator);
    }

    public static void onVariableEnter(GameObject variable, GameObject conditionVariable) {
        if (VariableEnter != null) VariableEnter(variable, conditionVariable);
    }

    public static void onVariableExit(GameObject variable) {
        if (VariableExit != null) VariableExit(variable);
    }
}
