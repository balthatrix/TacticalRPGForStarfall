using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Util.StateMachine;

namespace AT.Battle.AI {

	public class MeleeAttacker : AiController {
		public MeleeAttacker(AiControlledActor actor) : base(actor){
			Initialize ();
		}

		Camping camping;
		FindingBestTarget findingBestTarget;
		SeekingTarget seekingTarget;
		MovingToPosition movingToPosition;
		Attacking attacking;

		private ATTile lastTileLostEnemyVisionOn = null;

		public void Initialize() {

			//action yielding state
			 camping = new Camping (actor, this);

			//should be transitory states.... but they yield wait if somehow get caught up.
			 findingBestTarget = new FindingBestTarget (actor, this);
			 seekingTarget = new SeekingTarget (actor, this);

			//more action yielding states
			 movingToPosition = new MovingToPosition (actor, this);
			 attacking = new Attacking (actor, this);

			camping.AddTransition ((state) => {
				return camping.TargetSighted();
			}, findingBestTarget);

			camping.AddTransition ((state) => {
				if(lastTileLostEnemyVisionOn != null) {
					movingToPosition.position = lastTileLostEnemyVisionOn;
					lastTileLostEnemyVisionOn = null;
					return true;
				} 
				return false;
			}, movingToPosition);

			findingBestTarget.AddTransition ((state) => {
				return findingBestTarget.NoTargetInSight();
			}, camping);
			findingBestTarget.AddTransition ((state) => {
				Actor enemy = findingBestTarget.BestTarget();
				if(enemy != null) {
					seekingTarget.target = enemy;
					return true;
				}
				return false;
			}, seekingTarget);
			findingBestTarget.AddTransition ((state) => {
				if(findingBestTarget.NoPathToBestTarget() && lastTileLostEnemyVisionOn != null) {
					movingToPosition.position = lastTileLostEnemyVisionOn;
					lastTileLostEnemyVisionOn = null;
					return true;
				}
				return false;
			}, movingToPosition);


			seekingTarget.AddTransition ((state) => {
				if(seekingTarget.RightByTarget()) {
					attacking.target = seekingTarget.target;
					return true;
				}
				return false;
			}, attacking);

			seekingTarget.AddTransition ((state) => {
				ATTile bestPosition = seekingTarget.BestPosition();
				if(bestPosition != null) {
					movingToPosition.position = bestPosition;
					return true;
				}
				return false;
			}, movingToPosition);

			seekingTarget.AddTransition ((state) => {
				if(seekingTarget.NoPathToTarget()) {
					return true;
				}
				return false;
			}, findingBestTarget);





			//This cancels out the movement if any enemy characters move
			movingToPosition.OnDidEnter += (s, fromPrevious) => {
				//used to go somewhere when an enemy is seen for a flash
//				tileToInspect = null;
//				tileToInspect = null;
				foreach (Actor enemy in actor.GetComponent<Vision>().EnemiesInVision) {
					enemy.OnDidPerform += CancelMovementIfEnemyActs;
				}
			};
			movingToPosition.OnWillExit += (s, toDest) => {

				foreach (Actor enemy in actor.GetComponent<Vision>().EnemiesInVision) {
					enemy.OnDidPerform -= CancelMovementIfEnemyActs;
				}
			};
			movingToPosition.AddTransition ((state) => {
				if(movingToPosition.ReachedPosition()) {

					return true;
				}
				return false;
			}, findingBestTarget);


			actor.GetComponent<Vision>().OnVisionOfEnemyGained += CancelMovementOnGainedVision;
			actor.GetComponent<Vision>().OnVisionOfEnemyLost += CancelMovementOnLostVision;


			attacking.AddTransition ((state) => {
				return attacking.LostEnemySight();
			}, findingBestTarget);

			attacking.AddTransition ((state) => {
				return attacking.TargetDied();
			}, findingBestTarget);

			attacking.AddTransition ((state) => {
				return attacking.TargetMoved();
			}, findingBestTarget);

//			InitDebugStateChanges ();

			this.SwitchState(camping);
		}


		private void CancelMovementOnLostVision(Actor enemyLostVisionOf) {
			movingToPosition.canceled = true;

			lastTileLostEnemyVisionOn = enemyLostVisionOf.TileMovement.occupying;
			enemyLostVisionOf.OnDidPerform -= CancelMovementIfEnemyActs;
		}

		private void CancelMovementOnGainedVision(Actor enemySighted) {
			
//			movingToRememberedPosition.canceled = true;
			movingToPosition.canceled = true;
		}

		private void CancelMovementIfEnemyActs(Action action) {
			
			if (action is Wait ||
			    action is Dodge ||
			    action is Dash) {
				return;
			}

			movingToPosition.canceled = true;
			//Do this so in case the character is no longer in vision, they don't cause canceling of movement randomly later
			action.actor.OnDidPerform -= CancelMovementIfEnemyActs;
		}

		public void DeInitialize() {
			//Do this so any current state does it's cleanup
			SwitchState (camping);
			actor.GetComponent<Vision>().OnVisionOfEnemyLost -= CancelMovementOnLostVision;
			actor.GetComponent<Vision>().OnVisionOfEnemyGained -= CancelMovementOnGainedVision;
		}

		public void InitDebugStateChanges() {
			camping.OnDidEnter += (State s, State prv) => {
				if(prv != null) {

					Debug.Log(actor.CharSheet.Name + " entered camping from " + prv.GetType().ToString());
				} else {
					Debug.Log(actor.CharSheet.Name + " entered camping ex nihilio");	
				}
			};
			findingBestTarget.OnDidEnter += (State s, State prv) => {
				if(prv != null) {

					Debug.Log(actor.CharSheet.Name + " entered findingBestTarget from " + prv.GetType().ToString());
				} else {
					Debug.Log(actor.CharSheet.Name + " entered findingBestTarget ex nihilio");	
				}
			};
			seekingTarget.OnDidEnter += (State s, State prv) => {
				if(prv != null) {

					Debug.Log(actor.CharSheet.Name + " entered seekingTarget from " + prv.GetType().ToString());
				} else {
					Debug.Log(actor.CharSheet.Name + " entered seekingTarget ex nihilio");	
				}
			};
			movingToPosition.OnDidEnter += (State s, State prv) => {
				if(prv != null) {

					Debug.Log(actor.CharSheet.Name + " entered movingToPosition from " + prv.GetType().ToString());
				} else {
					Debug.Log(actor.CharSheet.Name + " entered movingToPosition ex nihilio");	
				}
			};
			attacking.OnDidEnter += (State s, State prv) => {
				if(prv != null) {

					Debug.Log(actor.CharSheet.Name + " entered attacking from " + prv.GetType().ToString());
				} else {
					Debug.Log(actor.CharSheet.Name + " entered attacking ex nihilio");	
				}
			};
		}

	}

}