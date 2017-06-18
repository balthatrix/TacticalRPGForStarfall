using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using AT.Battle;
using AT.Character;
using System.Linq;
using UnityEngine.EventSystems;
using Util;

namespace AT {
	[RequireComponent (typeof (BoxCollider2D))]
	public class ATTile : MonoBehaviour {

		[SerializeField]
		private Transform onTheGroundParent; 

		public delegate void ClickAction(ATTile t);
		public event ClickAction OnClicked;

		public delegate void MouseOverAction(ATTile t);
		public event MouseOverAction OnMouseOverEvent;

		public delegate void MouseOutAction(ATTile t);
		public event MouseOutAction OnMouseOutEvent;

		private List<InventoryItem> onTheGround;

		private GameObject fogOfWar;


		/// <summary>
		///The actors that see this tile.  This is a useful optimization that
		/// allows tiles to inform actors when another actor sets foot in it, for things like discovery.
		/// </summary>
		public List<Actor> actorsThatSeeThisTile = new List<Actor> ();
		public void AddViewer(Actor a) {
			actorsThatSeeThisTile.Add (a);
			if (OnViewerAdded != null) {
				OnViewerAdded (a);
			}
		}

		public void RemoveViewer(Actor a) {
			actorsThatSeeThisTile.Remove (a);
			if (OnViewerRemoved != null) {
				OnViewerRemoved (a);
			}
		}

		public void ClearFOW() {
			fogOfWar.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,0f);

		}
		public void HalfFOW() {
			fogOfWar.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f, 0.5f);
		}
		public void EnableFOW() {
			fogOfWar.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
		}

		public bool isDifficultTerrain;
		public bool IsDifficultTerrain {
			get { return isDifficultTerrain; }
			set { isDifficultTerrain = value; }
		}

		public bool blocksVision;
		public bool BlocksVision {
			get { return blocksVision; }
			set { blocksVision = value; }
		}

		public bool blocksMissiles;
		public bool BlocksMissiles {
			get { return blocksMissiles; }
			set { blocksMissiles = value; }
		}


		public bool SeenByAlliesOf(Actor a) {
			bool ret = false;
			foreach (Actor actorSees in actorsThatSeeThisTile) {
				if (!actorSees.EnemiesWith (a)) {
					ret = true;
					break;
				}
			}
			return ret;
		}

		/// <summary>
		/// Gets the on the ground copy of list.  You cannot add or remove to this, use AddItemToGround or RemoveItemFromGround for that.
		/// </summary>
		/// <value>The on the ground.</value>
		public List<InventoryItem> OnTheGround {
			get { 
				return onTheGround.ToList ();
			}
		}

		public void AddItemToGround(InventoryItem item) {
			onTheGround.Add (item);
			//refresh ground icon(s)
			RefreshGroundItems();
		}

		public void RemoveItemFromGround(InventoryItem item) {
			onTheGround.Remove (item);
			//refresh ground icon(s)
			RefreshGroundItems();
		}


		private void RefreshGroundItems() {
			for (int i = 0; i < onTheGroundParent.childCount; i++) {
				Destroy (onTheGroundParent.GetChild (i).gameObject);
			} 

			foreach (InventoryItem item in OnTheGround) {
				GameObject obj = Instantiate (MapManager.instance.onTheGroundItemPrefab, onTheGroundParent);

				obj.GetComponent<SpriteRenderer> ().sprite = IconDispenser.instance.SpriteFromIconName (item.IconType);
				obj.transform.localPosition = Vector3.zero;
			}
		}

		private Color startingColor;

		private List<MaskLayer> masks; 


		//actors who have reach into this tile....
		[SerializeField]
		private List<Actor> reachers;

		void Awake() {
			Initialize (MapManager.instance);
			onTheGround = new List<InventoryItem> ()  { 
				//new GenericArmour(EquipmentSubtype.ARMOUR_CHAIN) <- was just testing
			};


		}
		void Start() {
			MapManager.instance.TryCheckInTile (this);

		}

		bool initializedYo = false;
		void Initialize(MapManager inst) {
			if (initializedYo) {
				Debug.LogError ("already fooo");
				return;
			}
			initializedYo = true;


			gameObject.layer = LayerMask.NameToLayer ("Tile");

			masks = new List<MaskLayer> ();
			DestroyAllTileMask ();
			Collider2D coll = GetComponent<Collider2D> ();
			coll.isTrigger = true;

			currentOccupants = new List<TileMovement> ();
			startingColor = new Color(1f, 1f, 1f, 0f);
			reachers = new List<Actor> ();


			//prevents the obvious rounding errors with tiles:
			transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y, 1f);
			OnOccupantAdded += ATTile_OnOccupantAdded;
			OnOccupantRemoved += ATTile_OnOccupantRemoved;
			//MapManager.OnAwake -= Initialize;
			fogOfWar = Instantiate(MapManager.instance.fogOfWarMask);
			fogOfWar.transform.SetParent (transform, false);
		}


		private List<TileMovement> stumblingOcc = new List<TileMovement>();
		void ATTile_OnOccupantRemoved (TileMovement occ, ATTile destination)
		{
			if (IsDifficultTerrain) { 
				stumblingOcc.Remove(occ);
				occ.ActorComponent.CharSheet.OnAboutToAttack -= CursedWillPerform;
				occ.ActorComponent.CharSheet.OnAboutToBeAttacked -= CursedWillBeAttacked;
//				Debug.LogError ("bla bla");
			}

			//have actors that can see this tile unsee the actor that was removed if they can't see the destination tile and can see enemy.
			foreach (Actor act in actorsThatSeeThisTile) {
				Vision eyes = act.GetComponent<Vision> ();
				if(eyes.CanSee(occ.ActorComponent) && act != occ.ActorComponent) {
					if (destination == null) { //the actor probably died.
						Debug.Log("actor: " + eyes.Actor.GetType());
						eyes.UndiscoverEnemyActor (occ.ActorComponent);
					} else if (act.EnemiesWith (occ.ActorComponent)) {
						if(!destination.actorsThatSeeThisTile.Contains(act))
							eyes.UndiscoverEnemyActor (occ.ActorComponent);
					}
				}
			}
		}

		void ATTile_OnOccupantAdded (TileMovement occ, ATTile previous)
		{
			if (IsDifficultTerrain) {
				stumblingOcc.Add (occ);
				occ.ActorComponent.CharSheet.OnAboutToAttack += CursedWillPerform;
				occ.ActorComponent.CharSheet.OnAboutToBeAttacked += CursedWillBeAttacked;
			} 

			//have actors that can see this tile discover the actor that was added if they don't already see it.
			foreach (Actor act in actorsThatSeeThisTile) {
				if (occ.ActorComponent == act)
					continue;
				Vision eyes = act.GetComponent<Vision> ();
				if (act.EnemiesWith (occ.ActorComponent) && !eyes.CanSee(occ.ActorComponent)) {
					eyes.DiscoverEnemyActor (occ.ActorComponent);
				}
			}
		}

		void CursedWillPerform(AT.Character.Situation.AttackSituation sit) {
//			Debug.Log ("Disi!");
			sit.FlagDisadvantage ();
		}

		void CursedWillBeAttacked(AT.Character.Situation.AttackSituation sit) {
//			Debug.Log ("Disi!");
			sit.FlagAdvantage ();
		}




		void DestroyAllTileMask () {
			for(int i = 0; i < transform.childCount; i ++){
				Transform t = transform.GetChild (i);
				if (t.name.Contains ("TileMask")) {
					Destroy (t.gameObject);
				}
			}
		}

		public void PopShadeStack() {
			if (masks.Count > 0) {
				MaskLayer last = masks [masks.Count - 1];

				masks.Remove(last);
				Destroy (last.gameObject);
			}
		}

		public void AddReacher(Actor a) {
			reachers.Add (a);
			a.OnActorKilled += OnReacherKilledCleanup;
		}

		public void OnReacherKilledCleanup(Actor a) {
			RemoveReacher (a);
			a.OnActorKilled -= OnReacherKilledCleanup;
		}


		public void RemoveReacher(Actor a) {
			reachers.Remove (a);
		}

		public List<Actor> Reachers {
			get { return reachers; }
		}

		public void PushShade(Color c) {
			
			GameObject M = Instantiate (MapManager.instance.tileMask);
			MaskLayer ml= M.GetComponent<MaskLayer> ();
			ml.Color (c);
			if (masks.Count > 0) {
				ml.SetParent (masks [masks.Count - 1]);
			} else {
				ml.transform.SetParent (transform,false);
				ml.transform.localPosition = Vector3.zero;
				ml.transform.localScale = ml.transform.localScale * ml.scaleFactor;
			}

			masks.Add (ml);
		}


		public bool WithinReach(Actor a) {
			if (reachers.Contains (a)) {
				return true;
			}

			return false;
		}

		public bool WithinAttackRangeOf(Actor a) {
			bool reach = WithinReach (a);
			if (reach)
				return true;
			if (a.CharSheet.MainHand ().IsRanged () || a.CharSheet.MainHand ().IsThrown ()) {
				int cost = this.HCostTo (a.GetComponent<TileMovement>().occupying);
				AT.Character.GenericWeapon wep = a.CharSheet.MainHand ();
				if (wep.MaxRng >= cost) {
					return true;
				}
			}


			return false;
		}

		public bool overriddenAsNotWalkable = false;

		[SerializeField]
		public int baseMoveCost = 1;

		public bool Occupyable() {
			return (!overriddenAsNotWalkable && FirstOccupant == null && !HasUnwalkableProps());
		}

		public bool Travellable(Actor a) {
			//in 5e, you can travel on tiles that have friendlies, but they cost additional 5 ft of movement.
			return Occupyable() || (FirstOccupant != null && !FirstOccupant.GetComponent<Actor>().EnemiesWith(a));
		}

		public bool HasUnwalkableProps() {
			
			for (int i = 0; i < transform.childCount; i++) {
				Transform c = transform.GetChild (i);

				SpriteRenderer sr = c.GetComponent<SpriteRenderer> ();
				if (sr == null) {
					continue;
				}
				if (sr.sortingLayerName == "TilePropUnwalkable" || sr.tag == "TilePropUnwalkable")
					return true;
			}
			return false;
		}

		//move cost is in units of 5 ft.
		public int MoveCostFor(Actor a) {
			//Should later return a val based on movement policies in 5e
			int ret = baseMoveCost;
			if (FirstOccupant != null && a.GetComponent<TileMovement>() != FirstOccupant)
				ret += 1;
			return ret;
		}

		public bool HasEnemyReachers(Actor a) {
			foreach (Actor actor in Reachers) {
				if(a.EnemiesWith(actor))
					return true;
			}
			return false;
		}


		//used to avoid 
		public int PerceivedMoveCostFor(Actor a) {
			int ret = MoveCostFor (a);
			foreach (Actor en in Reachers) {
				if (en.EnemiesWith (a))
					ret += 1;
			}
			return ret;
		}

		//pathParent is for pathfinding
		//This works because it's a turn based game	
		//It might be concurrent frame safe, but it's definitely not thread safe.
		//two pathfinders would be setting g/h/parents on the same Tiles in memory
		//Nessecary?
		public ATTile pathParent;
		public float gCost;
		public float hCost;
		public float FCost() {
			return gCost + hCost;
		}

		public List<ATTile> TracePathTo(ATTile aStarredTarget){
			ATTile current = aStarredTarget;
			List<ATTile> ret = new List<ATTile>();
			while (current != this) {
				if (current.pathParent == null) {
					return null;
				}
				ret.Add (current);
				current = current.pathParent;
			}
			ret.Reverse ();
			return ret;
		}


		[SerializeField]
		private List<TileMovement> currentOccupants;

		public List<TileMovement> AllOccupants {
			get { return currentOccupants.ToList (); }
		}

		public TileMovement FirstOccupant
		{
			get { 
				if (currentOccupants.Count == 0) {
					return null;
				}
				return currentOccupants [0]; 
			}
		}

		public TileMovement LastOccupant {
			get { 
				if (currentOccupants.Count == 0) {
					return null;
				}
				return currentOccupants [currentOccupants.Count - 1]; 
			}
		}

		public void RemoveOccupant(TileMovement occ, ATTile desitination) {
			currentOccupants.Remove (occ);
			occ.ActorComponent.OnActorKilled -= OnDeathOccupantCleanup;
			if (OnOccupantRemoved != null) {

				OnOccupantRemoved (occ, desitination);
			}

		}

		public void AddOccupant(TileMovement occ, ATTile previous) {
			
			occ.ActorComponent.OnActorKilled += OnDeathOccupantCleanup;
			currentOccupants.Add (occ);

			if(OnOccupantAdded != null) {
				
				OnOccupantAdded (occ, previous);	
			}
		}

		public delegate void OccupantAddedAction(TileMovement occ, ATTile previous);
		public event OccupantAddedAction OnOccupantAdded;
		public delegate void OccupantRemovedAction(TileMovement occ, ATTile destination);
		public event OccupantRemovedAction OnOccupantRemoved;

		public delegate void ViewerAddedAction(Actor actor);
		public event ViewerAddedAction OnViewerAdded;
		public delegate void ViewerRemovedAction(Actor actor);
		public event ViewerRemovedAction OnViewerRemoved;

		public void OnDeathOccupantCleanup(Actor a) {
			RemoveOccupant (a.TileMovement, null);
		}

		public float DistanceSquared(ATTile to) {
			return Mathf.Pow (to.X () - X (), 2f) +
				   Mathf.Pow (to.Y () - Y (), 2f);
		}


		//TODO: Move distance might want to incorporate
		public int HCostTo(ATTile to) {
			return (int)Mathf.Round (Mathf.Abs (to.X() - X()) + Mathf.Abs(to.Y() - Y()));
		}

		public float X() {
			return transform.position.x;
		}

		public float Y() {
			return transform.position.y;
		}

		public List<ATTile> Neighbors() {
			List<ATTile> ret = new List<ATTile> ();
			ATTile up = Up ();
			ATTile down = Down ();
			ATTile left = Left ();
			ATTile right = Right ();

			if (up != null)
				ret.Add (up);

			if (down != null)
				ret.Add (down);

			if (left != null)
				ret.Add (left);

			if (right != null)
				ret.Add (right);

			return ret;
		}

		public ATTile Up() {
			return MapManager.instance.TileAt (new Vector2 (transform.position.x, transform.position.y + 1f));
		}

		public ATTile Down() {
			return MapManager.instance.TileAt (new Vector2 (transform.position.x, transform.position.y - 1f));
		}

		public ATTile Left() {
			return MapManager.instance.TileAt (new Vector2 (transform.position.x - 1f, transform.position.y));
		}

		public ATTile Right() {
			return MapManager.instance.TileAt (new Vector2 (transform.position.x + 1f, transform.position.y));
		}
			

		void LogNeighbors() {
			foreach (ATTile n in Neighbors()) {
				Debug.Log ("Neighbor: " + n.transform.position.ToString());
			}
		}

		public List<ATTile> TilesWithinRange(int range) {
			List<ATTile> tiles = new List<ATTile> ();


			Vector3 bottomLeftOfRange = new Vector3 (transform.position.x - range,	transform.position.y - range);
			int checks = (range * 2) + 1;
			for(int i = 0; i < checks; i++) {
				for(int j = 0; j < checks; j++) {
					ATTile prospect = MapManager.instance.TileAt (bottomLeftOfRange + new Vector3(i, j, transform.position.z));
					if (prospect == null || prospect.overriddenAsNotWalkable || prospect == this || prospect.HCostTo(this) > range) {
						//Debug.Log ("prospect is null");
						continue;
					}


					tiles.Add (prospect);
				}
			}
			return tiles;
		}

		public static bool PointerOverUi() {
			return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ();
		}

		void OnMouseDown() {
			
			if (OnClicked != null) {
				if (PointerOverUi ()) {
					return;
				}

				OnClicked (this);
			}



		}


		void OnMouseEnter() {
//			Debug.Log ("entered name: " + name);
			if (PointerOverUi ()) {
				return;
			}
			CallMouseOver ();
		}


		void OnMouseExit() {
//			Debug.Log ("exited name: " + name);
			if (PointerOverUi ()) {
				return;
			}
			CallMouseOut ();

		}


		/// <summary>
		/// whether this object thinks it has mouse over it.
		/// Seems redundant, but it's nessecary for odd edge cases where mouse crosses from ui to tile.
		/// </summary>
		public bool mousedOver = false;
		void CallMouseOut() {
			if (!mousedOver)
				return;
			
			mousedOver = false;
			if (OnMouseOutEvent != null) {
				
				OnMouseOutEvent (this);
			}
		}
		void CallMouseOver() {
			if (mousedOver)
				return;
			
			mousedOver = true;
			if (OnMouseOverEvent != null) {

				OnMouseOverEvent (this);
			}
		}


		void Update () {
			
			if (mousedOver && PointerOverUi ()) { //pointer went over ui
				
				CallMouseOut();

			}


		}



		string ToString() {
			return "Tilename: " + name + ", @" + transform.position.ToString ();
		}




	}

}