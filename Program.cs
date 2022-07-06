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


namespace DataInterfaceCellexalVR
{
    class Program
    {
        public static string server = "http://127.0.0.1:8001/";

        static void Main(string[] args)
        {
            string gene = args[0] ?? "ProcR";
            string[][] result = GetExpression(gene);
            Console.WriteLine(result.Count() + ": " + result[0][0] + " " + result[0][1]);

            string[] drcNames = GetDrcNames();
            Console.WriteLine("DRC names: " + string.Join(" ", drcNames));

            for (int i = 0; i < drcNames.Length; i++)
            {
                string[][] drc = GetDrcCoords(drcNames[i]);
                Console.WriteLine("DRC " + drcNames[i] + "(n=" + drc.Length + "): " + string.Join(" ", drc[0]));
            }

            StreamReader streamreader = new StreamReader(@"./selection1.txt");
            char[] delimiter = new char[] { '\t' };
            int lines = 0;
            while (streamreader.Peek() > 0)
            {
                streamreader.ReadLine();
                lines++;
            }
            streamreader.Close();
            streamreader = new StreamReader(@"selection1.txt");
            string[][] datatable = new string[lines][];
            lines = 0;
            while (streamreader.Peek() > 0)
            {
                datatable[lines++] = streamreader.ReadLine().Split(delimiter);
            }
            streamreader.Close();

            Console.WriteLine("use this grouping: " + string.Join(" ", datatable[0]));

            JArray res = Correlate2( gene, false, true);

            Console.WriteLine("genes correlated to "+gene+": " + res);
            
            string[] gois = getDifferentials( datatable );

            Console.WriteLine( "differential genes :"+String.Join(" ", gois ) );

            JObject tfNetworks = getTFnetwork(datatable, 5);

            Console.WriteLine("Networks JSON :" + tfNetworks);

            string response = LogScreeshotHttp( "test.png" );

            Console.WriteLine("log screenshot response :" + response);
            // And now stop the server - just a test not helpful for debugging ;-)
            Stop();
            
        }

        protected static string GetBase64StringForImage(string imgPath)  
        {  
            byte[] imageBytes = System.IO.File.ReadAllBytes(imgPath);  
            string base64String = Convert.ToBase64String(imageBytes);
            Console.WriteLine("GetBase64StringForImage base64 encoded: " + base64String);
            return base64String;
        } 

        public static string LogScreeshotHttp(string screenshotImageFilePath){

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(server + "LogScreenshot");
            string postData = "png="+ GetBase64StringForImage(screenshotImageFilePath) ;

            //Console.WriteLine( postData );
            byte[] send = Encoding.Default.GetBytes(postData);

            req.Method = "POST";
            req.ContentType = "application/octet-stream";
            req.ContentLength = send.Length;

            Stream sout = req.GetRequestStream();
            sout.Write(send, 0, send.Length);
            sout.Flush();
            sout.Close();

            WebResponse res = req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());

            return sr.ReadToEnd();
        }

        public static void Stop()
        {
            // This will 
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(server + "shutdown");
                WebResponse res = req.GetResponse();
            }
            catch
            {
                Console.WriteLine("as expected - server is down.");
            }
        }


        public static JArray Correlate2( string gene, bool marker, bool cpp )
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(server + "corr2");
            string postData = "gname=" + gene + "&" +
            "is.smarker="+marker.ToString().ToUpper()+"&cpp="+cpp.ToString().ToUpper() ;

            //Console.WriteLine( postData );
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

            JArray jObject = JArray.Parse(sr.ReadToEnd());
            return jObject;
            //string[][] genes = JsonConvert.DeserializeObject<string[][]>(sr.ReadToEnd());
            //return genes;
        }
        public static string[] getDifferentials(string[][] selection)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(server + "HeatmapList");

            string postData = "selection=" + JsonConvert.SerializeObject(selection) + "&" +
            "num.sig=250&logfc.threshold=0.65&minPct=0.1";
            //string postData = JsonConvert.SerializeObject( selection );
            byte[] send = Encoding.Default.GetBytes(postData);

            req.Method = "POST";
            req.ContentType = "x-www-form-urlapplication/encoded";
            req.ContentLength = send.Length;

            Stream sout = req.GetRequestStream();
            sout.Write(send, 0, send.Length);
            sout.Flush();
            sout.Close();

            WebResponse res = req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());

            string[] genes = JsonConvert.DeserializeObject<string[]>(sr.ReadToEnd());
            return genes;
        }

        public static JObject getTFnetwork(string[][] selection, int nGene )
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(server + "TFnetwork");

            string postData = "cellidfile="
                              + JsonConvert.SerializeObject(selection)
                              + "&top.n.inter="
                              + nGene.ToString()
                              + "&cutoff.ggm=0.1&exprFract=0.1&method=rho.p";

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

            string[] array = JsonConvert.DeserializeObject<string[]>(sr.ReadToEnd());

            JObject jObject = JObject.Parse(array[0]);
            //string[] result = jObject[0].ToObject<string[]>();
            //List<DataInterfaceCellexalVR.Networks> result = JsonConvert.DeserializeObject<List<DataInterfaceCellexalVR.Networks>>(array[0]);

            //string[] genes = JsonConvert.DeserializeObject<string[]>( sr.ReadToEnd() );
            //return genes;
            return jObject;
        }

        public static string[][] GetDrcCoords(string name)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(server + "get_coords");

            string postData = "name=" + name;
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

            string[] array = JsonConvert.DeserializeObject<string[]>(sr.ReadToEnd());
            string[][] result = JsonConvert.DeserializeObject<string[][]>(array[0]);

            return result;
        }

        public static string[] GetDrcNames()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(server + "GetDrcNames");

            WebResponse res = req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());
            string[] array = JsonConvert.DeserializeObject<string[]>(sr.ReadToEnd());

            return array;
        }

        public static string[][] GetExpression(string gene)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(server + "GetExprs");

            string postData = "name=" + gene;
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
            string[] array = JsonConvert.DeserializeObject<string[]>(sr.ReadToEnd());
            string[][] result = JsonConvert.DeserializeObject<string[][]>(array[0]);
            return result;
        }
    }
}
