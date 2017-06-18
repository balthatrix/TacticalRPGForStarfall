using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Character;
using AT.Character.Situation;
using System.Linq;


namespace AT {

namespace Battle {  


	public class GiveItem : Interaction {
		
		public static int GIVE_DISTANCE {
			get { return 2; } 
		}
			public override void DecorateOption(ActionButtonNode n) {
				n.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.GIVE);
			}

		public GiveItem(Actor actor=null) : base(actor) {
			this.ActionOptions.Add (new ItemsOnPersonOption ());

			ActionTargetTileParameter param = new ActionTargetTileParameter (this, GIVE_DISTANCE);
			param.targetTileFilters.Add(HasFriendlyAndFriendlyHasInventoryAndInteraction);
			param.OnListen += (List<ATTile> potentialTargets) => {
				//color green
				MapManager.instance.ColorTiles (potentialTargets, new Color(0f, 1f, 0f, .3f));
			};


			param.OnStopListen += (List<ATTile> potentialTargets) => {

				MapManager.instance.UnColorTiles (potentialTargets);

			};

			this.actionTargetParameters.Add (param);
		}

		private List<ATTile> HasFriendlyAndFriendlyHasInventoryAndInteraction(List<ATTile> tiles, Actor actr) {
			List<ATTile> ret = ActionTargetTileParameter.HasFriendlyCharacter(tiles, actr)
				.Where((tile) => {
					bool actingPossible = (!tile.FirstOccupant.ActorComponent.UsedInteraction() || 
										   !tile.FirstOccupant.ActorComponent.UsedAction());
					bool inventoryFree = !tile.FirstOccupant.ActorComponent.CharSheet.inventory.NoRoomLeft;
					return actingPossible && inventoryFree;
				}).ToList();

			return ret;
		}

		public override void Perform() {
			CallOnBegan ();

			Debug.LogError ("AM I AN INTERACTION??? " + IsInteraction);

			ItemsOnPersonOption opt = ActionOptions [0] as ItemsOnPersonOption;
			InventoryItemChoice choice = opt.chosenChoice as InventoryItemChoice;

			ActionTargetTileParameter param = actionTargetParameters [0];

			List<InventoryItem> items = actor.CharSheet.inventory.ListOfItems ();
			InventoryItem ch = choice.item;
			if (items.Contains (ch)) {
				actor.CharSheet.inventory.RemoveItem (choice.item);
			} else if (actor.CharSheet.PaperDoll.slots.ContainsValue (choice.item as Equipment)) {
				actor.CharSheet.PaperDoll.Unequip(choice.item as Equipment, actor.CharSheet);
			}

			Actor targetTaker = param.chosenTile.FirstOccupant.ActorComponent;

			PickUp compoundAction = new PickUp (targetTaker);
			if (targetTaker.UsedInteraction ()) {
				compoundAction.IsInteraction = false;
			}
			compoundAction.ActionOptions [0].chosenChoice = new InventoryItemChoice (choice.item) as IActionOptionChoice;

			compoundAction.Perform ();
//			TileMovement tm = actor.GetComponent<TileMovement> ();
//			tm.occupying.onTheGround.Add (choice.item);

			CallOnFinished ();
		}

	}

}
}