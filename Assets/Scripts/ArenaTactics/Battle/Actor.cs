using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AT.Character;
using AT.Character.Situation;


namespace AT { 
	namespace Battle {
		[RequireComponent (typeof(TileMovement))]
		public class Actor : MonoBehaviour {


			Dictionary<AT.Character.Condition.ICondition, GameObject> conditionsToTextObjects;

			List<Action> allActionsThisRound = new List<Action> ();

			Action currentlyPerforming = null;
			public Action CurrentlyPerforming {

				get { return currentlyPerforming; }
				set { currentlyPerforming = value; }
			
			}

			public void PreventReactions(Action cause) {
				reactionThisRound = cause;	
			}

			public bool IsOnPlayerSide {
				get { 

					return (BattleManager.instance.SideFor (this) == BattleManager.Side.PLAYER);
				}
			}

			public bool IsSeenByAPlayer {
				get { 
					return TileMovement.occupying.SeenByAlliesOf(
						BattleManager.instance.PlayerActors.First()
					);
				}
			}

			private  bool dying;

			private Vector3 advDisIndOffset = new Vector3(.3f, .60f, 0f);

			public bool Dying {
				get { return dying; }
				set { dying = value; }
			}
			public string sheetName;
			public virtual void Awake() {
	//			Debug.Log("Initializing a " + GetType().ToString());
				Vision eyes = GetComponent<Vision> ();

					
				tileMovement = GetComponent<TileMovement> ();
				//string path = Application.persistentDataPath + "/testCreation";
	//			Debug.LogError ("Hardcoded path for getting a sheet character.  TODO: dont do this: " + path);
				//charSheet = AT.Serialization.Manager.Deserialize<Sheet>(path);
				charSheet= new Sheet();
	            
				//sheetName = charSheet.Name;
				conditionsToTextObjects = new Dictionary<AT.Character.Condition.ICondition, GameObject> ();
			}


			// Use this for initialization
			public virtual void Start () {

	//			charSheet.OnWasAttacked += InstantiateMissText;
				charSheet.OnKilled += Died;


				charSheet.OnHealed += ShowHealedText;

				//TODO: this will be a problem when serialization occurs.
				//For example: say the player saves while poisoned.
				//when the char sheet is initialized above with all serialized conditions,
				//this won't have happened yet, so this instance won't be notified.
				charSheet.OnConditionTaken += ShowCondition;
				charSheet.OnConditionRemoved += RemoveFromConditions;


				tileMovement.OnWalkedOutOfTile += ActorMovedOutOfTile;
				OnWillPerform += AggregateActionsThisRound;

				charSheet.OnDidAttack += AdvDisIndicator;


			}

			void AdvDisIndicator(AttackSituation sit) {
				if (sit.DisadvantageFlagged () && sit.AdvantageFlagged()) {
				} else if(sit.AdvantageFlagged()) {
					GameObject o = Instantiate(UIManager.instance.advantageIndicator);
					o.transform.SetParent (Avatar,false);
					o.transform.localPosition = Vector3.zero  + advDisIndOffset;
				} else if(sit.DisadvantageFlagged()) {
					GameObject o = Instantiate(UIManager.instance.disadvantageIndicator);
					o.transform.SetParent (Avatar,false);
					o.transform.localPosition =  Vector3.zero + advDisIndOffset;
				}
			}

			public Transform Avatar {
				get {return TileMovement.avatar;}
			}

			void OnDestroy() {
				tileMovement.OnWalkedOutOfTile -= ActorMovedOutOfTile;
				OnWillPerform -= AggregateActionsThisRound;

				//TODO: seperate this into a "indicate FX" script or something.
				charSheet.OnConditionTaken -= ShowCondition;
				charSheet.OnConditionRemoved -= RemoveFromConditions;
				charSheet.OnKilled -= Died;
	//			charSheet.OnWasAttacked -= InstantiateMissText;
				charSheet.OnDidAttack -= AdvDisIndicator;
				charSheet.OnHealed -= ShowHealedText;
			}

			public void ShowHealedText(AT.Character.Effect.Healing effect, Action source) {
				//Debug.LogError ("trygin to show healed text");
				GameObject heal = Object.Instantiate (FXPrefabs.instance.healingTextPrefab);

				heal.GetComponent<FadeText> ().textComponent.text = effect.Amount.ToString ();
				heal.transform.SetParent(Avatar, false);
			}


			public void ShowCondition(Sheet c, AT.Character.Condition.ICondition condition) {
				GameObject cText = Instantiate (UIManager.instance.conditionText);
				cText.transform.SetParent (Avatar,false);

				cText.transform.localPosition = new Vector3 (0f, ConditionTextOffsetY(conditionsToTextObjects.Count), 0f);

				cText.GetComponent<ConditionIndicator> ().textObject.text = "";//condition.Tag ();
				cText.GetComponent<ConditionIndicator> ().image.sprite = condition.SpriteIcon();
				conditionsToTextObjects.Add (condition, cText);
			
			}

			public void RemoveFromConditions(Sheet c, AT.Character.Condition.ICondition condition) {
				GameObject o;

				if(conditionsToTextObjects.TryGetValue(condition, out o)) {


					conditionsToTextObjects.Remove (condition);

					Destroy (o);




				}
			}

			IEnumerator MonitorConditionsTexts() {
				yield return new WaitForSeconds (2f);


				StartCoroutine (MonitorConditionsTexts ());
			}

			private float ConditionTextOffsetY(int count) {
				return -count * .2f + .3f;
			}

			public void Died(Sheet mySheet) {

				Dying = true;
				Debug.Log (mySheet.Name + " died! ");
				if (UIManager.instance.cameraController.CurrentLock == transform) {
					UIManager.instance.cameraController.CancelLock ();
				}

				if (CurrentlyPerforming != null) {
					CurrentlyPerforming.CallOnFinished ();
				}

				ActorDied ();
	//			StartCoroutine (DiedStuffToDelay ());

			}


				

			public void InstantiateMissText() {
				
				GameObject missText = Object.Instantiate (UIManager.instance.missHitText);
				missText.transform.SetParent (Avatar,false);
				missText.transform.localPosition = Vector3.zero;

			}


				



			//Tried to keep this as close to the SRD as possible//
			//must not exceed character attacks per round
			private Action actionThisRound; //interactions can fit into this action....

			private Action bonusActionThisRound;
			//reaction is attack of opportunity or spell trigger.
			private Action reactionThisRound;

			//interaction is picking up an item, or using and item, trading (uses interaction of other character as well), or equipping an item.
			private Action interactionThisRound;

			public void ResetMoves() {
				movesThisRound = 0;
			}

			public void ResetAttacks() {
				attacksThisRound = 0;
			}


			public void ResetActions() {
				ResetMoves ();
				ResetAttacks ();
				actionThisRound = null;
				bonusActionThisRound = null;
				reactionThisRound = null;
				interactionThisRound = null;
				allActionsThisRound.Clear ();
			}


			public void ActorMovedOutOfTile(ATTile current, ATTile dest) {
				movesThisRound += dest.MoveCostFor(this);
				foreach (Actor a in current.Reachers) {
					if (!a.EnemiesWith (this))
						continue;
					//If you are moving out of a tile within reach for this reacher....
					if (!a.TileMovement.TilesWithinAttackReach ().Contains (dest) && !a.UsedReaction()) {
						GetComponent<AnimationTransform> ().Pause ();
						Attack opportunity = new Attack (a);
						//TODO: give player an option to use main/offhand for reaction
						//Hardcode offhand being false for now
	//					IsOffhand optVal = new IsOffhand (false); 
						(opportunity.ActionOptions [0] as AttackTypeOption).chosenChoice =  new AttackTypeChoice(AttackType.MAINHAND_MELEE, a.CharSheet.MainHand());
						opportunity.IsReaction = true;
						opportunity.LateSetTargetParameters ();
						opportunity.SetTarget (this);

						opportunity.OnFinished += FinishedGettingOpportunityAttacked;
						opportunity.Perform ();
					}
				}
			} 


			/// <summary>
			/// Produces the root action button node, used on turn begin to populate the action bar.
			/// </summary>
			/// <returns>The root action button node.</returns>
			public ActionButtonNode ProduceRootActionButtonNode() {
				ActionButtonNode root = new ActionButtonNode ();
				DressRootWithStandardActions (root);
				DressRootWithBonusActions(root);
				DressRootWithInventoryActions (root);
				return root;			
			}

			private void DressRootWithStandardActions(ActionButtonNode root) {
				List<Action> actions = QueryActions ();
				foreach(Action action in actions) {

					ActionButtonNode actionRoot = action.GetOptionTreeRoot ();

					if (action.IsAttack) {
						if (AttacksLeft () <= 0) {
							actionRoot.disabled = true;
							actionRoot.disabledReason = "You cannot attack more than\n" + CharSheet.AttacksPerRound + " time(s) per turn.";
						} else if (UsedAction()) {

							actionRoot.disabled = true;
							actionRoot.disabledReason = "You've already taken an action this turn";

						}
					} else if(UsedAction() && !action.IsWait) {
						actionRoot.disabled = true;
						actionRoot.disabledReason = "You've already taken an action this turn";
					}

					//implements the rule where casting can't be done with unproficent equipment
					if (action is Cast) {
						if (!CharSheet.IsProficientInAllWornEquipment()) {
							actionRoot.disabled = true;
							actionRoot.disabledReason = "You cannot cast spells when wielding weapons or armour you are not proficient in";
						}
					}

					root.AddChild(actionRoot);
				}
			}

			private void DressRootWithBonusActions(ActionButtonNode root) {

				//Add bonus actions...
				List<Action> bonus = QueryBonusActions (CharSheet);

				//I think this is doing nothing..... since the folooring line does a select
				foreach (Action act in bonus) {
					ActionButtonNode ro = act.GetOptionTreeRoot();
				}
				ActionPointer p = new ActionPointer("Bonus Action", bonus.Select(( act)=>act.GetOptionTreeRoot()).ToList() );
				p.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.BONUS_ACTION);
				ActionButtonNode bonusRoot = p.GetRoot ();
				if (UsedBonusAction ()) {
					bonusRoot.disabled = true;
					bonusRoot.disabledReason = "You already used a bonus action this round";
				}
				root.AddChild(bonusRoot);
			}

			private void DressRootWithInventoryActions(ActionButtonNode root) {
				List<Action> inventoryActions = new List<Action> ();
				inventoryActions.Add (new PickUp (this));
				inventoryActions.Add (new DropItem(this));
				inventoryActions.Add (new Equip(this));
				inventoryActions.Add (new GiveItem (this));
				List<ActionButtonNode> nodes = inventoryActions.Select ((ac) => {
					//					actions.Remove(ac);
					ActionButtonNode actionRoot =  ac.GetOptionTreeRoot ();

					if (UsedInteraction()) {
						if (UsedAction ()) {
							actionRoot.disabled = true;
							actionRoot.disabledReason = "You've already taken an action\n and an interaction this turn.";
						} else {
							//Allows the interaction to happen as an action.
							ac.IsInteraction = false;
						}
					}


					return actionRoot;
				}).ToList();

				ActionPointer inventory = new ActionPointer("Inventory", nodes);
				inventory.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.INVENTORY);
				root.AddChild (inventory.GetRoot());
			}




			public void ActionFromResolutionPath(ActionBar bar, List<int> path) {
				bar.OnPathResolved -= ActionFromResolutionPath;
				ActionButtonNode root = ProduceRootActionButtonNode();

				Action ret = null;
				ActionButtonNode trav = root;
				int pathI = 0;
				int childI = path[pathI];

				//find the action first...
				while (ret == null) {
					trav = trav.children [childI];
					if (trav.choice is Action) {
						ret = trav.choice as Action;
					}
					pathI++;
					if(pathI < path.Count)
						childI = path [pathI];
				}


				//now fill the options
				foreach(ActionOption opt in ret.ActionOptions) {
					trav = trav.children [childI];


					opt.chosenChoice = trav.choice;
					pathI++;
					if(pathI < path.Count)
						childI = path [pathI];
				}

				ret.LateSetTargetParameters ();
				CallOnReadyToFillParams(ret);
			}

			public delegate void ReadyToFillParamsAction(Actor self, Action a);
			public event ReadyToFillParamsAction OnReadyToFillParams;
			public void CallOnReadyToFillParams(Action a) {
	//			Debug.Log ("on ready: " + a.GetType ().ToString ());
	//			Debug.Log ("filled? " + a.OptionsFilled);
				if (OnReadyToFillParams != null) {
					OnReadyToFillParams (this, a);
				}
			}


			public bool Waited() {
				foreach (Action a in allActionsThisRound) {
					if (a.IsWait)
						return true;
				}
				return false;
			}

			void FinishedGettingOpportunityAttacked (Action a)
			{
				a.OnFinished -= FinishedGettingOpportunityAttacked;
				GetComponent<AnimationTransform> ().Unpause ();
			}


			public bool CanStillAct() {
				if (Waited ()) {
					return false;
				}

				if (CanMove ()) {
					return true;
				}

				if (HasActionOptions ())
					return true;
				
				return false;

			}

			public bool HasActionOptions() {
				ActionButtonNode n = ProduceRootActionButtonNode ();
				bool ret = false;
				if (n.children.Count > 0) 	{ 
					foreach (ActionButtonNode child in n.children) {
						if (!child.disabled) {
							ret = true;
							break;
						}
					}
				}
				return ret;

			}


			public bool CanMove() {
					
				foreach (ATTile n in tileMovement.occupying.Neighbors()) {
					if (n.MoveCostFor (this) <= MovesLeft () && n.Occupyable())
						return true;
				}
				return false;
			}

			public bool CanAttack() {
				if (AttacksLeft() <= 0)
					return false;

				if (actionThisRound != null && !(actionThisRound is Attack)) {
					return false;
				}
				

				return true;
			}


			public List<Action> QueryActions() {
				List<Action> opts = new List<Action> ();
				CharSheet.ProduceActions (this, opts);
				foreach (Action act in opts) {
					act.actor = this;
				}

				return opts;
			}


			public List<Action> QueryBonusActions(Sheet a) {
				List<Action> opts = new List<Action> ();
				a.ProduceBonusActions (this, opts);
				foreach (Action act in opts) {
					act.actor = this;
				}
			
				return opts;
			}

			public string ActionsLeftDescription() {
				string desc = "-Actions Taken-";

				
				desc+="\nPrimary: ";
				if (actionThisRound != null) {
					desc += actionThisRound.GetType ().ToString ();		
				}else {
					desc += " - ";
				}

				desc += "\nBonus: ";
				if (bonusActionThisRound != null) {
					desc += bonusActionThisRound.GetType ().ToString ();		
				} else {
					desc += " - ";
				}

				desc += "\nInteract: ";
				if (interactionThisRound != null) {
					desc += interactionThisRound.GetType ().ToString ();		
				}else {
					desc += " - ";
				}

				desc += "\nReaction: ";
				if (reactionThisRound != null) {
					desc += reactionThisRound.GetType ().ToString ();		
				}else {
					desc += " - ";
				}
				return desc;

			}




			public bool EnemiesWith(Actor other) {
				BattleManager.Side mine = BattleManager.instance.SideFor (this);
				BattleManager.Side others = BattleManager.instance.SideFor (other);

				if (others == mine)
					return false;
				else
					return true;
			}


			// Use this for initialization
			private Gauge currentInitiative;

			public Gauge CurrentInitiative {
				get { return currentInitiative; }
			}


			public void RollInitiative() {
				currentInitiative = CharSheet.ProduceAndRollInitiative ();
				Debug.Log (CharSheet.Name + " rolled initiative: " + currentInitiative.ToString());
			}

			public void AggregateActionsThisRound (Action a) {
				if (a.IsMove) { //These are tracked differently
					return;
				}

				if (a.IsReaction) {
					//Debug.Log ("Used reaction action!");
					reactionThisRound = a;
				} else if (a.IsBonus) {
					//Debug.Log ("Used bonus action!");
					bonusActionThisRound = a;
				} else if (a.IsAttack) {
					//Debug.Log ("Used attack action (hence action)!");
					actionThisRound = a;
					attacksThisRound += 1;
				
				} else if (a.IsInteraction) {
					interactionThisRound = a;
				} else {
					//Debug.Log ("Used up action!");
					actionThisRound = a;
				}

				allActionsThisRound.Add (a);
			}




			

			public bool UsedAction() {
				return actionThisRound != null;
			}

			public bool UsedReaction() {
				return reactionThisRound != null;
			}

			public bool UsedBonusAction() {
				return bonusActionThisRound != null;
			}

			public bool UsedInteraction() {
				return interactionThisRound != null;
			}

			public bool HasEnemyReachersIn(List<ATTile> path) {
				foreach (ATTile t in path) {
					if (t.HasEnemyReachers (this)) {
						return true;
					}
				}


				return false;
			}




			public delegate void WillPerformAction(Action a);
			public event WillPerformAction OnWillPerform;
			public void CallOnWillPerform(Action a) {

				if (OnWillPerform != null) {
					OnWillPerform (a);
	                
				}

	            CharSheet.CallOnWillPerform(a);
			}

			public delegate void DidPerformAction(Action a);
			public event DidPerformAction OnDidPerform;
			public void CallOnDidPerform(Action a) {
				if (OnDidPerform != null) {
					OnDidPerform (a);
				}

	            CharSheet.CallOnDidPerform(a);
	        }

			private Sheet charSheet;
			public Sheet CharSheet {
				get { 
					return charSheet;
				}

				set { charSheet = value; }


			}


			private TileMovement tileMovement;
			public TileMovement TileMovement {
				get { return tileMovement; }
			}

			[SerializeField]
			private int movesThisRound;
			public int MovesLeft() {
				return charSheet.MovementSpeed - movesThisRound;
			}

			[SerializeField]
			private int attacksThisRound;
			public int AttacksLeft() {
				return charSheet.AttacksPerRound - attacksThisRound;
			}



			public delegate void ActorDeathAction(Actor s);
			public event ActorDeathAction OnActorKilled;
			public void ActorDied() {
				GetComponent<CharacterWalker> ().Interrupt ();
				GetComponent<AnimationTransform> ().Play ("Die");
				GetComponent<AnimationTransform> ().OnAnimationEvent += DiedAnimationFinished;

				if (OnActorKilled != null) {
					OnActorKilled (this);
				}
			}

			private IEnumerator DiedStuffToDelay() {

	//			yield return new WaitForSeconds (.7f);
				float currentAlpha = 1f;
				while (currentAlpha > 0f) {
					yield return null;
					currentAlpha -= Time.deltaTime;

					GetComponent<AnimationTransform> ().SetAlpha (currentAlpha);
				}

				yield return null;
				Destroy (gameObject, .5f);
			}


			public bool TurnInProgress {
				get;
				set;
			}


			private void DiedAnimationFinished(AnimationEventType type, string name) {
				GetComponent<AnimationTransform> ().Pause ();
				GetComponent<AnimationTransform> ().OnAnimationEvent -= DiedAnimationFinished;

				StartCoroutine (DiedStuffToDelay ());
			}

			public delegate void ActorTurnBeganAction(Actor s);
			public event ActorTurnBeganAction OnTurnBegan;
			public void ActorTurnBegan() {
				if (!TurnInProgress) {
					CharSheet.TurnBegan (this);
					TurnInProgress = true;
				}

				if (OnTurnBegan != null) {
					OnTurnBegan (this);
				}
			}

			public Vision Eyes {
				get { 
					return GetComponent<Vision> ();
				}
			}
		}






	}
}