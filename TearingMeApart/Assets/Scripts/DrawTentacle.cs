using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTentacle : MonoBehaviour {
	public Transform[] nodesInOrder = new Transform[13];
	LineRenderer rend;
	void Awake(){
		rend = GetComponent<LineRenderer>();
	}
	void Update(){
		Vector3[] points = new Vector3[13];
		for(int i = 0; i < nodesInOrder.Length; i++){
			points[i] = nodesInOrder[i].position;
		}
		rend.SetPositions(LerpList(points));
	}

	public Vector3[] LerpList(Vector3[] unlerped){
		Vector3[] lerped = new Vector3[unlerped.Length];
		for(int i = 0; i < unlerped.Length; i++){
			if(i < unlerped.Length - 3){
				lerped[i] = Vector2.Lerp(unlerped[i],Vector2.Lerp(unlerped[i+1],unlerped[i+2],0.5f),0.5f);
			}
			else if(i < unlerped.Length-2){
				lerped[i] = Vector2.Lerp(unlerped[i],unlerped[i+1],0.5f);
			}
			else{
				lerped[i] = unlerped[i];
			}
		}
		return lerped;
	}
}
