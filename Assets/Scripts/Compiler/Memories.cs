using UnityEngine;

public static class Memories
{
	private static CellsContainer[] memories;
	private static CellsContainer[] customMemories;
	public static int memoriesLength { get { return memories.Length; } }
	public static int customMemoriesLength { get { return customMemories.Length; } }

	static Memories()
	{
		memories = GetFiles("Memories");
		customMemories = GetFiles("CustomMemories");
		/*
		memories = new Cell[3][];
		memories[0] = new Cell[]{
			new WhileCell(new EqualsCell(Commands.ZERO, Commands.ZERO), 4),
			new IfCell(new EvenCell(Commands.ROUND),1),
			new ActionCell(Commands.ATTACK),
			new ElseCell(new EvenCell(Commands.ROUND),1),
			new ActionCell(Commands.DEFEND),
			new EndCell(-6)
		};

		memories[0] = new Cell[]{
			new WhileCell(new EqualsCell(Commands.ZERO, Commands.ZERO), 1),
			new ActionCell(Commands.ATTACK),
			new EndCell(-3)
		};

		memories[0] = new Cell[]{
			new WhileCell(new EqualsCell(Commands.ZERO, Commands.ZERO), 5),
			new ActionCell(Commands.ATTACK),
			new ActionCell(Commands.DEFEND),
			new ActionCell(Commands.CHARGE),
			new ActionCell(Commands.DODGE),
			new ActionCell(Commands.HEAL),
			new EndCell(-7)
		};

		memories[1] = new Cell[]{
			new WhileCell (new TrueCell(), 5),
			new IfCell (new GreaterCell(Commands.ENEMY_ACTUAL_HEALTH, Commands.ENEMY_MAX_HEALTH_HALF), 2),
			new ActionCell(Commands.CHARGE),
			new ElseCell (1),
			new ActionCell(Commands.ATTACK),
			new EndCell(),
			new EndCell(-7)
		};

		memories[2] = new Cell[]{
			new IfCell (new NotEqualsCell(Commands.ENEMY_ACTUAL_SHIELD, Commands.ZERO), 5),
			new ActionCell(Commands.DEFEND),
			new ActionCell(Commands.DEFEND),
			new ActionCell(Commands.CHARGE),
			new ActionCell(Commands.CHARGE),
			new ActionCell(Commands.ATTACK),
			new EndCell(),
			new IfCell (new EqualsCell(Commands.ENEMY_ACTUAL_HEALTH, Commands.ENEMY_MAX_HEALTH), 5),
			new ActionCell(Commands.DODGE),
			new ActionCell(Commands.CHARGE),
			new ActionCell(Commands.CHARGE),
			new ActionCell(Commands.ATTACK),
			new ActionCell(Commands.ATTACK),
			new EndCell(),
			new ActionCell(Commands.HEAL),
			new ActionCell(Commands.HEAL),
			new ActionCell(Commands.CHARGE),
			new ActionCell(Commands.CHARGE),
			new ActionCell(Commands.ATTACK)
		};*/
	}

	public static CellsContainer[] GetFiles(string folderName)
	{
		string[] fileNames = System.IO.Directory.GetFiles(folderName);
		CellsContainer[] memories = new CellsContainer[fileNames.Length];

		for (int i = 0; i < fileNames.Length; i++)
		{
			memories[i] = CellsContainer.Deserialize(fileNames[i]);
		}

		return memories;
	}

	public static CellsContainer GetMemory(int level)
	{
		if (level < memoriesLength)
		{
			return memories[level];
		}
		else
		{
			return customMemories[level - memoriesLength];
		}
	}
}
