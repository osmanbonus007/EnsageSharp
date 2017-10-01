using System.ComponentModel.Composition;
using System.Reflection;

using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;

using log4net;
using PlaySharp.Toolkit.Logging;

namespace UnitsControlPlus
{
    [ExportPlugin(
        name: "UnitsControlPlus",
        mode: StartupMode.Auto,
        author: "YEEEEEEE", 
        version: "1.0.3.0")]
    internal class UnitsControlPlus : Plugin
    {
        private Config Config { get; set; }

        public IServiceContext Context { get; }

        public ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [ImportingConstructor]
        public UnitsControlPlus([Import] IServiceContext context)
        {
            Context = context;
        }

        protected override void OnActivate()
        {
            Config = new Config(this);
        }

        protected override void OnDeactivate()
        {
            Config?.Dispose();
        }
    }
}
