using Terraria;
using System;
using System.Collections.Generic;
using Hooks;
using TShockAPI;

namespace PluginTest
{
    [APIVersion(1, 11)]
    public class AutoBossSpawner : TerrariaPlugin
    {
        private DateTime LastCheck = DateTime.UtcNow;
        private DateTime OtherLastCheck = DateTime.UtcNow;
        private int BossTimer = 30;
        private List<Terraria.NPC> bossList = new List<Terraria.NPC>();
        private int[] henchmenArray = new int[] { 2, 6, 16, 24, 28, 29, 31, 32, 34, 42, 44, 45, 48, 59, 60, 62, 71, 75, 77, 78, 81, 82, 83, 84, 85, 86, 93, 104, 110, 111, 120, 121, 122, 137, 138, 140, 141, 143, 144, 145 };
        private bool BossToggle = true;

        public override string Name
        {
            get { return "Auto Boss Spawner"; }
        }
        public override string Author
        {
            get { return "by InanZen"; }
        }
        public override string Description 
        {
            get { return "Auto spawn bosses every night"; }
        }
        public override Version Version
        {
            get { return new Version("1.0.1"); }
        }
        public override void Initialize()
        {
            GameHooks.Update += OnUpdate;
            GameHooks.Initialize += OnInitialize;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GameHooks.Update -= OnUpdate;
                GameHooks.Initialize -= OnInitialize;
            }
            base.Dispose(disposing);
        }
        public AutoBossSpawner(Main game) : base(game)
        {

        }

        public void OnUpdate()
        {
            if (BossToggle && ((DateTime.UtcNow - LastCheck).TotalSeconds >= 1))
            {
                LastCheck = DateTime.UtcNow;
                double thetime = Main.time;
                string daynight = (Main.dayTime)?"day":"night";
                if (!Main.dayTime && BossTimer<60)
                {
                    if (BossTimer == 30) 
                    {
                        TShockAPI.TShock.Utils.Broadcast("ATTENTION: Boss battle starting in 30 seconds (/warp arena)", Color.Aquamarine);
                    }
                    else if (BossTimer == 10) 
                    {
                        TShockAPI.TShock.Utils.Broadcast("ATTENTION: Boss battle starting in 10 seconds! (/warp arena)", Color.Aquamarine);
                    }
                    else if (BossTimer == 0)
                    {
                        TShockAPI.TShock.Utils.Broadcast("Boss battle in Arena has begun! (/warp arena)", Color.Aquamarine);
                        startBossBattle();                        
                    }
                    else if ((BossTimer < 0) && (BossTimer % 20 == 0))
                    {
                        bool bossActive = false;
                        for (int i = 0; i < bossList.Count; i++)
                        {
                            if (bossList[i].active) bossActive = true;
                        }
                        if (bossActive) spawnHenchmen();
                        else
                        {
                            TShockAPI.TShock.Utils.Broadcast("All Bosses have been defeated. That's it for tonight.",Color.Aquamarine);
                            BossTimer = 100;
                            for (int i = 0; i < Main.npc.Length; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].type == 70)
                                {                    
                                    TSPlayer.Server.StrikeNPC(i, 9999, 90f, 1);
                                }
                            }
                        }
                    }
                    BossTimer--;                   
                }
                else if (BossTimer != 30 && Main.dayTime)
                {
                    BossTimer = 30;
                }
            }
        }

        private void startBossBattle()
        {
            NPC npc = new NPC();
            TShockAPI.DB.Region arenaregion = TShock.Regions.GetRegionByName("arena");
            int arenaX = arenaregion.Area.X + (arenaregion.Area.Width / 2);
            int arenaY = arenaregion.Area.Y + (arenaregion.Area.Height / 2);
            string broadcastString = "";
            Random r = new Random();
            switch (r.Next(1, 9))
            {
                case (1):
                    npc = TShockAPI.TShock.Utils.GetNPCById(134);     //destroyer                   
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 1, arenaX, arenaY, 20, 20);
                    npc = TShockAPI.TShock.Utils.GetNPCById(4);  //eye
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 3, arenaX, arenaY, 50, 50);
                    broadcastString = "Boss selected: Destroyer + 3x Eye of Chutuhlu!";
                    break;                    
                case (2):
                    npc = TShockAPI.TShock.Utils.GetNPCById(134);     //destroyer                   
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 2, arenaX, arenaY, 30, 30);
                    broadcastString = "Boss selected: 2x Destroyer!";
                    break;
                case (3):
                    npc = TShockAPI.TShock.Utils.GetNPCById(125); //twins
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 1, arenaX, arenaY, 20, 20);
                    npc = TShockAPI.TShock.Utils.GetNPCById(126); //twins
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 1, arenaX, arenaY, 20, 20);
                    npc = TShockAPI.TShock.Utils.GetNPCById(50);     //king             
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 5, arenaX, arenaY, 30, 30);
                    broadcastString = "Boss selected: The Twins + 5x King Slime!";
                    break;
                case (4):
                    npc = TShockAPI.TShock.Utils.GetNPCById(127); //prime
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 1, arenaX, arenaY, 20, 20);
                    npc = TShockAPI.TShock.Utils.GetNPCById(70);     //spikes                   
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 5, arenaX, arenaY, 50, 50);
                    broadcastString = "Boss selected: Skeletron Prime + 5x Spike Ball!";
                    break;
                case (5):                    
                    npc = TShockAPI.TShock.Utils.GetNPCById(127); //prime
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 1, arenaX, arenaY, 20, 20);
                    npc = TShockAPI.TShock.Utils.GetNPCById(134);     //destroyer                   
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 1, arenaX, arenaY, 20, 20);
                    broadcastString = "Boss selected: Skeletron Prime + Destroyer!";
                    break;
                case (6):
                    npc = TShockAPI.TShock.Utils.GetNPCById(134); //destroyer
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 1, arenaX, arenaY, 20, 20);
                    npc = TShockAPI.TShock.Utils.GetNPCById(70);     //spikes              
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 7, arenaX, arenaY, 50, 50);
                    broadcastString = "Boss selected: Destroyer + 7x Spike Ball!";
                    break;
                case (7):
                    npc = TShockAPI.TShock.Utils.GetNPCById(125); //twins
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 1, arenaX, arenaY, 20, 20);
                    npc = TShockAPI.TShock.Utils.GetNPCById(126); //twins
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 1, arenaX, arenaY, 20, 20);
                    npc = TShockAPI.TShock.Utils.GetNPCById(70);     //spikes              
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 5, arenaX, arenaY, 50, 50);
                    broadcastString = "Boss selected: The Twins + 5x Spike Ball!";
                    break;
                case (8):
                    npc = TShockAPI.TShock.Utils.GetNPCById(87); //wyvern
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, 5, arenaX, arenaY, 50, 50);
                    broadcastString = "Boss selected: 5x Wyvern!";
                    break;
                default:
                    broadcastString = "Stars say: No boss tonight."; 
                    break;
            }
            TShockAPI.TShock.Utils.Broadcast(broadcastString, Color.Aquamarine);

            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].boss) bossList.Add(Main.npc[i]);
            }
        }
        private void spawnHenchmen()
        {
            //TODO: num of henchmen based on players in arena, spawn life when boss is at half health.
            NPC npc = new NPC();         
            Random r = new Random();
            npc = TShockAPI.TShock.Utils.GetNPCById(henchmenArray[r.Next(0,henchmenArray.Length)]);
            TShockAPI.DB.Region arenaregion = TShock.Regions.GetRegionByName("arena");
            int arenaX = arenaregion.Area.X + (arenaregion.Area.Width / 2);
            int arenaY = arenaregion.Area.Y + (arenaregion.Area.Height / 2);
            int henchmenNumber = r.Next(10,31);
            TSPlayer.Server.SpawnNPC(npc.type, npc.name, henchmenNumber , arenaX, arenaY, 50, 50);
            TShockAPI.TShock.Utils.Broadcast("Spawning Boss Henchmen: " + henchmenNumber + "x " + npc.name + "!", Color.SteelBlue);
        }
        
        #region Commands
        public void OnInitialize()
        {
            Commands.ChatCommands.Add(new Command("autoboss", toggleautoboss, "togglearena"));
        }

        public void toggleautoboss(CommandArgs args)
        {
            BossToggle = !BossToggle;
            args.Player.SendMessage("Boss battles now: "+ ((BossToggle)?"Enabled":"Disabled"));
            BossTimer = 30;
        }
        
        #endregion
    }
}