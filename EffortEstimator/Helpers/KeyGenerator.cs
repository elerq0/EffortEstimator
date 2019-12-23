using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffortEstimator.Helpers
{
    public class KeyGenerator
    {
        private readonly Random random;
        public KeyGenerator()
        {
            random = new Random();
        }

        public string GetUserActivactionKey()
        {
            return GetRandomStringKey(6);
        }

        public string GetGroupJoiningKey()
        {
            return GetRandomStringKey(20);
        }

        public string GetChannelName(string groupName)
        {
            string channelName;
            if (groupName.Length > 8)
                channelName = groupName.Substring(0, 8) + GetRandomStringKey(12);
            else
                channelName = groupName + GetRandomStringKey(20 - groupName.Length);

            return channelName;
        }

        private string GetRandomStringKey(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
