using UnityEngine;

[System.Serializable]
public class Medal
{
	public int level;
	[SerializeField] private int maxRounds;
	[SerializeField] private int maxSize;

	public bool roundsMedal { get; private set; }
	public bool sizeMedal { get; private set; }

	public Medal(int level, int maxRounds, int maxSize)
	{
		this.level = level;
		this.maxRounds = maxRounds;
		this.maxSize = maxSize;
		roundsMedal = false;
		sizeMedal = false;
	}

	public void CheckMedals(int rounds, int size)
	{
		roundsMedal = roundsMedal ? true : rounds <= maxRounds;
		sizeMedal = sizeMedal ? true : size <= maxSize;
	}
}
