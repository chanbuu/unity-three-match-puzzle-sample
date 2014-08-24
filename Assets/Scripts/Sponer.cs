using UnityEngine;
using System.Collections;

public class Sponer : MonoBehaviour {

	private string FindTagName = "";
	private bool isEndInitialSpone = false;
	public bool IsEndInitialSpone
	{
		get { return isEndInitialSpone; }
		set { isEndInitialSpone = value; }
	}
	private int sponarNum;
	private bool isSponing = false;

	private void Awake()
	{
		FindTagName = this.gameObject.name;
		string sponarNumber = this.gameObject.name.Substring (this.gameObject.name.Length - 3, 3);
		sponarNum = System.Convert.ToInt32 (sponarNumber) - 1;
	}

	private void Start()
	{
		PazzleConfig.Instance.UserGameStatus = GameStatus.Initialize;
		BlockCreate ();
	}

	private void Update()
	{
		GameObject[] objs	= GameObject.FindGameObjectsWithTag (FindTagName);
//		if( (objs.Length <= PazzleConfig.RowCount && isEndInitialSpone) )
//		if( isEndInitialSpone )
		if ((objs.Length <= PazzleConfig.RowCount && isEndInitialSpone && PazzleConfig.Instance.UserGameStatus == GameStatus.Initialize)
		    || ( objs.Length <= PazzleConfig.RowCount && PazzleConfig.Instance.UserGameStatus != GameStatus.Initialize && !isSponing) )
		{
			isEndInitialSpone = false;
			if (PazzleConfig.Instance.UserGameStatus == GameStatus.Play) 
			{
				PazzleConfig.Instance.UserGameStatus = GameStatus.Drop;
			}

			BlockCreate ();
		}
	}

	public bool BlockCreate()
	{
		if (GameObject.FindGameObjectsWithTag (FindTagName).Length < PazzleConfig.RowCount) {
			isSponing = true;
			int blockNum = Random.Range (PazzleConfig.MinBlockSeriesNum, PazzleConfig.MaxBlockSeriesNum);

			GameObject block = Instantiate (Resources.Load (string.Format ("Block{0:D3}", blockNum)),
			                                this.gameObject.transform.position,
			                                Quaternion.identity) as GameObject;
			block.tag = FindTagName;
			block.name = "b" + FindTagName;
			block.transform.parent	= GameObject.FindGameObjectWithTag ("BlockArea").transform;
			Block blockInfo = block.GetComponent<Block> ();
			blockInfo.Id = blockNum;

			for (int i = PazzleConfig.RowCount - 1; i >= 0; i--) {
//				Debug.Log ("i : " + i + " / sponarNum : " + sponarNum);
				if (PazzleConfig.Instance.BlockPosition [sponarNum, i] == null) {
					Debug.Log ("x : " + sponarNum + " / y : " + i);
					PazzleConfig.Instance.BlockPosition [sponarNum, i] = block;
					blockInfo.BlockPosition = new Vector2 (sponarNum, i);
					break;
				}
			}
			return true;
		} 
		else
		{
			PazzleConfig.Instance.UserGameStatus = GameStatus.Play;
			return false;
		}
	}

	private void OnTriggerEnter2D( Collider2D collider2D )
	{
	}

	private void OnTriggerExit2D( Collider2D collider2D )
	{
		BlockCreate ();

		if (!isEndInitialSpone && GameObject.FindGameObjectsWithTag (FindTagName).Length == 4) 
		{
			isEndInitialSpone = true;
			PazzleConfig.Instance.UserGameStatus = GameStatus.Play;
			isSponing = false;
		}
	}
}
