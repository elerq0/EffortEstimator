using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using EffortEstimator.Helpers;
using EffortEstimator.Models;

namespace EffortEstimator.Services
{
    public class GroupService
    {
        private readonly MySQL sql;
        private readonly KeyGenerator keyGenerator;
        private readonly MailOperator mailOperator;
        public GroupService(string connectionString)
        {
            mailOperator = new MailOperator();
            keyGenerator = new KeyGenerator();
            sql = new MySQL(connectionString);
        }

        public bool CreateGroup(string groupName, string email)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new Exception("Group name can't be empty!");

            if (groupName.Length >= 100)
                throw new Exception("Group name is too long!");

            if (groupName.IndexOf("'") != -1 || groupName.IndexOf("\"") != -1)
                throw new Exception("Group name has a forbidden symbol");

            if (sql.CreateGroup(groupName, email))
                CreateChannel(groupName.Replace(" ", ""), 0);

            return true;
        }

        public string CreateJoiningKey(string groupName, string email)
        {
            if (groupName.Length > 100)
                throw new Exception("Group name is too long!");

            if (string.IsNullOrEmpty(groupName))
                throw new Exception("Group name can't be empty!");

            string joiningKey = keyGenerator.GetGroupJoiningKey();

            if (sql.RenewJoiningKey(email, groupName, joiningKey))
                return joiningKey;
            else
                return CreateJoiningKey(groupName, email);
        }

        public string JoinGroup(string email, string joiningKey)
        {
            if(string.IsNullOrEmpty(joiningKey))
                throw new Exception("Joining key can't be empty!");

            joiningKey = joiningKey.Trim();

            if (joiningKey.Length > 20)
                throw new Exception("Wrong joining key!");

            return sql.JoinGroup(email, joiningKey);
        }

        public List<UserGroupEntity> GetGroups(string email)
        {
            return sql.GetGroups(email);
        }

        private string CreateChannel(string groupName, int conferenceId)
        {
            string channelName = keyGenerator.GetChannelName(groupName.Replace(" ", ""));
            if (sql.CreateChannel(groupName, conferenceId, channelName))
                return channelName;
            else
                return CreateChannel(groupName, conferenceId);
        }

        public string GetChannel(string email, string groupName, int conferenceId)
        {
            return sql.GetChannelName(email, groupName, conferenceId);
        }

        public async Task<bool> CreateConference(string email, string groupName, string topic, string description, DateTime startDate, IFormFile file)
        {
            if (string.IsNullOrEmpty(topic))
                throw new Exception("Topic can't be empty!");

            if (topic.Length >= 300)
                throw new Exception("Topic is too long!");

            if (groupName.IndexOf("'") != -1 || groupName.IndexOf("\"") != -1)
                throw new Exception("Group name has a forbidden symbol");

            if (topic.IndexOf("'") != -1 || topic.IndexOf("\"") != -1)
                throw new Exception("Topic has a forbidden symbol");

            if (string.IsNullOrEmpty(description))
                description = "";

            if (description.Length >= 3000)
                throw new Exception("Description is too long!");

            if (description.IndexOf("'") != -1 || description.IndexOf("\"") != -1)
                throw new Exception("Description has a forbidden symbol");

            if (startDate < DateTime.Now)
                throw new Exception("Start date can't be in the past!");

            // Weryfikacja danych

            int ConId = sql.CreateConference(email, groupName, topic, description, startDate);
            string channelName = CreateChannel(groupName, ConId);

            List<string> mails = sql.GetMailsOfGroupMembers(groupName);
            foreach (string mail in mails)
                mailOperator.SendUserConferenceInfo(mail, groupName, topic, startDate);

            if (file != null)
                await FileOperator.UploadFile(channelName, file);

            return true;
        }

        public Tuple<bool, string> CheckIfFileExist(string channelName)
        {
            return FileOperator.CheckIfExist(channelName);
        }

        public string GetFilePath(string channelName)
        {
            return FileOperator.GetFilePath(channelName);
        }

        public List<ConferenceEntity> GetConferences(string email)
        {
            return sql.GetConferences(email);
        }

        public bool VoteInConference(string email, string chaName, double result)
        {
            if (double.IsNaN(result))
                throw new Exception("Result is not a number!");

            return sql.VoteInConference(email, chaName, result);
        }

        public EstimationForm GetEstimationForm(string email, string chaName)
        {
            ConferenceInfo info = sql.GetConferenceInfo(chaName);
            List<ConferenceResult> results = sql.GetConferenceResults(chaName);
            double UserResultValue = 0, MinResultValue = 0, MaxResultValue = 0, AvgResultValue = 0;

            if (results.Count > 0)
            {
                if (results.Select(x => x.Email).Contains(email))
                    UserResultValue = results.Where(x => x.Email == email).Single().Result;

                MinResultValue = results.Min(x => x.Result);
                MaxResultValue = results.Max(x => x.Result);
                AvgResultValue = results.Average(y => y.Result);
            }

            return new EstimationForm()
            {
                Topic = info.Topic,
                Description = info.Description,
                Iteration = info.State,
                UserResultValue = UserResultValue,
                MinResultValue = MinResultValue,
                MaxResultValue = MaxResultValue,
                AvgResultValue = AvgResultValue,
                ProposedValue = (MinResultValue + 4 * AvgResultValue + MaxResultValue) / 6,
                ResultValues = results.Select(x => x.Result).ToList()
            };
        }

        public bool IncrementConferenceState(string email, string chaName)
        {
            ConferenceInfo info = sql.GetConferenceInfo(chaName);
            return sql.SetConferenceState(email, chaName, info.State + 1);
        }

        public bool ZeroConferenceState(string email, string chaName)
        {
            return sql.SetConferenceState(email, chaName, 0);
        }
    }
}
