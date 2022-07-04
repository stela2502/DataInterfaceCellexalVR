// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Globalization;
using Newtonsoft.Json.Linq;




/*
structure I need to parse here:
["{\"ids\":[\"HSPC_001\",\"HSPC_002\"],
\"values\":[\"10.1291385985772\",\"8.50969794159174\"]}"]
*/


namespace HelloWorld
{
    class Program
    {
        public static string server = "http://127.0.0.1:8001/";

        static void Main(string[] args)
        {
            string gene = args[0] ?? "ProcR";
            string[][] result = GetExpression( gene );
            Console.WriteLine( result.Count() + ": "+ result[0][0]+" "+ result[0][1]);

            string[] drcNames = GetDrcNames();
            Console.WriteLine( "DRC names: " + String.Join(" ", drcNames) );

            for (int i = 0; i < drcNames.Length; i++){
                string[][] drc = GetDrcCoords( drcNames[i]);
                Console.WriteLine( "DRC "+drcNames[i]+"(n="+ drc.Length +"): " + String.Join(" ", drc[0]) );
            }

            StreamReader streamreader = new StreamReader(@"./selection1.txt");
            char[] delimiter = new char[] { '\t' };
            int lines = 0;
            while (streamreader.Peek() > 0)
            {
                streamreader.ReadLine();
                lines ++;
            }
            streamreader.Close();
            streamreader = new StreamReader(@"selection1.txt");
            string[][] datatable = new string[lines][];
            lines=0;
            while (streamreader.Peek() > 0)
            {
                datatable[lines++] = streamreader.ReadLine().Split(delimiter);
            }
            streamreader.Close();

            Console.WriteLine( "use this grouping: "+String.Join(" ", datatable[0] ) );

            string[] gois = getDifferentials( datatable );

            Console.WriteLine( "differential genes :"+String.Join(" ", gois ) );

            // And now stop the server - just a test not helpful for debugging ;-)
            Stop();
        }

        public static void Stop() {
            // This will 
            try { 
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Program.server+"shutdown");
                WebResponse res = req.GetResponse();
            }
            catch  {
                Console.WriteLine( "as expected - server is down." );
            }
        }

        public static string[] getDifferentials(  string[][] selection ){
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Program.server+"HeatmapList");

            string postData = "selection="+JsonConvert.SerializeObject( selection ) + "&" +
            "num.sig=250&logfc.threshold=0.65&minPct=0.1";
            //string postData = JsonConvert.SerializeObject( selection );
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

            string[] genes = JsonConvert.DeserializeObject<string[]>( sr.ReadToEnd() );
            return genes;
        } 

        public static string[][] GetDrcCoords( string name){
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(server+"get_coords");
            
            string postData = "name="+name;
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

            string[] array = JsonConvert.DeserializeObject<string[]>( sr.ReadToEnd() );
            string[][] result = JsonConvert.DeserializeObject<string[][]>( array[0] );
            
            return result;
        } 

        public static string[] GetDrcNames(){
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(server+"GetDrcNames");
            
            WebResponse res = req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());
            string[] array = JsonConvert.DeserializeObject<string[]>( sr.ReadToEnd() );
            
            return array;
        } 

        public static string[][] GetExpression( string gene ) 
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(server+"GetExprs");
            
            string postData = "name="+gene;
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



            //JObject JArray = JObject.Parse(sr.ReadToEnd());

            /*
            List<string> x = JArray[0][$"values"]
                .Children()
                .Select(v => v.Value<float>())
                .ToList();
            
            cellValues = x.ToArray();
            
            */
            //SampleJsonSchemaGenerator generator = new SampleJsonSchemaGenerator();
            //var schema = generator.Generate( sr.ReadToEnd() );
            //JSchema schema = JSchema.Parse(sr.ReadToEnd());
            //JObject user = JObject.Parse(sr.ReadToEnd() );

            //Console.WriteLine( schema.ToJson() );

            /*
            ## this is part of cellexalVR's scarf handler:
            string response = System.Text.Encoding.UTF8.GetString(req.downloadHandler.data);
            JObject jObject = JObject.Parse(response);
            List<string> x = jObject[$"values"]
                .Children()
                .Select(v => v.Value<string>())
                .ToList();

            geneNames = x.ToArray();
            */
            
            //string response = sr.ReadToEnd();
            //JArray jObject = JArray.Parse(response);

            //string[] result = jObject[0].ToObject<string[]>();
            string[] array = JsonConvert.DeserializeObject<string[]>( sr.ReadToEnd() );
            string[][] result = JsonConvert.DeserializeObject<string[][]>( array[0] );
            return result;
        }
    }
}
