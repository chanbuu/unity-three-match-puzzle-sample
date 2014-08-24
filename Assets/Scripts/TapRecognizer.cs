using UnityEngine;
using System.Collections;

public class TapRecognizer : SingletonMonoBehaviour<TapRecognizer> {

	public enum Direction
	{
		 None = 0
		,Up
		,UpperRightUpper
		,UpperRightLower
		,Right
		,LowerRightUpper
		,LowerRightLower
		,Down
		,LowerLeftLower
		,LowerLeftUpper
		,Left
		,UpperLeftLower
		,UpperLeftUpper
	}

	private Direction moveDirection = Direction.None;
	public Direction MoveDirection 
	{
		get { return moveDirection; }
		set { moveDirection = value; }
	}

	public enum TouchStatus
	{
		 None = 0
		,Start
		,During
		,Finish
	}

	private TouchStatus fingerTouchStatus = TouchStatus.None;
	public TouchStatus FingerTouchStatus
	{
		get{ return fingerTouchStatus; } 
		set{ fingerTouchStatus = value; }
	}


	private bool isFlick;
	private bool isClick;
	public Vector3 TouchWorldPos{ get; set; }
	private Vector3 touchStartPos;
	private Vector3 touchEndPos;

	private void Update()
	{
		if (Input.GetMouseButtonDown (0)) 
		{
//			Debug.Log ("Mouse Button Down.");
			isFlick = true;
			fingerTouchStatus = TouchStatus.Start;
			TouchWorldPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			touchStartPos = Input.mousePosition;
			Invoke ("FlickOff", 0.3f);
			return;
		}
		if (Input.GetMouseButton (0)) 
		{
//			Debug.Log ("Mouse Button Downing.");
			TouchWorldPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			touchEndPos = Input.mousePosition;
			if (touchStartPos != touchEndPos)
				ClickOff ();

			fingerTouchStatus = TouchStatus.During;
		}
		if (Input.GetMouseButtonUp (0)) 
		{
//			Debug.Log ("Mouse Button Up.");
			fingerTouchStatus = TouchStatus.Finish;
			TouchWorldPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			touchEndPos = Input.mousePosition;
			if (isFlick) 
			{
				float directionX = touchEndPos.x - touchStartPos.x;
				float directionY = touchEndPos.y - touchStartPos.y;
				float radian     = Mathf.Atan2 (directionY, directionX) * Mathf.Rad2Deg;
				if (radian < 0)
					radian += 360;

				if (directionX != 0 || directionY != 0) 
				{
					if (radian <= 22.5f || radian > 337.5f)
						moveDirection	= Direction.Right;
					else if (radian <= 45.0f && radian > 22.5f)
						moveDirection = Direction.UpperRightLower;
					else if (radian <= 67.5f && radian > 45.0f)
						moveDirection = Direction.UpperRightUpper;
					else if (radian <= 112.5f && radian > 67.5f)
						moveDirection = Direction.Up;
					else if (radian <= 125.0f && radian > 112.5f)
						moveDirection = Direction.UpperLeftUpper;
					else if (radian <= 157.5f && radian > 125.0f)
						moveDirection = Direction.UpperLeftLower;
					else if (radian <= 202.5f && radian > 157.5f)
						moveDirection = Direction.Left;
					else if (radian <= 225.0f && radian > 202.5f)
						moveDirection = Direction.LowerLeftUpper;
					else if (radian <= 247.5f && radian > 225.0f)
						moveDirection = Direction.LowerLeftLower;
					else if (radian <= 292.5f && radian > 247.5f)
						moveDirection = Direction.Down;
					else if (radian <= 315.0f && radian > 292.5f)
						moveDirection = Direction.LowerRightLower;
					else if (radian <= 337.5f && radian > 315.0f)
						moveDirection = Direction.LowerRightUpper;
					else
						moveDirection = Direction.None;
				} 
				else 
				{
					moveDirection = Direction.None;
				}
				Debug.Log ("directionX : " + directionX);
				Debug.Log ("directionY : " + directionY);
				Debug.Log ("Radian : " + radian); 
//				float absDirX = Mathf.Abs (directionX);
//				float absDirY = Mathf.Abs (directionY);
//				if (absDirX > absDirY) {
//					if (directionX > 0)
//						moveDirection = Direction.Right;
//					else
//						moveDirection = Direction.Left;
//				} else {
//					if (directionY > 0)
//						moveDirection = Direction.Up;
//					else
//						moveDirection = Direction.Down;
//				}
				Debug.Log ("Flick : " + moveDirection.ToString ());
				Debug.Log ("GameStatus : " + PazzleConfig.Instance.UserGameStatus.ToString ());
			} 
			else
			{
//				Debug.Log ("Long Tap");
				moveDirection = Direction.None;
			}
			Invoke ("reset", 0.02f);
		}
	}

	public void FlickOff()
	{
		isFlick = false;
		moveDirection = Direction.None;
	}

	public void ClickOff()
	{
		isClick = false;
	}

	private void reset()
	{
		fingerTouchStatus = TouchStatus.None;
	}
}
