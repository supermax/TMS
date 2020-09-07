using System.Collections.Generic;
using TMS.Common.Serialization.Json;
using TMS.Common.Serialization.Json.Api;

namespace TMS.Common.Tests.Serialization.Json.TestClasses
{
	[JsonDataContract]
	public class HeroXp
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "id")]
		public int id { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "qty")]
		public int qty { get; set; }
	}

	[JsonDataContract]
	public class HeroSkillPoint
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "qty")]
		public int qty { get; set; }
	}

	[JsonDataContract]
	public class HskillGathering1
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "lvl")]
		public int lvl { get; set; }
	}

	[JsonDataContract]
	public class ArtiWeapon1
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
	}

	[JsonDataContract]
	public class Weapon
	{
		[JsonDataMember(Name = "arti_weapon_1")]
		public ArtiWeapon1 arti_weapon_1 { get; set; }
	}

	[JsonDataContract]
	public class ArtiAccess1
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
	}

	[JsonDataContract]
	public class Accessory
	{
		[JsonDataMember(Name = "arti_access_1")]
		public ArtiAccess1 arti_access_1 { get; set; }
	}

	[JsonDataContract]
	public class Hero
	{
        //[JsonDataMemberIgnore]
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }

		[JsonDataMember(Name = "id")]
		public int id { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "lvl")]
		public int lvl { get; set; }
		[JsonDataMember(Name = "name")]
		public string name { get; set; }
		[JsonDataMember(Name = "state")]
		public int state { get; set; }
		[JsonDataMember(Name = "loc")]
		public int loc { get; set; }
		[JsonDataMember(Name = "hero_xp")]
		public HeroXp hero_xp { get; set; }
		[JsonDataMember(Name = "hero_skill_point")]
		public HeroSkillPoint hero_skill_point { get; set; }
		[JsonDataMember(Name = "hskill_gathering_1")]
		public HskillGathering1 hskill_gathering_1 { get; set; }
		[JsonDataMember(Name = "weapon")]
		public Weapon weapon { get; set; }
		[JsonDataMember(Name = "accessory")]
		public Accessory accessory { get; set; }
	}

	[JsonDataContract]
	public class HeroBag
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "id")]
		public int id { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
	}

	[JsonDataContract]
	public class Crops
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "qty")]
		public int qty { get; set; }
		[JsonDataMember(Name = "cap")]
		public int cap { get; set; }
		[JsonDataMember(Name = "due")]
		public long due { get; set; }
	}

	[JsonDataContract]
	public class Stone
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "qty")]
		public int qty { get; set; }
		[JsonDataMember(Name = "cap")]
		public int cap { get; set; }
	}

	[JsonDataContract]
	public class Spirit
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "qty")]
		public int qty { get; set; }
		[JsonDataMember(Name = "cap")]
		public int cap { get; set; }
		[JsonDataMember(Name = "due")]
		public long due { get; set; }
	}

	[JsonDataContract]
	public class Alloy
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "qty")]
		public int qty { get; set; }
		[JsonDataMember(Name = "cap")]
		public int cap { get; set; }
	}

	[JsonDataContract]
	public class Flame
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "qty")]
		public int qty { get; set; }
		[JsonDataMember(Name = "cap")]
		public int cap { get; set; }
	}

	[JsonDataContract]
	public class Mist
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "qty")]
		public int qty { get; set; }
		[JsonDataMember(Name = "cap")]
		public int cap { get; set; }
	}

	[JsonDataContract]
	public class Void
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "qty")]
		public int qty { get; set; }
		[JsonDataMember(Name = "cap")]
		public int cap { get; set; }
	}

	[JsonDataContract]
	public class ArmyCamp
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "qty")]
		public int qty { get; set; }
		[JsonDataMember(Name = "cap")]
		public int cap { get; set; }
	}

	[JsonDataContract]
	public class HeroTrainer
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "qty")]
		public int qty { get; set; }
		[JsonDataMember(Name = "cap")]
		public int cap { get; set; }
	}

	[JsonDataContract]
	public class Resource
	{
		[JsonDataMember(Name = "crops")]
		public Crops crops { get; set; }
		[JsonDataMember(Name = "stone")]
		public Stone stone { get; set; }
		[JsonDataMember(Name = "spirit")]
		public Spirit spirit { get; set; }
		[JsonDataMember(Name = "alloy")]
		public Alloy alloy { get; set; }
		[JsonDataMember(Name = "flame")]
		public Flame flame { get; set; }
		[JsonDataMember(Name = "mist")]
		public Mist mist { get; set; }
		
	[JsonDataMember(Name = "army_camp")]
	public ArmyCamp army_camp { get; set; }
	[JsonDataMember(Name = "hero_trainer")]
	public HeroTrainer hero_trainer { get; set; }
}





	[JsonDataContract]
public class MissionsRenewer
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "due")]
	public long due { get; set; }
}

[JsonDataContract]
public class EagleMission
{
	
	}

	[JsonDataContract]
public class HeroTroop
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class CavT1
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class Troops
{
	[JsonDataMember(Name = "hero_troop")]
	public HeroTroop hero_troop { get; set; }
	[JsonDataMember(Name = "cav_t1")]
	public CavT1 cav_t1 { get; set; }
}

[JsonDataContract]
public class TrapRngT2
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class TrapInfT2
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class Traps
{
	[JsonDataMember(Name = "trap_rng_t2")]
	public TrapRngT2 trap_rng_t2 { get; set; }
	[JsonDataMember(Name = "trap_inf_t2")]
	public TrapInfT2 trap_inf_t2 { get; set; }
}

[JsonDataContract]
public class MyGarrison
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "home")]
	public string home { get; set; }
	[JsonDataMember(Name = "name")]
	public string name { get; set; }
	[JsonDataMember(Name = "state")]
	public int state { get; set; }
	[JsonDataMember(Name = "loc")]
	public int loc { get; set; }
	[JsonDataMember(Name = "troops")]
	public Troops troops { get; set; }
	[JsonDataMember(Name = "traps")]
	public Traps traps { get; set; }
}

[JsonDataContract]
public class Trg
{
	
	}

	[JsonDataContract]
public class a457946592969077
	{
		[JsonDataMember(Name = "uid")]
public string uid { get; set; }
[JsonDataMember(Name = "id")]
public long id { get; set; }
[JsonDataMember(Name = "alliance")]
public string alliance { get; set; }
[JsonDataMember(Name = "home")]
public string home { get; set; }
[JsonDataMember(Name = "vel")]
public int vel { get; set; }
[JsonDataMember(Name = "name")]
public string name { get; set; }
[JsonDataMember(Name = "state")]
public int state { get; set; }
[JsonDataMember(Name = "loc")]
public int loc { get; set; }
[JsonDataMember(Name = "due")]
public long due { get; set; }
[JsonDataMember(Name = "trg")]
public Trg trg { get; set; }
[JsonDataMember(Name = "cmd")]
public string cmd { get; set; }

	}

	[JsonDataContract]
public class MyMovingArmy
{
	[JsonDataMember(Name = "457946592969077")]
	public a457946592969077 a457946592969077 { get; set; }
	}

	[JsonDataContract]
public class MyReinforcingArmy
{
	[JsonDataMember(Name = "Remarks")]
	public string Remarks { get; set; }
	}

	[JsonDataContract]
public class Sup
{
	[JsonDataMember(Name = "Remarks")]
	public string Remarks { get; set; }
	}

	[JsonDataContract]
public class a1424902645088350
	{
		[JsonDataMember(Name = "uid")]
public string uid { get; set; }
[JsonDataMember(Name = "alliance")]
public string alliance { get; set; }
[JsonDataMember(Name = "home")]
public string home { get; set; }
[JsonDataMember(Name = "vel")]
public int vel { get; set; }
[JsonDataMember(Name = "name")]
public string name { get; set; }
[JsonDataMember(Name = "state")]
public int state { get; set; }
[JsonDataMember(Name = "loc")]
public int loc { get; set; }
[JsonDataMember(Name = "due")]
public long due { get; set; }
[JsonDataMember(Name = "sup")]
public Sup sup { get; set; }

	}

	[JsonDataContract]
public class MyGatheringArmy
{
	[JsonDataMember(Name = "1424902645088350")]
	public a1424902645088350 a1424902645088350 { get; set; }
	}

	[JsonDataContract]
public class MyMovingCaravan
{
	[JsonDataMember(Name = "Remarks")]
	public string Remarks { get; set; }
	}

	[JsonDataContract]
public class EnemyIncoming
{
	[JsonDataMember(Name = "Remarks")]
	public string Remarks { get; set; }
	}

	[JsonDataContract]
public class WoundedArmy
{
	[JsonDataMember(Name = "Remarks")]
	public string Remarks { get; set; }
	}

	[JsonDataContract]
public class HealingArmy
{
	[JsonDataMember(Name = "Remarks")]
	public string Remarks { get; set; }
	}

	

	[JsonDataContract]
public class FriendlyReinforcingArmy
{
	
	}

	[JsonDataContract]
public class TroopGroups
{
	[JsonDataMember(Name = "my_garrison")]
	public MyGarrison my_garrison { get; set; }
	[JsonDataMember(Name = "my_moving_army")]
	public MyMovingArmy my_moving_army { get; set; }
	[JsonDataMember(Name = "my_reinforcing_army")]
	public MyReinforcingArmy my_reinforcing_army { get; set; }
	[JsonDataMember(Name = "my_gathering_army")]
	public MyGatheringArmy my_gathering_army { get; set; }
	[JsonDataMember(Name = "my_moving_caravan")]
	public MyMovingCaravan my_moving_caravan { get; set; }
	[JsonDataMember(Name = "enemy_incoming")]
	public EnemyIncoming enemy_incoming { get; set; }
	[JsonDataMember(Name = "wounded_army")]
	public WoundedArmy wounded_army { get; set; }
	[JsonDataMember(Name = "healing_army")]
	public HealingArmy healing_army { get; set; }
	[JsonDataMember(Name = "friendly_reinforcing_army")]
	public FriendlyReinforcingArmy friendly_reinforcing_army { get; set; }
}

[JsonDataContract]
public class SpyScroll
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}


[JsonDataContract]
public class QueueBuildings
{
	[JsonDataMember(Name = "queued")]
	public List<Queued> queued { get; set; }
}

[JsonDataContract]
public class Queued
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "cmd")]
	public string cmd { get; set; }
	[JsonDataMember(Name = "type")]
	public int type { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
	[JsonDataMember(Name = "due")]
	public long due { get; set; }
}

[JsonDataContract]
public class QueueTroopsTrain
{
	[JsonDataMember(Name = "queued")]
	public List<Queued> queued { get; set; }
}

[JsonDataContract]
public class Queues
{
	[JsonDataMember(Name = "queue_buildings")]
	public QueueBuildings queue_buildings { get; set; }
	[JsonDataMember(Name = "queue_troops_train")]
	public QueueTroopsTrain queue_troops_train { get; set; }
}



	[JsonDataContract]
public class Island
{
	
	}

	[JsonDataContract]
public class Power
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
	[JsonDataMember(Name = "cap")]
	public int cap { get; set; }
}

[JsonDataContract]
public class Luck
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
	[JsonDataMember(Name = "cap")]
	public int cap { get; set; }
}

[JsonDataContract]
public class BouncingChest
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "lvl")]
	public int lvl { get; set; }
}

[JsonDataContract]
public class TokenTeleport
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class SpellTokens
{
	[JsonDataMember(Name = "token_teleport")]
	public TokenTeleport token_teleport { get; set; }
}

[JsonDataContract]
public class TokenProtection
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class Tokens
{
	[JsonDataMember(Name = "token_protection")]
	public TokenProtection token_protection { get; set; }
}

[JsonDataContract]
public class ResSpy
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "lvl")]
	public int lvl { get; set; }
}

[JsonDataContract]
public class ResMagAtt
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "lvl")]
	public int lvl { get; set; }
	[JsonDataMember(Name = "state")]
	public int state { get; set; }
	[JsonDataMember(Name = "due")]
	public long due { get; set; }
}

[JsonDataContract]
public class Researches
{
	[JsonDataMember(Name = "res_spy")]
	public ResSpy res_spy { get; set; }
	[JsonDataMember(Name = "res_mag_att")]
	public ResMagAtt res_mag_att { get; set; }
}

[JsonDataContract]
public class CaptureMonster4
{
	
	}

	[JsonDataContract]
public class Blessings
{
	[JsonDataMember(Name = "CaptureMonster4")]
	public CaptureMonster4 CaptureMonster4 { get; set; }
}

[JsonDataContract]
public class QTreeOfLife2
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class QScout1
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class Quest
{
	[JsonDataMember(Name = "q_tree_of_life_2")]
	public QTreeOfLife2 q_tree_of_life_2 { get; set; }
	[JsonDataMember(Name = "q_scout_1")]
	public QScout1 q_scout_1 { get; set; }
}

[JsonDataContract]
public class TokenSpeedup1Min
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public long typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class TokenSpeedup15Min
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public long typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class Spend
{

		[JsonDataMember(Name = "token_speedup_1_min")]
public TokenSpeedup1Min token_speedup_1_min { get; set; }
[JsonDataMember(Name = "token_speedup_15_min")]
public TokenSpeedup15Min token_speedup_15_min { get; set; }
	}

	[JsonDataContract]
public class CavalryTroops
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public long typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

	[JsonDataContract]
	public class Kill
	{
		[JsonDataMember(Name = "cavalry_troops")]
		public CavalryTroops cavalry_troops { get; set; }
	}

	[JsonDataContract]
public class EmpireResources
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public long typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

		[JsonDataContract]
		public class Raid
		{
			[JsonDataMember(Name = "empire_resources")]
			public EmpireResources empire_resources { get; set; }
		}

	[JsonDataContract]
public class Raided
{


	}

	[JsonDataContract]
public class InfantryTroops
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public long typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class InfT1
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public long typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class Speedup1Min
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public long typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class Speedup15Min
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public long typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class FinishNow
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public long typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class Chapel
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public long typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "lvl")]
	public int lvl { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class MineLife
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public long typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "lvl")]
	public int lvl { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class Make
{
	[JsonDataMember(Name = "infantry_troops")]
	public InfantryTroops infantry_troops { get; set; }
	[JsonDataMember(Name = "inf_t1")]
	public InfT1 inf_t1 { get; set; }
[JsonDataMember(Name = "speedup_1_min")]
public Speedup1Min speedup_1_min { get; set; }
[JsonDataMember(Name = "speedup_15_min")]
public Speedup15Min speedup_15_min { get; set; }
[JsonDataMember(Name = "finish_now")]
public FinishNow finish_now { get; set; }
[JsonDataMember(Name = "chapel")]
public Chapel chapel { get; set; }
[JsonDataMember(Name = "mine_life")]
public MineLife mine_life { get; set; }
	}

	[JsonDataContract]
public class DeffencesPvpLost
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public long typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class Killed
{

	}

	[JsonDataContract]
public class Xp
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public long typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "qty")]
	public int qty { get; set; }
}

[JsonDataContract]
public class Node
{
	[JsonDataMember(Name = "xp")]
	public Xp xp { get; set; }
}

[JsonDataContract]
public class Counter
{
	[JsonDataMember(Name = "quest")]
	public Quest quest { get; set; }
	[JsonDataMember(Name = "spend")]
	public Spend spend { get; set; }
	[JsonDataMember(Name = "kill")]
	public Kill kill { get; set; }
	[JsonDataMember(Name = "raid")]
	public Raid raid { get; set; }
	[JsonDataMember(Name = "deffences_pvp_lost")]
	public DeffencesPvpLost deffences_pvp_lost { get; set; }
	[JsonDataMember(Name = "killed")]
	public Killed killed { get; set; }
	[JsonDataMember(Name = "node")]
	public Node node { get; set; }
}

[JsonDataContract]
public class Member
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "typeId")]
	public int typeId { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "nick")]
	public string nick { get; set; }
	[JsonDataMember(Name = "name")]
	public string name { get; set; }
	[JsonDataMember(Name = "pwr")]
	public int pwr { get; set; }
	[JsonDataMember(Name = "kills")]
	public int kills { get; set; }
	[JsonDataMember(Name = "online")]
	public long online { get; set; }
}

[JsonDataContract]
public class AllianceMembership
{
	[JsonDataMember(Name = "member")]
	public Member member { get; set; }
}



	[JsonDataContract]
public class Bookmark
{

	}

	[JsonDataContract]
public class Empire
{
	[JsonDataMember(Name = "uid")]
	public string uid { get; set; }
	[JsonDataMember(Name = "id")]
	public int id { get; set; }
	[JsonDataMember(Name = "pwr")]
	public int pwr { get; set; }
	[JsonDataMember(Name = "ear")]
	public int ear { get; set; }
	[JsonDataMember(Name = "nick")]
	public string nick { get; set; }
	[JsonDataMember(Name = "created")]
	public long created { get; set; }
	[JsonDataMember(Name = "name")]
	public string name { get; set; }
	[JsonDataMember(Name = "loc")]
	public object loc { get; set; }
	[JsonDataMember(Name = "badge")]
	public object badge { get; set; }
	[JsonDataMember(Name = "cpl")]
	public int cpl { get; set; }
	[JsonDataMember(Name = "edr")]
	public int edr { get; set; }
	[JsonDataMember(Name = "push")]
	public bool push { get; set; }
	[JsonDataMember(Name = "intro")]
	public string intro { get; set; }
	[JsonDataMember(Name = "alliance")]
	public string alliance { get; set; }
	[JsonDataMember(Name = "reward_alliance")]
	public int reward_alliance { get; set; }
	[JsonDataMember(Name = "allianceName")]
	public string allianceName { get; set; }
	[JsonDataMember(Name = "avtr")]
	public string avtr { get; set; }
	[JsonDataMember(Name = "allianceId")]
	public int allianceId { get; set; }
	[JsonDataMember(Name = "Hero")]
	public Hero Hero { get; set; }
	[JsonDataMember(Name = "HeroBag")]
	public HeroBag HeroBag { get; set; }
	[JsonDataMember(Name = "Island")]
	public Dictionary<string, City>  Island { get; set; }
	[JsonDataMember(Name = "Power")]
	public Power Power { get; set; }
	[JsonDataMember(Name = "Luck")]
	public Luck Luck { get; set; }
	[JsonDataMember(Name = "bouncing_chest")]
	public BouncingChest bouncing_chest { get; set; }
	[JsonDataMember(Name = "spell_tokens")]
	public SpellTokens spell_tokens { get; set; }
	[JsonDataMember(Name = "tokens")]
	public Tokens tokens { get; set; }
	[JsonDataMember(Name = "researches")]
	public Researches researches { get; set; }
	[JsonDataMember(Name = "blessings")]
	public Blessings blessings { get; set; }

[JsonDataMember(Name = "counter")]
public Counter counter { get; set; }
[JsonDataMember(Name = "alliance_membership")]
public AllianceMembership alliance_membership { get; set; }
[JsonDataMember(Name = "bookmark")]
public Bookmark bookmark { get; set; }
	}

	[JsonDataContract]
	public class Building
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "id")]
		public int id { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "type")]
		public int type { get; set; }
		[JsonDataMember(Name = "name")]
		public string name { get; set; }
		[JsonDataMember(Name = "due")]
		public long due { get; set; }

		[JsonDataMember(Name = "state")]
		public int state { get; set; }
	}

	[JsonDataContract]
	public class City
	{
		[JsonDataMember(Name = "uid")]
		public string uid { get; set; }
		[JsonDataMember(Name = "id")]
		public int id { get; set; }
		[JsonDataMember(Name = "typeId")]
		public int typeId { get; set; }
		[JsonDataMember(Name = "time")]
		public long time { get; set; }
		[JsonDataMember(Name = "zon")]
		public int zon { get; set; }
		[JsonDataMember(Name = "name")]
		public string name { get; set; }
		[JsonDataMember(Name = "loc")]
		public int loc { get; set; }
		[JsonDataMember(Name = "resource")]
		public Resource resource { get; set; }
		[JsonDataMember(Name = "slots")]
		public Dictionary<string, Building> slots { get; set; }
		[JsonDataMember(Name = "missions_renewer")]
		public MissionsRenewer missions_renewer { get; set; }
		[JsonDataMember(Name = "eagle_mission")]
		public EagleMission eagle_mission { get; set; }
		[JsonDataMember(Name = "troop_groups")]
		public TroopGroups troop_groups { get; set; }
		[JsonDataMember(Name = "spy_scroll")]
		public SpyScroll spy_scroll { get; set; }
		[JsonDataMember(Name = "queues")]
		public Queues queues { get; set; }
	}

}
