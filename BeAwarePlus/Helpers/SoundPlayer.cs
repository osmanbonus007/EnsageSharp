using System;

using BeAwarePlus.Menus;

namespace BeAwarePlus.Checker
{
    internal class SoundPlayer
    {
        private MenuManager MenuManager { get; }

        public SoundPlayer(MenuManager menumanager)
        {
            MenuManager = menumanager;
        }

        public void Play(string Path)
        {
            if (MenuManager.SoundItem.Value)
            {
                var Player = new System.Media.SoundPlayer();
                var FullPath = Environment.CurrentDirectory;

                FullPath = FullPath.Remove(FullPath.Length - 10);

                if (MenuManager.DefaultSoundItem)
                {
                    FullPath += @"\dota\materials\ensage_ui\sounds\default.wav";
                }
                else
                {
                    FullPath += @"\dota\materials\ensage_ui\sounds\" + 
                        Path + 
                        "_" + 
                        MenuManager.LangList[MenuManager.LanguageItem.Value.SelectedIndex] + 
                        ".wav";
                }

                Player.SoundLocation = FullPath;
                Player.Load();
                Player.Play();
            }   
        }
    }
}
