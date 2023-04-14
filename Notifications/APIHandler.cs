using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Notifications
{
    class APIHandler
    {
        public static HttpClient HttpClient = new HttpClient();
        public static JArray GetGamesFromDate(string date)
        {
            string apiEndpoint = String.Format(ConfigurationManager.AppSettings["GamesAPIEndpoint"], date);
            Task<string> HttpRequest = APIHandler.HttpClient.GetStringAsync(apiEndpoint);

            if (RequestFailed(HttpRequest)) return new JArray();

            JObject response = JObject.Parse(HttpRequest.Result);

            if (Convert.ToInt32(response["totalGames"]) == 0) return new JArray();

            return (JArray)response["dates"][0]["games"];
        }

        public static JObject GetTeamStats(int teamID)
        {
            string apiEndpoint = String.Format(ConfigurationManager.AppSettings["TeamStatsAPIEndpoint"], teamID);
            Task<string> HttpRequest = APIHandler.HttpClient.GetStringAsync(apiEndpoint);

            if (RequestFailed(HttpRequest)) return new JObject();

            return (JObject)JObject.Parse(HttpRequest.Result)["stats"][0]["splits"][0]["stat"];
        }

        private static bool RequestFailed(Task<string> request)
        {
            while (request.Status != TaskStatus.RanToCompletion)
            {
                //need to log error
                if (request.Status == TaskStatus.Faulted) return true;
            }

            return false;
        }
    }
}
