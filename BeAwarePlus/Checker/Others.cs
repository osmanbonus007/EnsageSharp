using System;
using System.Linq;

using Ensage;
using Ensage.Common;
using Ensage.SDK.Helpers;

using BeAwarePlus.Menus;

namespace BeAwarePlus.Checker
{
    internal class Others
    {
        private MenuManager MenuManager { get; }

        private Unit MyHero { get; }

        private MessageCreator MessageCreator { get; }

        private SoundPlayer SoundPlayer { get; }

        private bool Roshan_Dead { get; set; } = false;

        private int Roshan_Respawn_Time { get; set; }

        public Others(
            MenuManager menumanager,
            Unit myhero,
            MessageCreator messagecreator,
            SoundPlayer soundplayer)
        {
            MenuManager = menumanager;
            MyHero = myhero;
            MessageCreator = messagecreator;
            SoundPlayer = soundplayer;

            Game.OnFireEvent += OnGameEvent;
            UpdateManager.Subscribe(OnTimedEvent, 1000);
        }

        public void Dispose()
        {
            Game.OnFireEvent -= OnGameEvent;
            UpdateManager.Unsubscribe(OnTimedEvent);
        }

        private void OnGameEvent(FireEventEventArgs args)
        {
            if (args.GameEvent.Name.Contains("dota_roshan_kill"))
            {
                Roshan_Dead = true;
            }
        }

        private void OnTimedEvent()
        {
            //Check Rune
            if (((Math.Round(Game.GameTime) + 10) % 120) == 0
                && MenuManager.OtherItem.Value.IsEnabled("rune_bounty") 
                && Utils.SleepCheck("check_rune"))
            {
                MessageCreator.MessageCheckRuneCreator(null);
                SoundPlayer.Play("check_rune");

                Utils.Sleep(2000, "check_rune");
            }

            //Hand of Midas
            var Midas = MyHero.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_hand_of_midas"));

            if (Midas != null 
                && Math.Round(Midas.Cooldown) == 5
                && MenuManager.OtherItem.Value.IsEnabled("item_hand_of_midas") 
                && Utils.SleepCheck("use_midas"))
            {
                MessageCreator.MessageUseMidasCreator(null);
                SoundPlayer.Play("use_midas");

                Utils.Sleep(2000, "use_midas");
            }

            if (Roshan_Dead
                && MenuManager.OtherItem.Value.IsEnabled("roshan_halloween_levels"))
            {
                var roshan = EntityManager<Unit>.Entities.Any(
                    x =>
                    x.Name == "npc_dota_roshan" &&
                    x.IsAlive);

                Roshan_Respawn_Time += 1;

                //Roshan MB Alive
                if (Roshan_Respawn_Time == 485)
                {
                    MessageCreator.MessageRoshanMBAliveCreator(null);
                    SoundPlayer.Play("roshan_mb_alive");
                }

                //Roshan Alive            
                if (roshan || Roshan_Respawn_Time == 665)
                {
                    MessageCreator.MessageRoshanAliveCreator(null);
                    SoundPlayer.Play("roshan_alive");

                    Roshan_Respawn_Time = 0;
                    Roshan_Dead = false;
                }
            }
        } 
    } 
}
