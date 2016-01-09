using StatsdClient;

namespace spectator
{
    public class StatsdPublisher : IStatsdPublisher
    {
        private readonly IStatsd _statsdClient;

        public StatsdPublisher(IStatsd statsdClient)
        {
            _statsdClient = statsdClient;
        }

        public void SendCount()
        {

        }

        public void SendGauge()
        {

        }

        public void SendTiming()
        {

        }
    }
}