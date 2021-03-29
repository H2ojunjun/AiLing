using UnityEngine;
using System.Collections;
using static iTween;

public class MoveSample : MonoBehaviour
{	
	void Start(){
		iTween.MoveBy(gameObject, iTween.Hash("x", 200, "easeType", "easeInOutExpo", "loopType", LoopType.loop, "delay", .1));
	}
}

