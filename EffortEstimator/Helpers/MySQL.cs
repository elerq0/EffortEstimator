using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using EffortEstimator.Models;

namespace EffortEstimator.Helpers
{
    public class MySQL
    {
        public string ConnectionString { get; set; }

        public MySQL(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public UserEntity GetUser(string email)
        {
            UserEntity user = new UserEntity();
            try
            {
                using MySqlConnection cnn = GetConnection();
                cnn.Open();

                using MySqlCommand cmd = new MySqlCommand("call GetUserInfo ('" + email + "')", cnn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    user.Id = (int)reader["Usr_UsrId"];
                    user.Email = reader["Usr_Email"].ToString();
                    user.Hash = reader["Usr_Hash"].ToString();
                    user.Name = reader["Usr_Name"].ToString();
                    user.Surname = reader["Usr_Surname"].ToString();
                    user.Active = (bool)reader["UAc_Active"];
                }
            }
            catch (Exception e)
            {
                Logger.Log("MySQL-GetUser email=[" + email + "]: " + e.Message);
                throw new Exception("There was an error, try again later!");
            }

            if (user != null)
                return user;
            else
                throw new Exception("User not found!");
        }

        public bool Register(string email, string hash, string name, string surname, string activationKey)
        {
            try
            {
                using MySqlConnection cnn = GetConnection();
                cnn.Open();
                using MySqlCommand cmd = new MySqlCommand("call RegisterUser ('" + email + "', '" + hash + "', '" + name + "', '" + surname + "', '" + activationKey + "')", cnn);
                cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                Logger.Log("MySQL-Register email=[" + email + "], hash=[" + hash + "], name=[" + name + "], surname=[" + surname + "], activationKey=[" + activationKey + "]: " + e.Message);
                throw new Exception("There was an error, try again later!");
            }
            return true;
        }

        public bool ActivateUser(string email, string activationKey)
        {
            try
            {
                using MySqlConnection cnn = GetConnection();
                cnn.Open();
                using MySqlCommand cmd = new MySqlCommand("call ActivateUser ('" + email + "', '" + activationKey + "')", cnn);
                cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                if (e.Message == "Wrong activation key!")
                    throw new Exception(e.Message);

                if (e.Message == "User already activated!")
                    throw new Exception(e.Message);

                Logger.Log("MySQL-ActivateUser email=[" + email + "], activationKey=[" + activationKey + "]: " + e.Message);
                throw new Exception("There was an error, try again later!");
            }

            return true;
        }

        public bool CreateGroup(string groupName, string email)
        {
            try
            {
                using MySqlConnection cnn = GetConnection();
                cnn.Open();
                using MySqlCommand cmd = new MySqlCommand("call CreateGroup ('" + groupName + "', '" + email + "')", cnn);
                cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                if (e.Message == "There is already an existing group with that name!")
                    throw new Exception(e.Message);

                Logger.Log("MySQL-CreateGroup email=[" + email + "], groupName=[" + groupName + "]: " + e.Message);
                throw new Exception("There was an error, try again later!");
            }

            return true;
        }

        public bool CreateChannel(string groupName, int conferenceId, string channelName)
        {
            try
            {
                using MySqlConnection cnn = GetConnection();
                cnn.Open();
                using MySqlCommand cmd = new MySqlCommand("call CreateChannel ('" + groupName + "', '" + conferenceId + "', '" + channelName + "')", cnn);
                cmd.ExecuteReader();
            }
            catch(Exception e)
            {
                if (e.Message == "There is already an existing channel with that name!")
                    return false;

                Logger.Log("MySQL-CreateChannel groupName=[" + groupName + "], conferenceId=[" + conferenceId + "], channelName=[" + channelName + "]: " + e.Message);
                throw new Exception("There was an error, try again later!");
            }

            return true;
        }

        public string GetChannelName(string email, string groupName, int conferenceId)
        {
            try
            {
                using MySqlConnection cnn = GetConnection();
                cnn.Open();
                using MySqlCommand cmd = new MySqlCommand("call GetChannelName ('" + email + "', '" + groupName + "', '" + conferenceId + "')", cnn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    return reader["Cha_Name"].ToString();
                }

                throw new Exception("Channel not found!");
            }
            catch (Exception e)
            {
                if (e.Message == "You do not have access to this!")
                    throw new Exception(e.Message);

                Logger.Log("MySQL-GetChannelName email=[" + email + "], groupName=[" + groupName + "], conferenceId=[" + conferenceId + "]: " + e.Message);
                throw new Exception("There was an error, try again later!");
            }
        }

        public int CreateConference(string email, string groupName, string topic, string description, DateTime startDate)
        {
            try
            {
                using MySqlConnection cnn = GetConnection();
                cnn.Open();
                using MySqlCommand cmd = new MySqlCommand("call CreateConference ('" + email + "', '" + groupName + "', '" + topic + "', '" + description + "', '" + startDate.ToString("yyyy-MM-dd HH:mm:ss") + "')", cnn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    return Convert.ToInt32(reader["ConId"]);
                }

                throw new Exception("Conference was not created?");
            }
            catch (Exception e)
            {
                if (e.Message == "You do not have access to this!")
                    throw new Exception(e.Message);

                Logger.Log("MySQL-CreateConference email=[" + email + "], groupName=[" + groupName + "], topic=[" + topic + "], description=[" + description + "], startDate=[" + startDate.ToString("yyyy-MM-dd HH:mm:ss") + "]: " + e.Message);
                throw new Exception("There was an error, try again later!");
            }
        }

        public List<ConferenceEntity> GetConferences(string email)
        {
            try
            {
                using MySqlConnection cnn = GetConnection();
                cnn.Open();
                using MySqlCommand cmd = new MySqlCommand("call GetMyConferences ('" + email + "')", cnn);
                using var reader = cmd.ExecuteReader();

                List<ConferenceEntity> conferences = new List<ConferenceEntity>();
                while (reader.Read())
                {
                    conferences.Add(new ConferenceEntity()
                    {
                        ConferenceId = (int)reader["Con_ConId"],
                        GroupName = reader["Grp_Name"].ToString(),
                        Topic = reader["Con_Topic"].ToString(),
                        StartDate = DateTime.Parse(reader["Con_StartDate"].ToString()),
                        Description = reader["Con_Description"].ToString()
                    });
                }
                return conferences;
            }
            catch (Exception e)
            {
                Logger.Log("MySQL-GetConferences email=[" + email + "]: " + e.Message);
                throw new Exception("There was an error, try again later!");
            }
        }

        public bool RenewJoiningKey(string email, string groupName, string joiningKey)
        {
            try
            {
                using MySqlConnection cnn = GetConnection();
                cnn.Open();
                using MySqlCommand cmd = new MySqlCommand("call RenewJoiningKey ('" + email + "', '" + groupName + "', '" + joiningKey + "')", cnn);
                cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                if (e.Message == "Joining key like that already exists")
                    return false;

                if (e.Message == "You do not have access to this!")
                    throw new Exception(e.Message);

                Logger.Log("MySQL-RenewJoiningKey email=[" + email + "], groupName=[" + groupName + "], joiningKey=[" + joiningKey + "]: " + e.Message);
                throw new Exception("There was an error, try again later!");
            }

            return true;
        }

        public string JoinGroup(string email, string joiningKey)
        {
            try
            {
                using MySqlConnection cnn = GetConnection();
                cnn.Open();
                using MySqlCommand cmd = new MySqlCommand("call JoinGroup ('" + email + "', '" + joiningKey + "')", cnn);
                using var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    return reader["Grp_Name"].ToString();
                }
                throw new Exception("Reader empty");
            }
            catch (Exception e)
            {
                if (e.Message == "Wrong joining key!")
                    throw new Exception(e.Message);

                if (e.Message == "You are already part of this group!")
                    throw new Exception(e.Message);

                Logger.Log("MySQL-JoinGroup email=[" + email + "], joiningKey=[" + joiningKey + "]: " + e.Message);
                throw new Exception("There was an error, try again later!");
            }
        }

        public List<UserGroupEntity> GetGroups(string email)
        {
            try
            {
                using MySqlConnection cnn = GetConnection();
                cnn.Open();
                using MySqlCommand cmd = new MySqlCommand("call GetMyGroups ('" + email + "')", cnn);
                using var reader = cmd.ExecuteReader();

                List<UserGroupEntity> groups = new List<UserGroupEntity>();
                while (reader.Read())
                {
                    groups.Add( new UserGroupEntity()
                    {
                        Name = reader["Grp_Name"].ToString(),
                        Role = reader["GMe_Role"].ToString()
                    });
                }
                return groups;
            }
            catch (Exception e)
            {
                Logger.Log("MySQL-GetGroups email=[" + email + "]: " + e.Message);
                throw new Exception("There was an error, try again later!");
            }
        }

        public List<string> GetMailsOfGroupMembers(string groupName)
        {
            try
            {
                using MySqlConnection cnn = GetConnection();
                cnn.Open();
                using MySqlCommand cmd = new MySqlCommand("select Usr_Email from Groups_Members join Users on Usr_UsrId = GMe_UsrId join `Groups` on Grp_GrpId = GMe_GrpId where Grp_Name = '" + groupName + "'", cnn);
                using var reader = cmd.ExecuteReader();

                List<string> mails = new List<string>();
                while (reader.Read())
                {
                    mails.Add(reader["Usr_Email"].ToString());
                }
                return mails;
            }
            catch (Exception e)
            {
                Logger.Log("MySQL-GetMailsOfGroupMembers groupName=[" + groupName + "]: " + e.Message);
                throw new Exception("There was an error, try again later!");
            }
        }
    }
}
