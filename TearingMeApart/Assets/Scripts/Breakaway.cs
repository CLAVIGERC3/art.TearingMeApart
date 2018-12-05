using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakaway : MonoBehaviour {
	public int index = -1;
	Rigidbody2D body;
	void Awake(){
		index = int.Parse(gameObject.name);
		body = GetComponent<Rigidbody2D>();
		GameObject.FindObjectOfType<BreakawayManager>().RegisterBreakaway(this);
	}
	public void TriggerBreak(){
		GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
		body.constraints = RigidbodyConstraints2D.None;
		body.gravityScale = Random.Range(0.1f,0.2f);
		float xDirection = (transform.position.normalized).x;
		body.AddForce(new Vector2(xDirection*7.0f,2.0f),ForceMode2D.Impulse);
	}
	public void LockInPlace(){
		body.isKinematic = false;
		body.constraints = RigidbodyConstraints2D.FreezeAll;
		body.gravityScale = 0;
	}
}
