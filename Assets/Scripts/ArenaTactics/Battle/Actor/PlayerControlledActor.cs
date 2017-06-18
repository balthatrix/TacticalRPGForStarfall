using UnityEngine;
using System.Collections;

namespace AT.Battle {
	
	public class PlayerControlledActor : Actor {
		public static int instCount;

		public override void  Awake() {
			base.Awake();
			GetComponent<Vision>().OnTileVisionGained += (ATTile tile) => {
				//this should only happen if the vision is already gained.
				tile.ClearFOW();

			};
			GetComponent<Vision>().OnTileVisionLost += (ATTile tile) => {
				if(!tile.SeenByAlliesOf(this)) {
					tile.EnableFOW(); //this should only happen if the tile is not in the collective vision
				}



			};
		}


		// Use this for initialization
		public override void Start () {
			base.Start ();
			instCount++;

//			Debug.Log ("Player reporting in!");
			BattleManager.instance.ReportInAs (BattleManager.Side.PLAYER, this);		



			OnTurnBegan += SetStatsPanel;

			OnTurnBegan += ShowIndicators;
			OnWillPerform += HideIndicators;

//			OnWillPerform += HideAndCleanup;

		}

		void ShowIndicators (AT.Battle.Actor a)
		{
//			Debug.LogError ("ma type: " + GetType ());
			foreach (Actor other in BattleManager.instance.AllActors()) {
				CharacterIndicator otherInd = other.GetComponent<CharacterIndicator> ();

				if (other.IsOnPlayerSide) {
					otherInd.ShowIndicator ();
					otherInd.ColorIndicator (new Color (0f, 1f, 0f));
				} else {
					if (other.IsSeenByAPlayer) {
						otherInd.ShowIndicator ();
						otherInd.ColorIndicator (new Color (1f, 0f, 0f));
					}
				}
			}

			GetComponent<CharacterIndicator> ().ColorIndicator (new Color (0f, 1f, 0f));
			GetComponent<CharacterIndicator> ().StartBlinking ();
		}


		void HideIndicators (Action a)
		{
			foreach (Actor other in BattleManager.instance.AllActors()) {
				CharacterIndicator otherInd = other.GetComponent<CharacterIndicator> ();
				otherInd.HideIndicator ();
			}

			GetComponent<CharacterIndicator> ().HideIndicator ();
			GetComponent<CharacterIndicator> ().StopBlinking();
		}


		public void OnDestroy() {
			OnTurnBegan -= SetStatsPanel;
		}

		public void SetStatsPanel (Actor me) {
//			UIManager.instance.statsPanel.gameObject.SetActive (true);
//			me.CharSheet.OnDamaged += UpdateHp;
//			UIManager.instance.statsPanel.name.text = CharSheet.Name;
//			UIManager.instance.statsPanel.hp.text = CharSheet.HitPointsGauge.ModifiedCurrent + "/" + CharSheet.HitPointsGauge.ModifiedMax;

			//TODO: Update this when you equip something....
//			UIManager.instance.statsPanel.ac.text = CharSheet.HypotheticalAC ().ToString ();
			//TODO: Update this when you equip something....
//			UIManager.instance.statsPanel.att.text = SignedBonusString(CharSheet.HypotheticalToHitBonus ());


			//TODO: Update this when you equip something....
//			if (me.CharSheet.OffHand () != null) {
//				UIManager.instance.statsPanel.offhandPanel.SetActive (true);
//				UIManager.instance.statsPanel.offhandAttack.text = SignedBonusString(CharSheet.HypotheticalOffhandToHitBonus ());
//			} else {
//				UIManager.instance.statsPanel.offhandPanel.SetActive (false);
//			}
		}

		public string SignedBonusString(int bonus) {
			string sign;
			if (bonus == 0) {
				sign = "";
			} else if (bonus > 0) {
				sign = "+";
			} else {
				sign = "-";
			}
			return sign + bonus.ToString ();
		}

//		public void UpdateHp(Infliction.Damage d, Action source = null) {
//			UIManager.instance.statsPanel.hp.text = CharSheet.HitPointsGauge.ModifiedCurrent + "/" + CharSheet.HitPointsGauge.ModifiedMax;
//		}
//
//		public void HideAndCleanup(Action a) {
//			CharSheet.OnDamaged -= UpdateHp;
//			UIManager.instance.statsPanel.gameObject.SetActive (false);
//		}

	}

}