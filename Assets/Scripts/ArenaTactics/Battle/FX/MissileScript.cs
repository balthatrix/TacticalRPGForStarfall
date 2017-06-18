using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Battle;
using AT;


#if UNITY_EDITOR

using UnityEditor;
[CustomEditor(typeof(MissileScript))]
public class MissileScriptEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		MissileScript missile = (MissileScript) target;
		if(GUILayout.Button("Test Launch"))
		{
			missile.LaunchAt (BattleManager.instance.AllActors () [0].TileMovement.occupying, true);
		}
	}
}
#endif

public class MissileScript : MonoBehaviour {
	public enum MissileAnimationName {
		NOT_SET,
		HANDAXE,
		DAGGER,
		PLACEHOLDER
	}

	public float speed;

	public ATTile targetTile;
	public ATTile previousHitTile;


	bool ignoreBlockers = false;
	public void DoIgnoreBarriers() {
		ignoreBlockers = true;
	}



	public void LaunchAt(Transform target, bool noBlockers=false) {
		Vector3 vel = (target.position - transform.position).normalized * speed;
		GetComponent<Rigidbody2D> ().velocity = vel;

		if(noBlockers)
			DoIgnoreBarriers ();
	}

	public void LaunchAt(AT.ATTile tTile, bool noBlockers=false) {
		targetTile = tTile;
		LaunchAt (targetTile.transform, noBlockers);

	}

	public void DelayLaunchAt(AT.ATTile tTile, float delay, bool noBlockers=false) {
		StartCoroutine (LaunchAfterDelay (delay, tTile, noBlockers));
	}

	IEnumerator LaunchAfterDelay(float delay, AT.ATTile target, bool noBlockers) {
		Debug.Log("Waiting " + delay +  " seconds to launch");
		yield return new WaitForSeconds (delay);
		LaunchAt (target, noBlockers);
	}


	public void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag ("Tile")) {
			
			ATTile tile = other.GetComponent<ATTile> ();
			if (tile == targetTile) {
				StartCoroutine(WaitForGroundZero ());

			} else if (tile.BlocksMissiles && !this.ignoreBlockers) {
				
				if (OnHitBlocker != null) {
					OnHitBlocker (this, tile);
				}
				DetachParticlesAndDestroy ();
			}

			previousHitTile = tile;
		}
	}

	bool distanceDecreasing;
	//while distance travell
	IEnumerator WaitForGroundZero() {
		distanceDecreasing = true;
		float distanceTraveledSqrd = 0f;
		Vector3 startPos = transform.position;
		float distanceToObjectSqrd = (targetTile.transform.position - startPos).sqrMagnitude;

		float percentMargin = (distanceToObjectSqrd * 0.5f);
		while (distanceTraveledSqrd < distanceToObjectSqrd - percentMargin) {
			Vector3 posDiff = transform.position - startPos;

			distanceTraveledSqrd = posDiff.sqrMagnitude;
			yield return null;
		}

		Debug.Log ("HERER!!!!!!!!!:");
		if (OnConnectedWithTarget != null) {
			OnConnectedWithTarget (this);
		}
		DetachParticlesAndDestroy ();
	}

	public void DetachParticlesAndDestroy() {
		List<GameObject> toDestroyAfterDelay = new List<GameObject> ();

		for (int i = 0; i < transform.childCount; i++) {
			Transform trasn = transform.GetChild (i);
			foreach (SpriteParticleEmitter.DynamicEmitter sys in trasn.GetComponentsInChildren<SpriteParticleEmitter.DynamicEmitter> ()) {
				sys.enabled = false;
			}
			foreach (SpriteRenderer sys in trasn.GetComponentsInChildren<SpriteRenderer> ()) {
				sys.enabled = false;
			}
			foreach (Animator sys in trasn.GetComponentsInChildren<Animator> ()) {
				sys.enabled = false;
			}

			trasn.SetParent (null);
			toDestroyAfterDelay.Add (trasn.gameObject);
		}
		Destroy (gameObject);
		GameManager.persistentInstance.StartCoroutine (DestroyChildrenAfterDelay (toDestroyAfterDelay));
	}

	public IEnumerator DestroyChildrenAfterDelay(List<GameObject> toDestroy) {
		
		yield return new WaitForSeconds (6f);

		foreach (GameObject go in toDestroy) {
			Destroy (go);
		}
	}





	public delegate void ConnectedWithTargetAction(MissileScript self);
	public event ConnectedWithTargetAction OnConnectedWithTarget;

	public delegate void HitBlockingObjectAction(MissileScript self, ATTile blocker);
	public event HitBlockingObjectAction OnHitBlocker;
}

