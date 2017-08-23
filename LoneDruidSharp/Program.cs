namespace LoneDruidSharpRewrite
{
    class Program
    {
        #region Static Fields

        private static Bootstrap bootstrap;

        #endregion

        static void Main(string[] args)
        {
            bootstrap = new Bootstrap();
            bootstrap.SubscribeEvents();
        }
    }
}
