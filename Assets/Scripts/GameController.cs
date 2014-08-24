using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	private PazzleConfig config;
	private GameObject targetObj;
	private TapRecognizer tapRecoginizer;
	private List<GameObject> removeObjs;

	private void Awake()
	{
		config = PazzleConfig.GetInstance ();
		PazzleConfig.Instance.UserGameStatus = GameStatus.Before;
		removeObjs = new List<GameObject> ();
	}

	// Use this for initialization
	private void Start () {
	}
	
	// Update is called once per frame
	private void Update () {
		//if (Input.GetMouseButton (0)) 
		if( TapRecognizer.Instance.FingerTouchStatus == TapRecognizer.TouchStatus.Start )
		{
			Vector2 tapPoint   = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Collider2D col2D   = Physics2D.OverlapPoint (tapPoint);
			RaycastHit2D ray2D = Physics2D.Raycast (tapPoint, Vector2.zero);
			if (col2D && ray2D && ray2D.collider.gameObject.tag != "Untagged") 
			{
//				Debug.Log (tapPoint);
				PazzleConfig.Instance.UserInput = InputType.Tap;
//				config.UserInput = InputType.Tap;
				targetObj = ray2D.collider.gameObject;
//				Debug.Log ("targetObj : " + targetObj);
			}
		}
//		if (Input.GetMouseButtonUp (0)) 
//		Debug.Log (TapRecognizer.Instance.FingerTouchStatus);
		if( TapRecognizer.Instance.FingerTouchStatus == TapRecognizer.TouchStatus.Finish
		   && PazzleConfig.Instance.UserGameStatus == GameStatus.Play )
		{
			PazzleConfig.Instance.UserGameStatus = GameStatus.Judge;
			scan ();
//			StartCoroutine( scan () );
			config.UserInput = InputType.None;
			//Destroy (targetObj);
			removeObject ();
		}
	}

	private void scan()
	{
		if (PazzleConfig.Instance.UserGameStatus != GameStatus.Judge)
//			yield return new WaitForEndOfFrame();
			return;

		Block targetBlockInfo = targetObj.GetComponent<Block> ();
		int targetBlockPositionX = System.Convert.ToInt32 (targetBlockInfo.BlockPosition.x);
		int targetBlockPositionY = System.Convert.ToInt32 (targetBlockInfo.BlockPosition.y);
		Vector2 exchangeBlockPosition = targetBlockInfo.BlockPosition;

		switch (TapRecognizer.Instance.MoveDirection) 
		{
		// Right.
		case TapRecognizer.Direction.Right:
		case TapRecognizer.Direction.LowerRightUpper:
		case TapRecognizer.Direction.UpperRightLower:
			exchangeBlockPosition += Vector2.right;
			break;

		// Up.
		case TapRecognizer.Direction.Up:
		case TapRecognizer.Direction.UpperRightUpper:
		case TapRecognizer.Direction.UpperLeftUpper:
			exchangeBlockPosition -= Vector2.up;
			break;

		// Left.
		case TapRecognizer.Direction.Left:
		case TapRecognizer.Direction.UpperLeftLower:
		case TapRecognizer.Direction.LowerLeftUpper:
			exchangeBlockPosition -= Vector2.right;
			break;

		// Down.
		case TapRecognizer.Direction.Down:
		case TapRecognizer.Direction.LowerLeftLower:
		case TapRecognizer.Direction.LowerRightLower:
			exchangeBlockPosition += Vector2.up;
			break;

		default:
			Debug.Log ("default / movePosition : " + TapRecognizer.Instance.MoveDirection);
			PazzleConfig.Instance.UserGameStatus = GameStatus.Play;
//			yield return null;
//			break;
			return;
		}
		Debug.Log ("targetPosition : " + targetBlockInfo.BlockPosition);
		Debug.Log ("exchangePosition : " + exchangeBlockPosition);
		int exchangeBlockPositionX = System.Convert.ToInt32 (exchangeBlockPosition.x);
		int exchangeBlockPositionY = System.Convert.ToInt32 (exchangeBlockPosition.y);



		if (exchangeBlockPositionX < 0 || exchangeBlockPositionY < 0) 
		{
			PazzleConfig.Instance.UserGameStatus = GameStatus.Play;
//			yield return null;
			return;
		}

		if (exchangeBlockPositionX > (PazzleConfig.ColCount - 1) || exchangeBlockPositionY > (PazzleConfig.RowCount - 1)) 
		{
			PazzleConfig.Instance.UserGameStatus = GameStatus.Play;
//			yield return null;
			return;
		}

		if (PazzleConfig.Instance.BlockPosition [exchangeBlockPositionX, exchangeBlockPositionY] != null) 
		{
//			GameObject swapObjInstance;
			Vector3 swapPosition;
			string swapTagName;
			Vector2 swapBlockPosition;

			GameObject exchangeBlockObj = PazzleConfig.Instance.BlockPosition [exchangeBlockPositionX, exchangeBlockPositionY];
			swapPosition    = exchangeBlockObj.transform.localPosition;
			swapTagName     = exchangeBlockObj.tag;

			Block exchangeBlockInfo 	= exchangeBlockObj.GetComponent<Block> ();
			swapBlockPosition           = exchangeBlockInfo.BlockPosition;

			// exchange Block position Instances.
			PazzleConfig.Instance.BlockPosition [targetBlockPositionX, targetBlockPositionY]     = exchangeBlockObj;
			PazzleConfig.Instance.BlockPosition [exchangeBlockPositionX, exchangeBlockPositionY] = targetObj;

//			if (blockChainCheck (targetObj, exchangeBlockPositionX, exchangeBlockPositionY)) 
			if( allBlockCheck() )
			{
				// exchange positions.
				exchangeBlockObj.transform.localPosition = targetObj.transform.localPosition;
				targetObj.transform.localPosition = swapPosition;
				// exchange tags.
				exchangeBlockObj.tag = targetObj.tag;
				targetObj.tag = swapTagName;
				// exchange Block positions.
				exchangeBlockInfo.BlockPosition = targetBlockInfo.BlockPosition;
				targetBlockInfo.BlockPosition = swapBlockPosition;
			}
			else
			{
				// revert Block position Instances.
				PazzleConfig.Instance.BlockPosition [targetBlockPositionX, targetBlockPositionY]     = targetObj;
				PazzleConfig.Instance.BlockPosition [exchangeBlockPositionX, exchangeBlockPositionY] = exchangeBlockObj;
			}

		}
//		PazzleConfig.Instance.UserGameStatus = GameStatus.Judge;

//		Invoke ("setPlayGameStatus", 0.02f);
//		yield return null;
		//Debug.Log (PazzleConfig.Instance.BlockPosition [exchangeBlockPositionX, exchangeBlockPositionY]);
	}

	private void setPlayGameStatus()
	{
		PazzleConfig.Instance.UserGameStatus = GameStatus.Play;
	}

	private bool allBlockCheck()
	{
		for (int i = 0; i < PazzleConfig.ColCount; i++) 
		{
			for (int m = 0; m < PazzleConfig.RowCount; m++) 
			{
				GameObject targetObj = PazzleConfig.Instance.BlockPosition [i, m];
				blockChainCheck (targetObj, i, m);
			}
		}

		if (removeObjs.Count > 0)
			return true;

		return false;
	}

	private bool blockChainCheck( GameObject targetObj, int movedX, int movedY )
	{
		bool result  = false;
		int searchId = targetObj.GetComponent<Block> ().Id;
		int[] targetUpperPosition  = new int[2]{ movedX, movedY + 1 };
		int[] targetRighterPosition = new int[2]{ movedX + 1, movedY };
		int[] targetLowerPosition = new int[2]{ movedX, movedY - 1 };
		int[] targetLefterPosition = new int[2]{ movedX - 1, movedY };

		Debug.Log ("upper : " + targetUpperPosition [0] + " / " + targetUpperPosition [1]);
		Debug.Log ("right : " + targetRighterPosition [0] + " / " + targetRighterPosition [1]);
		Debug.Log ("lower : " + targetLowerPosition [0] + " / " + targetLowerPosition [1]);
		Debug.Log ("lefter: " + targetLefterPosition [0] + " / " + targetLefterPosition [1]);

		if (limitCheck (targetUpperPosition)) 
		{
			GameObject upperObj = PazzleConfig.Instance.BlockPosition [targetUpperPosition [0], targetUpperPosition [1]];
			if (idCheck (searchId, upperObj)) 
			{
				int[] targetMultiUpperPosition = new int[2]{ targetUpperPosition [0], targetUpperPosition [1] + 1 };
				if (limitCheck (targetMultiUpperPosition)) 
				{
					GameObject multiUpperObj = PazzleConfig.Instance.BlockPosition [targetMultiUpperPosition [0], targetMultiUpperPosition [1]];
					if (idCheck (searchId, multiUpperObj)) 
					{
						addRemoveTarget (upperObj);
						addRemoveTarget (multiUpperObj);
//						removeObjs.Add (upperObj);
//						removeObjs.Add (multiUpperObj);
						result = true;
					}
				}
			}
		}


		if (limitCheck (targetRighterPosition)) 
		{
			GameObject righterObj = PazzleConfig.Instance.BlockPosition [targetRighterPosition [0], targetRighterPosition [1]];
			if (idCheck (searchId, righterObj)) 
			{
				Debug.Log ("righter true");
				int[] targetMultiRighterPosition = new int[2]{ targetRighterPosition [0] + 1, targetRighterPosition [1] };
				if (limitCheck (targetMultiRighterPosition)) 
				{
					GameObject multiRighterObj = PazzleConfig.Instance.BlockPosition [targetMultiRighterPosition [0], targetMultiRighterPosition [1]];
					if (idCheck (searchId, multiRighterObj)) 
					{
						addRemoveTarget (righterObj);
						addRemoveTarget (multiRighterObj);
//						removeObjs.Add (righterObj);
//						removeObjs.Add (multiRighterObj);
						result = true;
					}
				}
			}
		}

		if (limitCheck (targetLowerPosition)) 
		{
			GameObject lowerObj = PazzleConfig.Instance.BlockPosition [targetLowerPosition [0], targetLowerPosition [1]];
			if (idCheck (searchId, lowerObj)) 
			{
				Debug.Log ("lower true");
				int[] targetMultiLowerPosition = new int[2]{ targetLowerPosition [0], targetLowerPosition [1] - 1 };
				if (limitCheck (targetMultiLowerPosition)) 
				{
					GameObject multiLowerObj = PazzleConfig.Instance.BlockPosition [targetMultiLowerPosition [0], targetMultiLowerPosition [1]];
					if (idCheck (searchId, multiLowerObj)) 
					{
						addRemoveTarget (lowerObj);
						addRemoveTarget (multiLowerObj);
//						removeObjs.Add (lowerObj);
//						removeObjs.Add (multiLowerObj);
						result = true;
					}
				}
			}
		}

		if (limitCheck (targetLefterPosition)) 
		{
			GameObject lefterObj = PazzleConfig.Instance.BlockPosition [targetLefterPosition [0], targetLefterPosition [1]];
			if (idCheck (searchId, lefterObj)) 
			{
				Debug.Log ("lefter true");
				int[] targetMultiLefterPosition = new int[2]{ targetLefterPosition [0] - 1, targetLefterPosition [1] };
				if (limitCheck (targetMultiLefterPosition)) 
				{
					GameObject multiLefterObj = PazzleConfig.Instance.BlockPosition [targetMultiLefterPosition [0], targetMultiLefterPosition [1]];
					if (idCheck (searchId, multiLefterObj)) 
					{
						addRemoveTarget (lefterObj);
						addRemoveTarget (multiLefterObj);
//						removeObjs.Add (lefterObj);
//						removeObjs.Add (multiLefterObj);
						result = true;
					}
				}
			}
		}

		if (result)
			addRemoveTarget (targetObj);

		return result;
	}

	private bool limitCheck( int[] position )
	{
		if (position.Length > 2)
			return false;

		if (position [0] > (PazzleConfig.ColCount - 1) || position [0] < 0)
			return false;

		if (position [1] > (PazzleConfig.RowCount - 1) || position [1] < 0)
			return false;

		return true;
	}

	private bool idCheck( int targetId, GameObject searchObj )
	{
		if (searchObj.GetComponent<Block> () == null)
			return false;

		Block targetBlockInfo = searchObj.GetComponent<Block> ();
		if (targetId != targetBlockInfo.Id)
			return false;

		return true;
	}

	private bool addRemoveTarget( GameObject obj )
	{
		if (!removeObjs.Contains (obj)) 
		{
			removeObjs.Add (obj);
			return true;
		}
		return false;
	}

	private bool removeObject()
	{
		if (removeObjs.Count != 0) 
		{
			for (int i = 0; i < removeObjs.Count; i++) 
			{
//				string removeObjTag = removeObjs [i].tag;
				Block removeBlockInfo = removeObjs [i].GetComponent<Block> ();
				int removeBlockPositionX = System.Convert.ToInt32 (removeBlockInfo.BlockPosition.x);
				int removeblockPositionY = System.Convert.ToInt32 (removeBlockInfo.BlockPosition.y);
				Debug.Log ("destroy x : " + removeBlockPositionX + " /  y : " + removeblockPositionY);
				narrowBlockSpace (removeBlockPositionX, removeblockPositionY);
				Destroy (removeObjs [i]);
//				PazzleConfig.Instance.BlockPosition [removeBlockPositionX, removeblockPositionY] = null;
//				GameObject.Find (removeObjTag).GetComponent<Sponer> ().IsEndInitialSpone = true;
			}
			for (int m = 0; m > removeObjs.Count; m++) 
			{
//				Destroy (removeObjs [m]);
			}
		}

//		Invoke ("setPlayGameStatus", 0.02f);
		removeObjs.Clear ();
		return true;
	}

	private bool narrowBlockSpace( int x, int y )
	{
		for (int i = (y - 1); i >= 0; i--) 
		{
			GameObject targetObj  = PazzleConfig.Instance.BlockPosition [x, i];
			if (targetObj == null)
				break;

			Debug.Log ("narrow target obj x : " + x + " / y : " + i); 
			Debug.Log ("narrow target obj : " + targetObj);
			Block targetBlockInfo = targetObj.GetComponent<Block> ();
			targetBlockInfo.BlockPosition = new Vector2 (x, i + 1);
			PazzleConfig.Instance.BlockPosition [x, i + 1] = targetObj;
			PazzleConfig.Instance.BlockPosition [x, i] = null;
		}

		return true;
	}
}
