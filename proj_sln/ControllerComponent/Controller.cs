using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.Concurrent;
using System.Threading;
using ControllerComponent.AmsCommunication;
using Common.ControllerDataModel;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using Common.DeviceDataModel;
using Common.HashCodeUtils;
using ControllerComponent.ConfigurationUtils;
using Common.ControllerDataModel.DataModelInterfaces;

namespace ControllerComponent
{

    public class Controller : IController
    {
        public ManualResetEvent manualReset { get; private set; }

        public uint RetryTime { get; private set; }
        public uint ControllerCycle { get; private set; }

        public int ControllerCode { get; private set; }
        public string LocalDatabasePath { get; private set; }
        public string AliveEndpoint { get; private set; }
        public string DataEndpoint { get; private set; }

        public IServiceHost<DeviceDataSaver, ISendDeviceData> DataServiceHost { get; private set; }
        public IServiceHost<AliveMaintainer, ICheckAlive> aliveMaintainerHost { get; private set; }
        public IGetUnixTimestamp TimeKeep { get; private set; }

        public ISendControllerData DataSender { get; private set; }
        public IMakeControllerData dataMaker { get; private set; }
        public IHasherController HasherClient;

        public ConcurrentDictionary<int, IList<InnerDeviceData>> LocalBuffer { get; private set; }



        /// <summary>
        /// Reaguje na cancel event odnosno simulira se pad kontrolera
        /// Cuva lokalnu bazu podataka u formatu ControllerData
        /// 
        /// Trebalo je cuvati kao Dictionary<>
        /// 
        /// Ali XmlSerializer ne dozvoljava serijalizovanje klasa koje implementiraju IDictionary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected void ExitHandler(object sender, ConsoleCancelEventArgs e)
        {
            ControllerData data = this.dataMaker.MakeControllerData(this, this.TimeKeep);
            SaveDatabase(this.LocalDatabasePath,data);
        }

        public void SaveDatabase(string DatabasePath,ControllerData data)
        {
            if (data == null)
                throw new ArgumentNullException("Prosledjeni podaci ne smeju biti null");

            if (DatabasePath == null)
                throw new ArgumentNullException("Put do baze podataka ne sme biti null");

            DatabasePath = DatabasePath.Trim();
            if (DatabasePath == string.Empty)
                throw new ArgumentException("Put do baze podataka ne sme biti prazan string");


            XmlSerializer xml = new XmlSerializer(typeof(ControllerData));
            if (!File.Exists(DatabasePath))
            {
                using (FileStream fstream = File.Create(DatabasePath))
                {
                    using (StreamWriter writer = new StreamWriter(fstream))
                    {
                        xml.Serialize(writer, data);
                    }
                }
            }
        }

        /// <summary>
        /// Ucitava lokalnu bazu palog kontrolera ako postoji lokalna baza kontrolera 
        /// 
        /// Ucitava u tip ControllerData i ubacuje podatke u lokalni bafer podataka
        /// </summary>
        /// <param name="DatabasePath"></param>
        public void LoadDatabaseIfExists(string DatabasePath)
        {
            if (DatabasePath == null)
                throw new ArgumentNullException("Put do baze podataka ne sme biti null");

            DatabasePath = DatabasePath.Trim();
            if (DatabasePath == string.Empty)
                throw new ArgumentException("Put do baze podataka ne sme biti prazan string");

            lock (DatabasePath)
            {
                if (File.Exists(DatabasePath))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(ControllerData));

                    using (StreamReader reader = new StreamReader(DatabasePath))
                    {
                        if (!reader.EndOfStream)
                        {
                            ControllerData deserializedData = (ControllerData)xml.Deserialize(reader);

                            foreach (var dataNode in deserializedData.DevicesDataList)
                            {
                                this.LocalBuffer.TryAdd(dataNode.DeviceCode, dataNode.DataList);
                            }
                        }
                        Console.WriteLine("Loaded database");
                    }
                    File.Delete(this.LocalDatabasePath);
                }
            }
        }


        public Controller(ConcurrentDictionary<int, IList<InnerDeviceData>> Buffer, uint Cycle, uint RetryTime, IMakeControllerData dataMaker, IHasherController HasherClient,
            ISendControllerData dataSender, IServiceHost<DeviceDataSaver, ISendDeviceData> DataSaverHost,
            IServiceHost<AliveMaintainer, ICheckAlive> AliveHost, IGetUnixTimestamp TimeKeep,string aliveEndpoint,string dataEndpoint)
        {

            if (Buffer == null)
                throw new ArgumentNullException("Prosledjena instanca lokalnog bafera ne sme biti null");

            if (dataMaker == null)
                throw new ArgumentNullException("Instanca klase za pravljenje podataka ne sme biti null");

            if (HasherClient == null)
                throw new ArgumentNullException("Instanca klijenta za dobijanje hash codea ne sme biti null");

            if (dataSender == null)
                throw new ArgumentNullException("Instanca klase za slanje podataka ne sme biti null");
            if (DataSaverHost == null)
                throw new ArgumentNullException("Host usluge za cuvanje podataka ne sme biti null");
            if (AliveHost == null)
                throw new ArgumentNullException("Host ICheckAlive usluge ne sme biti null");
            if (TimeKeep == null)
                throw new ArgumentNullException("Instanca klase TimeKeeper ne sme biti null");

            if (aliveEndpoint == null)
                throw new ArgumentNullException("Alive Endpoint ne sme biti null");

            aliveEndpoint = aliveEndpoint.Trim();
            if (aliveEndpoint == string.Empty)
                throw new ArgumentException("Alive Endpoint ne sme biti prazan string");

            if (!CheckNetTcpUrl(aliveEndpoint))
                throw new ArgumentException("Alive Endpoint nije pravilnog formata");

            if (dataEndpoint == null)
                throw new ArgumentNullException("Data Endpoint ne sme biti null");

            dataEndpoint = dataEndpoint.Trim();
            if (dataEndpoint == string.Empty)
                throw new ArgumentException("Data Endpoint ne sme biti prazan string");

            if (!CheckNetTcpUrl(dataEndpoint))
                throw new ArgumentException("Data Endpoint nije pravilnog formata");


            Console.CancelKeyPress += new ConsoleCancelEventHandler(ExitHandler);
            this.manualReset = new ManualResetEvent(true);


            this.AliveEndpoint = aliveEndpoint;
            this.DataEndpoint = dataEndpoint;
            this.TimeKeep = TimeKeep;
            this.LocalBuffer = Buffer;
            this.RetryTime = RetryTime;
            this.ControllerCycle = Cycle;
            this.HasherClient = HasherClient;
            this.dataMaker = dataMaker;
            this.ControllerCode = GetDeviceCode(this.HasherClient);
            this.LocalDatabasePath = GetDataBasePath(ConfigurationManager.AppSettings["DatabasesFolder"]);
            LoadDatabaseIfExists(this.LocalDatabasePath);

            this.DataSender = dataSender;

            this.DataServiceHost = DataSaverHost;
            this.aliveMaintainerHost = AliveHost;
            
            this.DataServiceHost.Open();
            this.aliveMaintainerHost.Open();
        }


        /// <summary>
        /// proverava korektnost nettcp url-a
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool CheckNetTcpUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriRes) && (uriRes.Scheme == Uri.UriSchemeNetTcp);
        }



        /// <summary>
        /// Cita put do lokalnog foldera u kom se nalaze sve baze podataka instanci koje su pale
        /// Ako postoji baza u folderu uzece prvu, ucitati je i obrisati fajl
        /// Ako ne postoji baza u folderu dobija default ime 
        /// </summary>
        public string GetDataBasePath(string localBaseFolder)
        {
            if (localBaseFolder == null)
                throw new ArgumentNullException("Putanja do foldera lokalnih baza ne sme biti null");

            localBaseFolder = localBaseFolder.Trim();
            if (localBaseFolder == string.Empty)
                throw new ArgumentException("Putanja do foldera lokalnih baza ne sme biti prazna");


            string retval = string.Empty;

            string localDatabasesFolder = localBaseFolder;
            string defaultPath = $"{localDatabasesFolder}\\LocalDatabase{this.ControllerCode}.xml";
            retval = defaultPath;
            if (Directory.Exists(localDatabasesFolder))
            {
                IEnumerable<string> localDatabases = Directory.EnumerateFiles(localDatabasesFolder);
                retval = localDatabases.FirstOrDefault<string>() ?? defaultPath;
            }
            else
            {
                Directory.CreateDirectory(localDatabasesFolder);
            }

            return retval;
        }


        /// <summary>
        /// Dobija od amsa jedinstveno ime uredjaja
        /// </summary>
        public int GetDeviceCode(IHasherController HashCodeGetter, int MyCode = -1)
        {
            if (HashCodeGetter == null)
                throw new ArgumentNullException("Instanca klase koja dobavlja hash code ne sme biti null");

            int code = HashCodeGetter.GetDeviceCode(this.AliveEndpoint, this.DataEndpoint, MyCode);

            Console.WriteLine($"REGISTERED ON AMS WITH CODE:{code}");
            return code;
        }

        public void Run()
        {
            Console.WriteLine("Ime lokalnog kontrolera:"+this.ControllerCode);
            Console.WriteLine("Endpoint podataka lokalnog kontrolera:"+this.DataEndpoint);
            Console.WriteLine("Put do lokalne baze:"+this.LocalDatabasePath);
            Console.WriteLine("\n\n");

            while(true)
            {
                manualReset.WaitOne();
                lock (this.LocalBuffer)
                {
                    if (this.LocalBuffer.Count > 0)
                    {
                        if (this.DataSender.SendControllerData(this.dataMaker.MakeControllerData(this, this.TimeKeep)))
                        {
                            Console.WriteLine("LOKALNA BAZA JE PROSLEDJENA AMS-u");
                            this.LocalBuffer.Clear();
                        }
                    }
                }
                Thread.Sleep((int)this.ControllerCycle);

            }
        }


        /// <summary>
        /// Vrsi zaustavljanje kontrolera
        /// Potrebno je zaustaviti i sve niti usluga na kontroleru koje se vrte u pozadini
        /// najlaksi nacin je jednostavno zatvoriti instance ServisHost klase
        /// </summary>

        public void Pause()
        {
            manualReset.Reset();
            this.aliveMaintainerHost.Close();
            this.DataServiceHost.Close();
        }


        /// <summary>
        /// Restartovanje kontrolera
        /// Potrebno je ponovo otvoriti prethodno zatvorene hostove i pnovo se prijaviti na AMS jer se desila rekonfiguracija uredjaja 
        /// pa je endpoint zaustavljenog kontrolera izbirsan iz evidencije AMS-a
        /// ponovna registracija se vrsi istom metodom kao i inicijalna registracija
        /// samo sto se kao parametar salje kod koji je kontroler dobio od AMS-a inicijalno
        /// </summary>

        public void Resume(IServiceHost<AliveMaintainer,ICheckAlive> newAliveHost,IServiceHost<DeviceDataSaver,ISendDeviceData> newDataHost)
        {
            if (newAliveHost == null)
                throw new ArgumentNullException("Prosledjeni host za Alive uslugu ne sme biti null");
            if (newDataHost == null)
                throw new ArgumentNullException("Prosledjeni host za uslugu podataka ne sme biti null");

            this.aliveMaintainerHost = newAliveHost;
            this.DataServiceHost = newDataHost;
            
            this.aliveMaintainerHost.Open();
            this.DataServiceHost.Open();

            GetDeviceCode(this.HasherClient, this.ControllerCode);

            manualReset.Set();
        }
    }
}
