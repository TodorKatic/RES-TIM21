using AMS.HashCodeUtil;
using AMSDatabaseAccess.DatabaseCRUD;
using Common;
using Common.HashCodeUtils;
using System;
using System.Collections.Concurrent;
using System.ServiceModel;


namespace AMS.Communication
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple,IncludeExceptionDetailInFaults = true)]
    public class ControllerHashCodeService:IHasherController
    {
        public ICrudDaoDevice deviceTableHandle { get; private set; }
        public IHasher Hasher { get; private set; }
        public ConcurrentDictionary<int, Tuple<string,string>> RegisteredControllers { get; private set; }
        public IGetUnixTimestamp TimeKeep { get; private set; }
        public IMainWindowModel UIModel { get; private set; }

        public ControllerHashCodeService(ICrudDaoDevice deviceTableHandle, ConcurrentDictionary<int, Tuple<string,string>> controllers,IGetUnixTimestamp TimeKeep,IMainWindowModel model,IHasher hasher)
        {
            if (deviceTableHandle == null)
                throw new ArgumentNullException("Handle objekta koji radi sa tabelom uredjaja ne sme biti null");

            if (controllers == null)
                throw new ArgumentNullException("Referenca na kolekciju registrovanih kontrolera ne sme biit null");

            if (TimeKeep == null)
                throw new ArgumentNullException("Obejakt klase TimeKeeper ne sme biti null");

            if (model == null)
                throw new ArgumentNullException("Prosledjeni model ne sme biti null");

            if (hasher == null)
                throw new ArgumentNullException("Hasher ne sme biti null");

            this.deviceTableHandle = deviceTableHandle;
            this.RegisteredControllers = controllers;
            this.UIModel = model;
            this.Hasher = hasher;
            this.TimeKeep = TimeKeep;
        }

        /// <summary>
        /// Metoda koja registruje kontroler odnosno pravi red u bazi podataka 
        /// I upisuje ga u lokalnu kolekciju endpointa koja se kasnije proverava pri slanju endpointa ka uredjajima
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public int GetDeviceCode(string aliveEndpoint,string deviceDataEndpoint,int code)
        {
            if (aliveEndpoint == null)
                throw new ArgumentNullException("Alive endpoint ne sme biti null");

            if (deviceDataEndpoint == null)
                throw new ArgumentNullException("Data endpoint ne sme biti null");

            aliveEndpoint = aliveEndpoint.Trim();
            if (aliveEndpoint == string.Empty)
                throw new ArgumentException("Alive endpoint ne sme biti prazan string");

            deviceDataEndpoint = deviceDataEndpoint.Trim();
            if (deviceDataEndpoint == string.Empty)
                throw new ArgumentException("Device data endpoint ne sme biti prazan string");

            if (code == -1)
            {
                int hashcode;
                do
                {
                    hashcode = Hasher.GetDeviceCode();

                } while (deviceTableHandle.Exists(hashcode));

                deviceTableHandle.Insert(new AMSDatabaseAccess.Device() { Id = hashcode, WakeUpTime = TimeKeep.GetUnixTimestamp(), Type = (int)DeviceType.Controller, UpTime = 0, NumberOfChanges = 0 });
                this.RegisteredControllers[hashcode] = new Tuple<string, string>(aliveEndpoint, deviceDataEndpoint);

                this.UIModel.AddDevice(hashcode, 0, DeviceType.Controller);

                return hashcode;
            }
            else
            {
                this.RegisteredControllers[code] = new Tuple<string, string>(aliveEndpoint, deviceDataEndpoint);
                return code;
            }
        }
    }
}
