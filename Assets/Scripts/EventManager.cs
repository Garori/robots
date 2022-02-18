using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {
    public delegate void InsertBlock(GameObject block, GameObject line);
    public static event InsertBlock BlockEnter;

    public delegate void RemoveBlock(GameObject block);
    public static event RemoveBlock BlockExit;

    public static void onBlockEnter(GameObject block, GameObject line) {
        if (BlockEnter != null) BlockEnter(block, line);
    }

    public static void onBlockExit(GameObject block) {
        if (BlockExit != null) BlockExit(block);
    }
}
