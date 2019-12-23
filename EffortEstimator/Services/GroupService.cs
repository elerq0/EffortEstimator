using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            joiningKey = joiningKey.Trim();

            if (joiningKey.Length > 20)
                throw new Exception("Wrong joining key!");

            return sql.JoinGroup(email, joiningKey);
        }

        public List<UserGroupEntity> GetGroups(string email)
        {
            return sql.GetGroups(email);
        }

        private bool CreateChannel(string groupName, int conferenceId)
        {
            string channelName = keyGenerator.GetChannelName(groupName);
            if (sql.CreateChannel(groupName, conferenceId, channelName))
                return true;
            else
                return CreateChannel(groupName, conferenceId);
        }

        public string GetChannel(string email, string groupName, int conferenceId)
        {
            return sql.GetChannelName(email, groupName, conferenceId);
        }

        public bool CreateConference(string email, string groupName, string topic, string description, DateTime startDate)
        {
            if (string.IsNullOrEmpty(topic))
                throw new Exception("Topic can't be empty!");

            if (groupName.IndexOf("'") != -1 || groupName.IndexOf("\"") != -1)
                throw new Exception("Group name has a forbidden symbol");

            if (topic.IndexOf("'") != -1 || topic.IndexOf("\"") != -1)
                throw new Exception("Topic has a forbidden symbol");

            if (string.IsNullOrEmpty(description))
                description = "";

            if (description.IndexOf("'") != -1 || description.IndexOf("\"") != -1)
                throw new Exception("Description has a forbidden symbol");

            if (startDate < DateTime.Now)
                throw new Exception("Start date can't be in the past!");

            

            int ConId = sql.CreateConference(email, groupName, topic, description, startDate);

            if(CreateChannel(groupName.Replace(" ", ""), ConId))
            {
                List<string> mails = sql.GetMailsOfGroupMembers(groupName);
                foreach(string mail in mails)
                    mailOperator.SendUserConferenceInfo(mail, groupName, topic, startDate);

                return true;
            }

            return false;
        }

        public List<ConferenceEntity> GetConferences(string email)
        {
            return sql.GetConferences(email);
        }

    }
}
