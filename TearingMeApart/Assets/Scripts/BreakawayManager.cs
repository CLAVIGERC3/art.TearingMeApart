using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakawayManager : MonoBehaviour {
	public Vector2[] originalLocations = new Vector2[16];
	public int piecesDropped = 0;
	List<Breakaway> pieces = new List<Breakaway>();
	public List<Breakaway> activePieces = new List<Breakaway>();
	public void RegisterBreakaway(Breakaway obj){
		if(!pieces.Contains(obj)){
			pieces.Add(obj);  
			originalLocations[obj.index] = obj.transform.position;
			Debug.Log(obj.name + " -Chunk added to breakaway manager.");
		}
		if(pieces.Count == 16) StartCoroutine(ChunkBreak());
	}
	private bool CanBreakaway(){
		return piecesDropped < 4;
	}
	public void ChunkCaught(Breakaway chunk){
		if(activePieces.Contains(chunk)){
			piecesDropped--;
		}else Debug.LogError("ERROR: Chunk caught but not marked as active");
	}
	public void ChunkReset(Breakaway chunk){
		if(activePieces.Contains(chunk)){
			activePieces.Remove(chunk);
		}else Debug.LogError("ERROR: Chunk reset but not marked as active");
	}
	IEnumerator ChunkBreak(){
		while(true){
			yield return new WaitUntil(CanBreakaway);
			int randIndex = Random.Range(0,pieces.Count);
			if(!activePieces.Contains(pieces[randIndex])){
				activePieces.Add(pieces[randIndex]);
				pieces[randIndex].TriggerBreak();
				piecesDropped++;
				yield return new WaitForSeconds(Random.Range(5f,10f)); 
			}
		}
	}
}
