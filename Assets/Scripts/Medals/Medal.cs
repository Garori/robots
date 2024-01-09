using System;
using UnityEngine;

[System.Serializable]
public class Medal
{
	public int maxRounds { get; private set; }
	public int maxSize { get; private set; }

	public int bestRounds { get; private set; }
	public int bestSize { get; private set; }

	public bool roundsMedal { get; private set; }
	public bool sizeMedal { get; private set; }

	public Medal(int maxRounds, int maxSize)
	{
		this.maxRounds = maxRounds;
		this.maxSize = maxSize;
		roundsMedal = false;
		sizeMedal = false;
		bestRounds = int.MaxValue;
		bestSize = int.MaxValue;
	}

	public void CheckMedals(int rounds, int size)
	{
		roundsMedal = roundsMedal ? true : rounds <= maxRounds;
		sizeMedal = sizeMedal ? true : size <= maxSize;

		bestRounds = Mathf.Min(bestRounds, rounds);
		bestSize = Mathf.Min(bestSize, size);
	}

	public override string ToString()
	{
		return "Rounds Medal: " + maxRounds + "\nSize Medal: " + maxSize;
	}
}
