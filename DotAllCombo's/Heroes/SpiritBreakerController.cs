using DotaAllCombo.Extensions;

namespace DotaAllCombo.Heroes
{
    using Service.Debug;

    internal class SpiritBreakerController : Variables, IHeroController
    {
        public void Combo()
        {
           
        } // Combo

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("TODO", "0");
            
            Print.LogMessage.Error("This hero not Supported!");
        }
       
        public void OnCloseEvent()
        {
        }
    }
}