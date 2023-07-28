using UnityEngine;

[System.Serializable]
public class Medal {
	public string levelName;
	[SerializeField] private int maxRounds;
	[SerializeField] private int maxSize;
	
	public bool roundsMedal { get; private set; }
	public bool sizeMedal { get; private set; }
	
	public void CheckMedals(int rounds, int size) {
		roundsMedal = roundsMedal ? true : rounds <= maxRounds;
		sizeMedal = sizeMedal ? true : size <= maxSize;
	}
}
