using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Notifications
{
    class Team
    {
        public string Name { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int OTLosses { get; set; }
        public int Score { get; set; }

        public Team(JObject team)
        {
            JObject stats = APIHandler.GetTeamStats(Convert.ToInt32(team["team"]["id"]));

            Name = Convert.ToString(team["team"]["name"]);
            Wins = stats["wins"] != null ? Convert.ToInt32(stats["wins"]) : 0;
            Losses = stats["losses"] != null ? Convert.ToInt32(stats["losses"]) : 0;
            OTLosses = stats["ot"] != null ? Convert.ToInt32(stats["ot"]) : 0;
            Score = team["score"] != null ? Convert.ToInt32(team["score"]) : 0;
        }

        public string GetRecord()
        {
            if (Wins == 0 && Losses == 0 && OTLosses == 0) return String.Empty;
            return $" ({this.Wins}-{this.Losses}-{this.OTLosses}) ";
        }

        public static List<Game> BuildPreferredGames(JArray games, List<string> teams)
        {
            List<Game> preferredGames = new List<Game>();
            foreach (JObject game in games)
            {
                if (teams.Contains(game["teams"]["away"]["team"]["name"].ToString().ToLower()) ||
                    teams.Contains(game["teams"]["home"]["team"]["name"].ToString().ToLower()))
                {
                    preferredGames.Add(new Game(game, teams));
                }
            }

            return preferredGames;
        }
    }
}
