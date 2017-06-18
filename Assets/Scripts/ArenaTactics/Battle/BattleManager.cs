using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AT.Battle;
using AT.Character;
using AT;

using Util.StateMachine;

public class BattleManager : MonoBehaviour {


	/// <summary>
	/// The generic actor prefab, used to instantiate actors whether player or enemy
	/// </summary>
	public  GameObject genericActorPrefab;

	/// <summary>
	/// The current player sheets. these are set by a level manager of some kind (whether a skirmish menu or an in-game one)
	/// </summary>
	public List<Sheet> currentPlayerSheets = new List<Sheet>();
	public List<Sheet> currentEnemySheets= new List<Sheet>();

	/// <summary>
	/// The player actors.  Should be created dynamically, based on the player sheets set...
	/// </summary>
	private List<Actor> playerActors = new List<Actor>();

	/// <summary>
	/// The player actors.  Should be created dynamically, based on the player sheets set...
	/// </summary>
	private List<Actor> deadPlayerActors = new List<Actor>();

	/// <summary>
	/// The player actors.  Should be created dynamically, based on the player sheets set...
	/// </summary>
	private List<Actor> deadEnemyActors = new List<Actor> ();


	/// <summary>
	/// The player actors.  Should be created dynamically, based on the player sheets set...
	/// </summary>
	private List<Actor> enemyActors = new List<Actor> ();

	private Actor currentlyActing;


	public PhaseController battlePhases;

	public static BattleManager instance;
	void Awake() {
		instance = this;
		//DelayInitBattle ();
	}

	void DelayInitBattle() {

//		instance.StartCoroutine (InitBattle ());
	}

	public Side WinningSide {
		get { 
			if (playerActors.Count == 0) {
				return Side.ENEMY;
			}
			if (enemyActors.Count == 0) {
				return Side.PLAYER;
			}

			return Side.NONE;
		}
	}


	//TODO: should return a list of Actors
	public List<Actor> AllActors() {
		List<Actor> ret = new List<Actor> ();

		foreach (Actor g in playerActors) {
			ret.Add( g );
		}


		foreach (Actor g in enemyActors) {
			ret.Add( g );
		}


		return ret;
	}

	public List<Actor> ActorsOnSide(Side side) {
		return AllActors ().Where ((act) => SideFor (act) == side).ToList ();
	}

	public List<Actor> PlayerActors {
		get { 
			return playerActors.ToList ();
		}
	}

	public List<Actor> EnemyActors {
		get { 
			return enemyActors.ToList ();
		}
	}

	public Side SideFor(Actor a) {
		if(playerActors.Contains(a) || deadPlayerActors.Contains(a)) {
			return Side.PLAYER;
		} else {
			return Side.ENEMY;
		}
	}

	public List <Actor> EnemiesTo(Actor a) {
		
		if (SideFor(a) == Side.PLAYER)
			return new List<Actor> (enemyActors);
		else 
			return new List<Actor> (playerActors);
	}

	public enum Side
	{
		ENEMY,
		PLAYER,
		NONE
	}

	public void ReportInAs(Side side, Actor a) {
//		Debug.Log (a.CharSheet.Name + " reporting in on " + side);
		if (side == Side.ENEMY) {
			enemyActors.Add (a);
		} else {
			playerActors.Add (a);
		}

		a.OnActorKilled += RemoveActorWhenKilled;
	}

	public void ResetActionsThisRound() {
		foreach(Actor a in AllActors()) {
			a.ResetActions ();
		}
	}







	public void InitializeActorEntities() {
//		Debug.LogError ("here!");
		foreach (Sheet character in GameManager.persistentInstance.playerBattleCharacters) {
			GameObject actor = Instantiate (genericActorPrefab);

			actor.AddComponent<PlayerControlledActor> ();
			actor.GetComponent<Actor> ().CharSheet = character;
			actor.AddComponent<ClearWhenBehindGoesClearLayer> ();
			actor.name = character.Name;
			Vector2 spawn = MapManager.instance.currentMapInstance.PlayerSpawnPoints [0];
//			Debug.LogError("hto! " + spawn);
			actor.transform.localPosition = spawn;
		}

		foreach (Sheet character in GameManager.persistentInstance.enemyBattleCharacters) {
			GameObject actor = Instantiate (genericActorPrefab);
			actor.name = character.Name;
			actor.AddComponent<AiControlledActor> ();
			actor.AddComponent<VisibilityToPlayer> ();
			actor.GetComponent<Actor> ().CharSheet = character;
			Vector2 spawn = MapManager.instance.currentMapInstance.CpuEnemySpawnPoints [0];
			actor.transform.localPosition = spawn;
		}

//		Debug.LogError ("dga!");
		StartCoroutine (DelayActorsInitialized ());
	}

	public IEnumerator DelayActorsInitialized() {
		yield return new WaitForEndOfFrame ();
		ActorsInitialized ();
	}

	public bool actorsInitialized = false;
	public void ActorsInitialized() {
		actorsInitialized = true;
		if (OnActorsInitialized != null)
			OnActorsInitialized ();
	}

	public delegate void ActorsInitializedAction();
	public event ActorsInitializedAction OnActorsInitialized;


	public delegate void BattleInitializedAction();
	public event BattleInitializedAction OnBattleInitialized;
	public bool battleInitialized;
	public void BattleInitialized() {
		battleInitialized = true;
		if (OnBattleInitialized != null) {
			OnBattleInitialized ();
		}
	}

	public delegate void RoundEndedAction ();
	public event RoundEndedAction OnRoundEnded;
	public void RoundEnded() {
		if (OnRoundEnded != null) {
			OnRoundEnded ();
		}
	}

	public void InitBattle() {


		battlePhases = new PhaseController (this);
		battlePhases.SwitchState (battlePhases.battleBegin);
		//Don't shouldn't really need to clean this up, since each battle makes a new battlePhase controller.
		battlePhases.characterTurnBegin.OnDidEnter += CharacterBeganTurn;
		BattleInitialized ();
	}

	public Actor CurrentlyTakingTurn {
		get { return currentlyActing; }
	}

	void OnDestroy() {
		if (battlePhases != null) {
			battlePhases.characterTurnBegin.OnDidEnter -= CharacterBeganTurn;
		}
	}

	void CharacterBeganTurn(State s, State previous) {
		currentlyActing = battlePhases.characterTurnBegin.actor;
		if (OnCharacterTurnBegan != null) {
			OnCharacterTurnBegan (currentlyActing);
		}
	}

	public delegate void CharacterTurnBeganAction(Actor a);
	public event CharacterTurnBeganAction OnCharacterTurnBegan;






	void RemoveActorWhenKilled(Actor a) {
		
		if (playerActors.Contains (a)) {
			deadPlayerActors.Add (a);
			playerActors.Remove (a);
			if (playerActors.Count == 0) {
//				Debug.LogError ("battle phase should end.  cpu won!");
				battlePhases.SwitchState (battlePhases.battleEnd);
			}
		} else if (enemyActors.Contains (a)) {
			deadEnemyActors.Add (a);
			enemyActors.Remove (a);
			if (enemyActors.Count == 0) {
//				Debug.LogError ("battle phase should end.  player won!");
				battlePhases.SwitchState (battlePhases.battleEnd);
			}
		}



		a.OnActorKilled -= RemoveActorWhenKilled;
	}




	public class ActorMovedEvent {
		public Actor mover;
		public ATTile fromTile;
		public ATTile toTile;

		public Actor Actor {
			get { return mover; }
		}
		public ActorMovedEvent(Actor mover, ATTile fromTile, ATTile toTile) {
			this.mover = mover;
			this.fromTile = fromTile;
			this.toTile = toTile;
		}
	}

	public void EmitActorMovedEvent(Actor mover, ATTile fromTile, ATTile toTile){
		if (OnActorMovedEvent != null) {
			ActorMovedEvent evt = new ActorMovedEvent (mover, fromTile, toTile);
			OnActorMovedEvent (evt);
		}
	}//character event:
	public delegate void ActorMovedAction(ActorMovedEvent evt);
	public event ActorMovedAction OnActorMovedEvent;



	public class ActorPerformedEvent {
		public Actor actor;
		public Action action;

		public Actor Actor {
			get { return actor; }
		}
		public ActorPerformedEvent(Actor actor, Action action) {
			this.actor = actor;
			this.action = action;
		}
	}

	public void EmitActorPerformedEvent(Actor actor, Action action){
		if (OnActorPerformedEvent != null) {
			ActorPerformedEvent evt = new ActorPerformedEvent (actor, action);
			OnActorPerformedEvent (evt);
		}
	}

	public delegate void ActorPerformedAction(ActorPerformedEvent evt);
	public event ActorPerformedAction OnActorPerformedEvent;
}

