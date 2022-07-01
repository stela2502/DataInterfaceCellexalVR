// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System.IO;
using System.Net;
using System.Text;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            WebRequest req = WebRequest.Create("http://127.0.0.1:8000/GetExprs");
            string postData = "name=ProcR";
            byte[] send = Encoding.Default.GetBytes(postData);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = send.Length;

            Stream sout = req.GetRequestStream();
            sout.Write(send, 0, send.Length);
            sout.Flush();
            sout.Close();
            
            WebResponse res = req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());

            Console.WriteLine( sr.ReadToEnd() );

        }
    }
}
