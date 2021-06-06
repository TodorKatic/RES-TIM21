using System;

namespace Common
{
    public class TimeKeeper:IGetUnixTimestamp
    {
        public long GetUnixTimestamp()
        {
            return ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
        }
    }
}
