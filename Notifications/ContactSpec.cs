using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Mail;

namespace Notifications
{
    class ContactSpec
    {
        public static Common.Email CommonEmailClient = new Common.Email();
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Teams { get; set; }
        public bool SendGameAlert { get; set; }
        public bool SendGameResult { get; set; }

        private ContactSpec(NameValueCollection contactInfo)
        {
            Name = contactInfo["Name"];
            Email = contactInfo["Email"];
            Teams = BuildTeamsFromAppSettingSection(contactInfo["Teams"]);
            SendGameAlert = Convert.ToBoolean(contactInfo["SendGameAlert"]);
            SendGameResult = Convert.ToBoolean(contactInfo["SendGameResult"]);
        }

        public static List<ContactSpec> BuildContacts()
        {
            List<string> contactNames = GetContactsFromAppSettings();
            List<ContactSpec> cs = new List<ContactSpec>();

            foreach (string name in contactNames)
            {
                cs.Add(new ContactSpec(GetContactAppSettingSection(name)));
            }

            return cs;
        }

        private static NameValueCollection GetContactAppSettingSection(string section)
        {
            return (NameValueCollection)ConfigurationManager.GetSection(section);
        }

        private static List<string> BuildTeamsFromAppSettingSection(string delimitedTeams)
        {
            return delimitedTeams.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(team => team.ToLower().Trim()).ToList();
        }

        public void SendTodaysGameAlerts(List<Game> games)
        {
            if (!this.SendGameAlert) return;

            if (!games.Any())
            {
                CommonEmailClient.Send(GetNoGameAlertEmail());
                return;
            }

            string emailBody = String.Empty;
            foreach (Game game in games)
            {
                emailBody += GetTodaysGameEmailBody(game);
            }

            CommonEmailClient.Send(GetGameAlertEmail(emailBody.TrimEnd('\n')));
        }

        private static string GetTodaysGameEmailBody(Game game)
        {
            if (game.Status.ToLower() == "postponed")
            {
                return $"{game.Away.Name} @ {game.Home.Name} postponed.\n\n";
            }

            return $"{game.PreferredTeam.Name} playing at {game.PuckDrop.ToString("h:mmtt")}. {game.Away.Name}{game.Away.GetRecord()} @ {game.Home.Name}{game.Home.GetRecord()}.\n\n";
        }

        public void SendYesterdaysGameResults(List<Game> games)
        {
            if (!this.SendGameResult) return;

            if (!games.Any())  return;

            string emailBody = String.Empty;
            foreach (Game game in games)
            {
                emailBody += GetYesterdaysGameEmailBody(game);
            }

            CommonEmailClient.Send(GetGameResultEmail(emailBody.TrimEnd('\n')));
        }

        private string GetYesterdaysGameEmailBody(Game game)
        {
            if (game.Status.ToLower() == "postponed")
            {
                return $"{game.Away.Name} @ {game.Home.Name} postponed.\n\n";
            }

            return $"{game.PreferredTeam.Name} {(game.PreferredTeam.Score > game.OpposingTeam.Score ? "won" : "lost")}. Final Score: {game.PreferredTeam.Name} {game.PreferredTeam.Score}, {game.OpposingTeam.Name} {game.OpposingTeam.Score}.\n\n";
        }

        public static List<string> GetContactsFromAppSettings()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            List<string> contacts = new List<string>();

            var localSections = config.Sections.Cast<ConfigurationSection>().Where(s => s.SectionInformation.IsDeclared);
            foreach (ConfigurationSection section in localSections)
            {
                contacts.Add(section.SectionInformation.Name);
            }

            return contacts;
        }

        private MailMessage GetNoGameAlertEmail()
        {
            return new MailMessage(ConfigurationManager.AppSettings["FromEmail"], this.Email)
            {
                Subject = "Game Alert",
                Body = "There are no preferred games scheduled for today."
            };
        }

        private MailMessage GetGameAlertEmail(string body)
        {
            return new MailMessage(ConfigurationManager.AppSettings["FromEmail"], this.Email)
            {
                Subject = "Game Alert",
                Body = body
            };
        }

        private MailMessage GetGameResultEmail(string body)
        {
            return new MailMessage(ConfigurationManager.AppSettings["FromEmail"], this.Email)
            {
                Subject = "Game Result",
                Body = body
            };
        }
    }
}
