using MigrationEngine.Core;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Text.RegularExpressions;

namespace MigrationEngine.Strategies
{
    public class PIVisionStrategy : ISymbolConfigurator
    {
        public JObject Configure(JObject template, SymbolInputData input, DataTable projectDataTable)
        {
            // 1. Clone template to avoid modifying the cache
            JObject symbol = (JObject)template.DeepClone();
            try
            {
                // 2. Set Basic Geometry

                // Symbol  / TAGNAME
                symbol["Symbols"][0]["Name"] = input.InstanceName + "1";
                symbol["Symbols"][0]["Configuration"]["Top"] = input.Top;
                symbol["Symbols"][0]["Configuration"]["Left"] = input.Left;


                /*                string name = input.Properties.ContainsKey("TAGNAME") ? input.Properties["TAGNAME"] : "UNKNOWN";
                                string areaname = input.Properties.ContainsKey("MEAS") ? input.Properties["MEAS"] : "UNKNOWN";
                                string areaname2 = areaname.Substring(12, 12);
                                symbol["Symbols"][0]["Configuration"]["StaticText"] = areaname2.Replace(".","") ;*/



                symbol["Symbols"][0]["Configuration"]["StaticText"] = input.Properties.ContainsKey("TAGNAME") ? input.Properties["TAGNAME"] : "UNKNOWN";


                // Symbol 1 /Data
                symbol["Symbols"][1]["Name"] = input.InstanceName + "2";
                symbol["Symbols"][1]["Configuration"]["Top"] = input.Top + 17;
                symbol["Symbols"][1]["Configuration"]["Left"] = input.Left - 11.7;
                




                // 3. Extract Tag Logic 
                string data = input.Properties.ContainsKey("MEAS") ? input.Properties["MEAS"].Replace("IADAS.", "") : "UNKNOWN";
                


                var queryopen = from t in projectDataTable.AsEnumerable()

                                where t["DCS TAG"].ToString().Trim() == data.Trim()
                                select t;
                string piTag = "UNKNOWN";

                if (queryopen.Any())

                {

                    piTag = queryopen.First()["PI TAG"].ToString();

                }

                // 4. Bind Data Source



                symbol["Symbols"][1]["DataSources"] = new JArray($"pi:\\\\PISRV01\\{piTag}");
                symbol["Symbols"][1]["MSSymbolsIds"] = new JArray();
                symbol["Symbols"][1]["MSLimitDataSources"] = new JArray();



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return symbol;
        }
    }
}