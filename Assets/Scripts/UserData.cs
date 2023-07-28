using System;
using UnityEngine;

public class UserData : MonoBehaviour
{
	public Medal[] medals;
	
	private Cell[] memory;
	private int _level;
	public int Level 
	{
		get => _level;
		set 
		{
			memory = Memories.GetMemory(value);
			_level = value;
		}
	}
	
	public static UserData Instance { get; private set; }	
	
	private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}
	
	public void SetMedals(int rounds, int size) {
		medals[Level].CheckMedals(rounds, size);
	}
}
