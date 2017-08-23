namespace VisageSharpRewrite
{
    class Program
    {
        private static Bootstrap bootstrap;

        static void Main(string[] args)
        {
            bootstrap = new Bootstrap();
            bootstrap.SubscribeEvents();
        }
    }
}
