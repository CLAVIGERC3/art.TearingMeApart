using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour {
	public enum Stage{
		INACTIVE,
		MOVING_TO_CHUNK,
		GRABBING_CHUNK,
		RETURNING_CHUNK,
		RESETTING_CHUNK
	}
	public Stage currentStage =  Stage.INACTIVE;
	public GameObject grabber;
	Rigidbody2D grabBod;
	BreakawayManager manager;
	float MAX_DIST = 4.0f;
	float MIN_DIST = 0.25f;
	float MAX_SPEED = 7f;
	float MIN_SPEED = 0.05f;

	void Awake(){
		grabBod = grabber.GetComponent<Rigidbody2D>();
		manager = GameObject.FindObjectOfType<BreakawayManager>();
	}
	void Start(){
		StartCoroutine(ChunkRecovery());
	}
	void Update(){
		if(Input.anyKeyDown){
			Application.Quit();
		}
	}
	Breakaway activePiece = null;
	bool IsChunkReached(){
		return Vector2.Distance(grabber.transform.position,activePiece.transform.position) <= MIN_DIST;
	}
	bool IsChunkReturned(){
		return Vector2.Distance(grabber.transform.position,manager.originalLocations[activePiece.index]) <= MIN_DIST;
	}
	bool IsChunkReset(){
		Transform activeTransform = activePiece.transform;
		bool anglesCorrect = Mathf.Abs(activeTransform.eulerAngles.z) < 3.0f;
		bool positionCorrect = Vector2.Distance(manager.originalLocations[activePiece.index],activeTransform.position) <= MIN_DIST;
		return anglesCorrect && positionCorrect;
	}
	bool AreChunksLoose(){
		if(manager != null){
			return manager.piecesDropped > 0;
		}
		else return false;
	}
	IEnumerator ChunkRecovery(){
		while(true){
			if(!AreChunksLoose()){
				yield return new WaitForSeconds(0.1f);
				continue;
			}
			yield return new WaitForSeconds(Random.Range(1.0f,3.0f));
			PickChunk();
			currentStage = Stage.MOVING_TO_CHUNK;
			StartCoroutine(MoveToChunk());
			while(true){
				if(IsChunkReached()) break;
				yield return new WaitForEndOfFrame();
			}
			currentStage = Stage.GRABBING_CHUNK;
			StopCoroutine(MoveToChunk());
			GrabChunk();
			StartCoroutine(ReturnChunk());
			yield return new WaitForEndOfFrame();
			currentStage = Stage.RETURNING_CHUNK;
			while(true){
				if(IsChunkReturned()){
					currentStage = Stage.RESETTING_CHUNK;
					break;
				} 
				yield return new WaitForEndOfFrame();
			}
			StopCoroutine(ReturnChunk());
			currentStage = Stage.RESETTING_CHUNK;
			StartCoroutine(ResetChunk());
			while(true){
				if(IsChunkReset()) break;
				yield return new WaitForEndOfFrame();
			}
			StopCoroutine(ResetChunk());
			FinalCleanup();
			yield return new WaitForSeconds(Random.Range(0.5f,1f));
		}
	}
	IEnumerator MoveToChunk(){
		while(true){
			Vector2 direction = ((Vector2)activePiece.transform.position-grabBod.position).normalized;
			grabBod.velocity = (direction*GetNewSpeed(activePiece.transform.position));
			yield return new WaitForEndOfFrame();
		}
	}
	IEnumerator ReturnChunk(){
		Rigidbody2D activeBod = activePiece.GetComponent<Rigidbody2D>();
		while(true){
			Vector2 direction = (manager.originalLocations[activePiece.index] - grabBod.position).normalized;
			grabBod.velocity = (direction * GetNewSpeed(manager.originalLocations[activePiece.index]));
			activeBod.MovePosition(grabBod.position);
			yield return new WaitForEndOfFrame();
		}
	}
	IEnumerator ResetChunk(){
		Rigidbody2D activeChunkBody = activePiece.GetComponent<Rigidbody2D>();
		Vector2 startingLocation = activeChunkBody.position;
		float startingAngle = activePiece.transform.eulerAngles.z;
		int iterations = 0;
		while(true){
			activeChunkBody.MovePosition(Vector2.Lerp(startingLocation,manager.originalLocations[activePiece.index],iterations/60.0f));
			activeChunkBody.MoveRotation(Mathf.LerpAngle(startingAngle,0f,iterations/60.0f));
			iterations++;
			yield return new WaitForEndOfFrame();
		}
	}
	void PickChunk(){
		List<Breakaway> activePieces = manager.activePieces;
		float maxDist = 0f;
		if(activePieces.Count < 1) Debug.LogError("ERROR: Trying to pick a chunk from any empty list");
		Breakaway currentActive = activePieces[0];
		foreach(Breakaway chunk in activePieces){
			float hereDistance = Vector2.Distance(manager.originalLocations[chunk.index],chunk.transform.position);
			if(hereDistance > maxDist){
				maxDist = hereDistance;
				currentActive = chunk;
			}
			activePiece = currentActive;
		}
		if(activePiece == null) Debug.LogError("ERROR: Chosen active piece is null");
	}

	void GrabChunk(){
		activePiece.GetComponent<Rigidbody2D>().isKinematic = true;
		manager.ChunkCaught(activePiece);
	}
	void FinalCleanup(){
		StopAllCoroutines();
		grabBod.velocity = Vector2.zero;
		currentStage = Stage.INACTIVE;
		SpriteRenderer rend = activePiece.GetComponentInChildren<SpriteRenderer>();
		rend.sortingOrder = 1;
		Transform activeTransform = activePiece.transform;
		activeTransform.SetPositionAndRotation(manager.originalLocations[activePiece.index],Quaternion.identity);
		manager.ChunkReset(activePiece);
		activePiece.LockInPlace();
		activePiece = null;
		StartCoroutine(ChunkRecovery());
	}
	public float GetNewSpeed(Vector2 goal){
		float distance = Vector2.Distance(goal,grabBod.position);
		float percent = (distance - MIN_DIST) / (MAX_DIST - MIN_DIST);
		return (MAX_SPEED*percent) + MIN_SPEED;
	}

}
