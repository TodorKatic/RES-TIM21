using System;

namespace DeviceComponent.ConfigurationUtils
{
    public class InputReader : IReadInput
    {
        public string ReadInputLine()
        {
            string retval = Console.ReadLine();
            
            if (retval == null)
                retval = string.Empty;
            return retval;
        }
    }
}
