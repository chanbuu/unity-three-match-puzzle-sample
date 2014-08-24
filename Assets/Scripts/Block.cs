using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	public int Id{ get; set; }
	public Vector2 BlockPosition{ get; set; }
	public int BlockPositionX{ get; set; }
	public int BlockPositionY{ get; set; }

	private Vector3 originalPosition;
	private PazzleConfig config;

	// Use this for initialization
	private void Awake () 
	{
		originalPosition = this.gameObject.transform.localPosition;
		BlockPosition = Vector2.zero;
	}

	private void Start()
	{
		config = PazzleConfig.GetInstance ();
	}
	
	// Update is called once per frame
	private void Update () {
//		if (config.UserInput == InputType.None) 
		if( TapRecognizer.Instance.FingerTouchStatus == TapRecognizer.TouchStatus.None )
		{
			this.gameObject.transform.localPosition = new Vector3 (originalPosition.x,
			                                                       this.gameObject.transform.localPosition.y,
			                                                       this.gameObject.transform.localPosition.z);
		} 
		else
		{
//			Debug.Log ("Move!!!!!");
			originalPosition = this.gameObject.transform.localPosition;
		}
	}
}
