using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using DeviceComponent.ConfigurationUtils;
using DeviceComponent.DeviceDataConverter;
using DeviceComponent.DataSendingServices;

namespace DeviceComponent
{
    public class DeviceSimulator
    {
        public  AbstractDevice Device { get; private set; }

        public string DeviceDataEndpoint { get; private set; }
        public DeviceType Type { get; private set; }

        public uint DeviceCycleLength { get; private set; }
        public uint RetryTime { get; private set; }

        public ISendDeviceData DataSender { get; private set; }
        public ISetEndpoint DataSenderEndpointSetter { get; private set; }

        public IMakeDeviceData DataMaker { get; private set; }
        public IGetUnixTimestamp TimeKeep { get; private set; }
        public IChannelFactory<ISendDeviceData> DataSenderChannelFactory { get; private set; }

        public IGetEndpoints EndpointGetter { get; private set; }

        public ManualResetEvent resetEvent { get; private set; }



        public DeviceSimulator(IReadInput inputReader,IGetUnixTimestamp TimeKeep,IGetEndpoints endpointGetter,IHasherDevice HashCodeGetter,
            IChannelFactory<ISendDeviceData> DataSenderChannelFactory,IMakeDeviceData dataMaker,ISendDeviceData dataSender,ISetEndpoint dataSenderEndpointSetter,
            uint RetryTime,uint Cycle)
        {

            if (inputReader == null)
                throw new ArgumentNullException("InputReader ne sme biti null");
            if (TimeKeep == null)
                throw new ArgumentNullException("TimeKeep ne sme biti null");
            if (endpointGetter == null)
                throw new ArgumentNullException("EndpointGetter ne sme biti null");
            if (dataMaker == null)
                throw new ArgumentNullException("DataMaker ne sme biti null");
            if (dataSender == null)
                throw new ArgumentNullException("DataSender ne sme biti null");
            if (dataSenderEndpointSetter == null)
                throw new ArgumentNullException("DataSenderEndpointSetter ne sem bit null");
            if (DataSenderChannelFactory == null)
                throw new ArgumentNullException("DataSenderChannelFactory ne sme biti null");
            if (HashCodeGetter == null)
                throw new ArgumentNullException("HashCodeGetter ne sme biti null");

            this.DataSender = dataSender;
            this.DataSenderEndpointSetter = dataSenderEndpointSetter;
            this.TimeKeep = TimeKeep;
            this.EndpointGetter = endpointGetter;
            this.DataSenderChannelFactory = DataSenderChannelFactory;
            this.DataMaker = dataMaker;
            this.resetEvent = new ManualResetEvent(true);
            this.DeviceCycleLength = Cycle;
            this.RetryTime = RetryTime;


            Console.WriteLine("*******POKRENUT PROCES KONFIGURISANJA UREDJAJA*******");

            this.Type = ChooseDeviceType(inputReader);
            this.Device.DeviceCode = ConfigureDeviceCode(HashCodeGetter);

            this.DeviceDataEndpoint = ConfigureEndpoint(inputReader);

            Console.WriteLine("*******PROCES KONFIGURISANJA JE USPESNO ZAVRSEN*******");


            this.DataSenderEndpointSetter.SetEndpoint(this.DeviceDataEndpoint,this.DataSenderChannelFactory);

        }


        /// <summary>
        /// racuna se hashcode i pokusava se provera jedinstevnosti hashcoda sa ams-om
        /// ako je provera uspesna uredjaju se dodeljuje hash code
        /// ako hashcode nije jedinstven racuna se novi i ponovo se pokrece provera
        /// ako se iz nekog razloga ne moze izvrsiti provera, ams nije online ili je nastala greska u komunikaciji uredjaj ce pokusavati 
        /// da stupi u kontakt sa ams-om dok se ne iskljuci ili dok ams ne bude ponovo online
        /// </summary>
        public int ConfigureDeviceCode(IHasherDevice HashCodeGetter)
        {
            if (HashCodeGetter == null)
                throw new ArgumentNullException("Objekat klase koja dobavlja hash code ne sme biti null");

            int code = HashCodeGetter.GetDeviceCode(Type);
            return code;
        }
       
        public string ConfigureEndpoint(IReadInput inputReader)
        {
            if (inputReader == null)
                throw new ArgumentNullException("InputReader ne sme biti null");

            string retval = string.Empty;
            do
            {
                try
                {
                    List<string> endpoints = this.EndpointGetter.GetEndpoints().ToList<string>();

                    if (endpoints.Count > 0) {
                        int chosen = -1;
                        do
                        {
                            for (int i = 0; i < endpoints.Count; i++)
                                Console.WriteLine($"{i + 1}.{endpoints[i]}");

                            if (!int.TryParse(inputReader.ReadInputLine(), out chosen))
                                chosen = -1;
                        } while (chosen == -1 || (chosen < 1 || chosen > endpoints.Count));

                        retval = endpoints[chosen - 1];
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            } while (retval == string.Empty);

            return retval;
        }


        public string ReconfigureEndpoint()
        {
            string retval = string.Empty;
            do
            {
                try
                {
                    List<string> endpoints = this.EndpointGetter.GetEndpoints().ToList<string>();
                    if (endpoints.Count > 0)
                    {
                        retval = endpoints[0];
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            } while (retval == string.Empty);
            
            return retval;
        }


        public DeviceType ChooseDeviceType(IReadInput inputReader)
        {

            if (inputReader == null)
                throw new ArgumentNullException("InputReader ne sme biti null");

            int x;

            DeviceType type = DeviceType.Analog;

            do
            {
                System.Console.WriteLine("1.Analogni uredjaj");
                System.Console.WriteLine("2.Digitalni uredjaj");

                string read = inputReader.ReadInputLine();
                if (!int.TryParse(read, out x))
                      x = 0;

            } while (x < 1 && x > 2);

            switch (x)
            {
                case 1:
                    Device = new AnalogDevice(int.Parse(ConfigurationManager.AppSettings["LimitingValue"]));
                    type = DeviceType.Analog;
                    break;
                case 2:
                    Device = new DigitalDevice();
                    type = DeviceType.Digital;
                    break;
            }

            return type;
        }

        /// <summary>
        /// Pokusava se slanje podatka ka endpointu 
        /// ako je uspesno simulira se promena stanja uredjaja a zatim se ceka da prodje vreme koje predstavlja njegov ciklus
        /// ako je neuspesno pokrece se ponovna konfiguracija endpointa
        /// rekonfiguracija treba da bude automatska
        /// ams bi vracao listu endpointa pa samim tim moze se napraviti round robin algoritam
        /// odnosno ams bi mogao vracati listu endpointa sa uvek razlicitim prvim endpointom
        /// a na strani uredjaja napraviti metodu koja ce automatski pri rekonfiguraciji birati prvi endpoint
        /// </summary>

        public void Run()
        {
            Console.WriteLine("Ime uredjaja:"+this.Device.DeviceCode);
            Console.WriteLine("Endpoint:"+this.DeviceDataEndpoint);
            Console.WriteLine("\n\n");
            while (true)
            {
                
                resetEvent.WaitOne();
                if (!DataSender.SendDeviceData(DataMaker.GetDeviceData(Device,this.TimeKeep)))
                {
                    this.DeviceDataEndpoint = ReconfigureEndpoint();
                    Console.WriteLine("\n\n");

                    Console.WriteLine("Izvrsena rekonfiguracija");
                    Console.WriteLine("Ime uredjaja:"+this.Device.DeviceCode);
                    Console.WriteLine("Endpoint na koji salje podatke:"+this.DeviceDataEndpoint);
                    Console.WriteLine("\n\n");
                    this.DataSenderEndpointSetter.SetEndpoint(this.DeviceDataEndpoint,this.DataSenderChannelFactory);
                }
                else
                {
                    Device.ChangeState();
                }
                Thread.Sleep((int)DeviceCycleLength);
            }
        }

        public void Pause()
        {
            this.resetEvent.Reset();
        }

        public void Resume()
        {
            this.resetEvent.Set();
        }
    }
}
