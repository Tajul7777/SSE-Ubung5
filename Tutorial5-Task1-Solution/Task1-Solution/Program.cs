using System;
using System.Threading.Tasks;

namespace SSE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var url = new Url("http://www.tu-chemnitz.de");
                var resp = await HttpRequest.Get(url);

                Console.WriteLine(resp);
            }).Wait();
        }
    }
}
