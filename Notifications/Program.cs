using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Notifications
{
    class Program
    {
        static void Main(string[] args)
        {
            JArray todaysGames = APIHandler.GetGamesFromDate(DateTime.Now.ToString("yyyy-MM-dd"));
            JArray yesterdaysGames = APIHandler.GetGamesFromDate(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            List<ContactSpec> contacts = ContactSpec.BuildContacts();

            foreach (ContactSpec contact in contacts)
            {
                contact.SendTodaysGameAlerts(Team.BuildPreferredGames(todaysGames, contact.Teams));
                contact.SendYesterdaysGameResults(Team.BuildPreferredGames(yesterdaysGames, contact.Teams));
            }
        }
    }
}
