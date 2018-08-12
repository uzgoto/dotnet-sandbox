namespace Uzgoto.DotNetSnipet.Data
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var context = DbContext.Create();
            context.OpenIfClosed();

        }
    }
}
