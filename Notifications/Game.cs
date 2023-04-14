using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Notifications
{
    class Game
    {
        public Team Home { get; set; }
        public Team Away { get; set; }
        public Team PreferredTeam { get; set; }
        public Team OpposingTeam { get; set; }
        public DateTime PuckDrop { get; set; }
        public string Venue { get; set; }
        public string Status { get; set; }
        public Game(JObject game, List<string> preferredTeams)
        {
            Home = new Team((JObject)game["teams"]["home"]);
            Away = new Team((JObject)game["teams"]["away"]);
            PreferredTeam = GetPreferredTeam(preferredTeams);
            OpposingTeam = GetOpposingTeam();
            PuckDrop = DateTime.Parse(Convert.ToString(game["gameDate"])).ToLocalTime();
            Venue = Convert.ToString(game["venue"]["name"]);
            Status = Convert.ToString(game["status"]["detailedState"]);
        }

        private Team GetPreferredTeam(List<string> preferredTeams)
        {
            if (preferredTeams.Contains(this.Home.Name.ToLower())) return this.Home;

            return this.Away;
        }

        private Team GetOpposingTeam()
        {
            if (this.PreferredTeam.Name.ToLower() == this.Home.Name.ToLower()) return this.Away;

            return this.Home;
        }
    }
}
