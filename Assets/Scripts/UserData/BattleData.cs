using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleData
{
    public static int selectedLevel { get; set; }
    public static bool isTest { get; set; }
    public static CellsContainer levelMemory { get; set; }
    public static List<List<Commands>> levelCommands { get; set; }
}
