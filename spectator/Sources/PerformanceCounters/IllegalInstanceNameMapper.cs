namespace spectator.Sources.PerformanceCounters
{
    public class IllegalInstanceNameMapper : IIllegalInstanceNameMapper
    {
        public string Map(string instanceName)
        {
            var mappedName = instanceName.ToCharArray();

            for (int i = 0; i < mappedName.Length; i++)
            {
                switch (mappedName[i])
                {
                    case '(':
                        mappedName[i] = '[';
                        break;
                    case ')':
                        mappedName[i] = ']';
                        break;
                    case '#':
                    case '\\':
                    case '/':
                        mappedName[i] = '_';
                        break;
                }
            }

            return new string(mappedName);
        }
    }
}