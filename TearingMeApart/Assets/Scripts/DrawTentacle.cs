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
		rend.SetPositions(points);
	}
}
