using UnityEngine;
using System.Collections;
using AT.Character.Effect;


namespace AT.Character.Situation {
	
	public enum ResultType {
		HIT,
		CRITICAL_HIT,
		CRITICAL_MISS,
		MISS,
		SUCCESS,
		FAILURE
	};

	public enum SkillType {
		//Str
		ATHLETICS, //jump and 

		//Dex
		STEALTH, //hide
		//PICKPOCKETING,
		ACROBATICS, //avoid stuff 

		//Int
		INVESTIGATION, //allows seeing of certain stats, like hp/etc...  allows the party to make more informed decisions
		//ARCANA,
		NATURE,
		//RELIGION,
		//HISTORY,

		//Wis
		//ANIMAL_HANDLING,
		//INSIGHT,
		MEDICINE, //help revive fallen (active)
		PERCEPTION, //can see through concealment easier, sees hidden stuff. (passive)
		//SURVIVAL, 

		//Cha
		//INTIMIDATION, //intimidate (active)
		PERFORMANCE, //feint (active)
		//PERSUASION,
		//DECEPTION, 

		NULL
	};



	public enum AbilityType {
		STRENGTH,
		DEXTERITY,
		CONSTITUTION,
		INTELLIGENCE,
		WISDOM,
		CHARISMA,
	}

	public interface DcProducer {
		void ProduceDcInCheck(CheckSituation cr);
		void AboutToResolveDcInCheck (CheckSituation cr);
		void DidResolveDcInCheck (CheckSituation cr);


		void ProduceDcInSave(SaveSituation cr);
		void AboutToResolveDcInSave (SaveSituation cr);
		void DidResolveDcInSave (SaveSituation cr);
	}

	//produces 1 check and success or not.
	public interface CheckLikeSituation : AT.Battle.ProvidesBattleTranscript {
		ResultType GetResult(bool isSample);
		ResultType CalculateResult (Gauge check, Gauge dc);

		bool AdvantageFlagged();
		bool DisadvantageFlagged();

	}



	//produces 2 checks and a winner...
	public interface  ContestLikeSituation : AT.Battle.ProvidesBattleTranscript {
		Sheet GetWinner();
		Sheet CalculateWinner (Sheet contestant1, Gauge check1, Sheet contestant2, Gauge check2);
	}

}