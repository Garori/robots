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
