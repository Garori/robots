using UnityEngine;

public static class Memories
{
	private static Cell[][] memories;

	static Memories()
	{
		memories = new Cell[3][];
		// memories[0] = new Cell[]{
		// 	new WhileCell(new EqualsCell(Commands.ZERO, Commands.ZERO), 4),
		// 	new IfCell(new EvenCell(Commands.ROUND),1),
		// 	new ActionCell(Commands.ATTACK),
		// 	new ElseCell(new EvenCell(Commands.ROUND),1),
		// 	new ActionCell(Commands.DEFEND),
		// 	new EndCell(-6)
		// };

		// memories[0] = new Cell[]{
		// 	new WhileCell(new EqualsCell(Commands.ZERO, Commands.ZERO), 1),
		// 	new ActionCell(Commands.ATTACK),
		// 	new EndCell(-3)
		// };

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
			new IfCell (new GreaterCell(Commands.ENEMY_ACTUAL_HEALTH, Commands.ENEMY_MAX_HEALTH_HALF), 1),
			new ActionCell(Commands.CHARGE),
			new EndCell(),
			new ElseCell (new GreaterCell(Commands.ENEMY_ACTUAL_HEALTH, Commands.ENEMY_MAX_HEALTH_HALF), 1),
			new ActionCell(Commands.ATTACK),
			new EndCell()
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
		};
	}

	public static Cell[] GetMemory(int index)
	{
		return memories[index];
	}
}
