using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Common;
using Common.ControllerDataModel;

namespace AMS.Communication
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple,IncludeExceptionDetailInFaults = true)]
    public class EndpointsGetterService : IGetEndpoints
    {

        public ConcurrentDictionary<int, Tuple<string, string>> RegisteredControllers { get; private set; }
        public string AMSDataEndpoint { get; private set; }
        public IChannelFactory<ICheckAlive> Factory { get; private set; }


        public EndpointsGetterService(ConcurrentDictionary<int, Tuple<string, string>> registeredControllers, string amsDataEndpoint,IChannelFactory<ICheckAlive> factory)
        {
            if (registeredControllers == null)
                throw new ArgumentNullException("Kolekcija prijavljenih kontrolera ne sme biti null");

            if (amsDataEndpoint == null)
                throw new ArgumentNullException("AMS Device Data Endpoint ne sme biti null");

            amsDataEndpoint = amsDataEndpoint.Trim();
            if (amsDataEndpoint == string.Empty)
                throw new ArgumentException("Prosledjeni endpoint ne sme biti prazan string");

            if (factory == null)
                throw new ArgumentNullException("Prosledjena facbika ne sme biti null");

            this.AMSDataEndpoint = amsDataEndpoint;
            this.RegisteredControllers = registeredControllers;
            this.Factory = factory;

        }

        public IEnumerable<string> GetEndpoints()
        {
            ConcurrentDictionary<int, bool> IsAlive = new ConcurrentDictionary<int, bool>();

            Task[] tasks = new Task[this.RegisteredControllers.Count];

            tasks = (from u in this.RegisteredControllers
                     select Task.Factory.StartNew(() => CheckIsEndpointAlive(u.Key, u.Value.Item1, ref IsAlive))).ToArray<Task>();

            Task.WaitAll(tasks);

            IEnumerable<string> toReturn = (from u in this.RegisteredControllers
                                            from v in IsAlive
                                            where u.Key == v.Key && v.Value
                                            select u.Value.Item2);




            IEnumerable<int> keysToRemove = (from u in IsAlive
                                             where !u.Value
                                             select u.Key);


            foreach (var toRemove in keysToRemove)
            {
                this.RegisteredControllers.TryRemove(toRemove, out var value);
            }

            toReturn = FisherYatesShuffle(toReturn);

            return toReturn;
        }



        public void CheckIsEndpointAlive(int key, string endpoint, ref ConcurrentDictionary<int, bool> IsAlive)
        {
            try
            {
                ICheckAlive aliveCheckerChannel = this.Factory.CreateTcpChannel(endpoint);
                IsAlive[key] = aliveCheckerChannel.CheckIfAlive();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                IsAlive[key] = false;
            }
        }


        /// <summary>
        /// pokusaj fiser-yates shuffle algoritma
        /// trebalo bi da je dovoljno dobro
        /// 
        /// iako je Random jako biased generator
        /// % (i + 1) nije unfirmno
        /// </summary>
        /// <param name="endpoints"></param>
        /// <returns></returns>
        public IEnumerable<string> FisherYatesShuffle(IEnumerable<string> endpoints)
        {

            List<string> list = endpoints.ToList<string>();
            Random rand = new Random(DateTime.Now.Second);

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rand.Next() % (i + 1);
                var temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }

            list.Add(this.AMSDataEndpoint);
            return list.AsEnumerable<string>();
        }
    }
}
