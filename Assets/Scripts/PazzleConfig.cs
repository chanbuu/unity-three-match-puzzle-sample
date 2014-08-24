using System;
using System.Collections;
using UnityEngine;

public class PazzleConfig {
	private static PazzleConfig _instance = new PazzleConfig();
	public static PazzleConfig Instance 
	{ 
		get { return _instance; }
	}
	public static PazzleConfig GetInstance()
	{
		return _instance;
	}

	private PazzleConfig()
	{
	}

	private InputType userInput = InputType.None;
	public InputType UserInput 
	{
		get { return userInput; }
		set { userInput = value; }
	}

	private GameStatus userGameStatus = GameStatus.None;
	public GameStatus UserGameStatus
	{
		get { return userGameStatus; }
		set { userGameStatus = value; }
	}

	public const int MaxBlockSeriesNum = 5;
	public const int MinBlockSeriesNum = 1;
	public const int RowCount = 4;
	public const int ColCount = 5;

	public GameObject[,] BlockPosition = new GameObject[ColCount,RowCount];
}

public enum InputType
{
	 None
	,Tap
}

public enum GameStatus
{
	 None
	,Before
	,Initialize
	,Start
	,Play
	,Drop
	,Judge
	,Finish
}