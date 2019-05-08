using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SentimentCounter
{
    class Program
    {

        static void Main(string[] args)
        {

            RunResponseTotalsReport();

        }

        static void RunResponseTotalsReport()
        {
            //Set Data
            string data = "[{'sentiment':'Good', 'responseDate':'2019-03-01', 'employeeId': '1', 'employeeName': '', 'active':'true'}, {'sentiment':'Bad', 'responseDate':'2019-03-02', 'employeeId': '1', 'employeeName': '', 'active':'true'},{'sentiment':'Ok', 'responseDate':'2019-04-30', 'employeeId': '1', 'employeeName': '', 'active':'true'}]";

            //Date range to filter on
            DateTimeOffset dtFrom;
            DateTimeOffset dtTo; 
            
            string input = string.Empty;

            //Loop until user wants to quit
            while (input != "q")
            {
                //Initialize range
                dtFrom = DateTimeOffset.MinValue;
                dtTo = DateTimeOffset.MinValue;

                Console.WriteLine("Please enter beginning date and ending date; enter 'Q' to quit.");


                //Loop until user gives a good date for dtFrom or wants to quit
                while (dtFrom == DateTimeOffset.MinValue & input != "q")
                {
                    Console.Write("Please enter beginning date:  ");
                    input = Console.ReadLine().ToLower().Trim();

                    //Break out if user wants to quit
                    if (input.ToLower().Trim() == "q") break;

                    //If input can't be parsed into a date, ask for it again
                    if (!DateTimeOffset.TryParse(input, out dtFrom))
                    {
                        Console.WriteLine("Invalid Date");
                    }
                }

                //TODO:  Get rid of duplicate code
                //Loop until user gives a good date for dtTo or wants to quit
                while (dtTo == DateTimeOffset.MinValue && input != "q")
                {
                    Console.Write("Please enter ending date:  ");
                    input = Console.ReadLine().ToLower().Trim();

                    //Break out if user wants to quit
                    if (input == "q") break;

                    //If input can't be parsed into a date, ask for it again
                    if (!DateTimeOffset.TryParse(input, out dtTo))
                    {
                        Console.WriteLine("Invalid Date");
                    }

                }

                //Break out if user wants to quit
                if (input.ToLower().Trim() == "q") break;

                //Process data to get counts
                SentimentCounts Counts = GetSentimentCounts(data, dtFrom, dtTo);


                //Display results
                Console.WriteLine(String.Format("\n\nSurvey Response Totals for {0} through {1}", dtFrom.ToString("M/d/yyyy"), dtTo.ToString("M/d/yyyy")));
                Console.WriteLine(String.Format("\tGood: \t{0}", Counts.goodCount));
                Console.WriteLine(String.Format("\tOk:  \t{0}", Counts.okCount));
                Console.WriteLine(String.Format("\tBad: \t{0}", Counts.badCount));
                Console.WriteLine("\n\n");
            }


        }


        public static SentimentCounts GetSentimentCounts(string data, DateTimeOffset dtStart, DateTimeOffset dtEnd)
        {


            //Deserialize data into Responses, filtering out inactive employees and responses outside date range,
            //  and sort in descending date order (most recent responses at top)
            List<Response> FilteredResponses = (JsonConvert.DeserializeObject<List<Response>>(data))
                .Where(r => r.responseDate >= dtStart
                    && r.responseDate <= dtEnd
                    && r.active == true)
                .OrderByDescending(r => r.responseDate)
                .ToList();  //TODO:  Hey look, I *can*  use just one List :-)

            
            HashSet<long> uniqueEmployeeIds = new HashSet<long>();

            long goodCounter = 0, badCounter = 0, okCounter = 0, otherCounter = 0;   //TODO:  forgot to initialize 


            //Loop through responses, attempt to add response's employeeID to hashset as a filter for uniqueness
            // If it is added, then increment sentiment count
            foreach (Response response in FilteredResponses)
            {
                if (uniqueEmployeeIds.Add(response.employeeId))
                {
                    switch  (response.sentiment.ToLower().Trim())
                    {
                        case "good":
                            goodCounter++;
                            break;

                        case "bad":
                            badCounter++;
                            break;

                        case "ok":
                            okCounter++;
                            break;

                        default:
                            otherCounter++;
                            break;
                    }
                }
            }

            return new SentimentCounts { goodCount = goodCounter, okCount = okCounter, badCount = badCounter, otherCount = otherCounter };

        }
    }

    public class Response
    {
        public string sentiment;  //TODO:  think about changing to enum
        public DateTimeOffset responseDate;
        public long employeeId;
        public bool active;

    }

    public struct SentimentCounts
    {
        public long goodCount;
        public long okCount;
        public long badCount;
        public long otherCount;
    }
}
