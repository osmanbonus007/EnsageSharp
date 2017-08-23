using Ensage.Common.Menu;

namespace BeAwarePlus.Menus
{
    internal class MenuDebug
    {
        private MenuManager MenuManager { get; }

        public MenuDebug(MenuManager menumanager)
        {
            MenuManager = menumanager;
        }

        public void Debug(string HeroTexturName, string HeroName)
        {
            var heroMenu = new Menu(HeroName, HeroTexturName, false, "npc_dota_hero_" + HeroTexturName, true);

            heroMenu.AddItem(new MenuItem(HeroTexturName, "Enable").SetValue(true));

            MenuManager.BugHeroSettingsMenu.AddSubMenu(heroMenu);
        }

    }
}
