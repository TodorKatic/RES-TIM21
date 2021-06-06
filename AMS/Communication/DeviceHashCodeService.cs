using Common;
using System.ServiceModel;
using AMSDatabaseAccess.DatabaseCRUD;
using Common.HashCodeUtils;
using AMS.HashCodeUtil;
using System.Collections.Concurrent;
using System;

namespace AMS.Communication
{

    /// <summary>
    /// Usluga koja sluzi za registrovanje uredjaja i kontrolera u sistemu odnosno daje im jedinstvena imena i upisuje ih u bazu kada je pronadjeno jedinstveno ime
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    public class DeviceHashCodeService:IHasherDevice
    {

        public ICrudDaoDevice deviceTableHandle { get; private set; }
        public IHasher Hasher { get; private set; }
        public IGetUnixTimestamp TimeKeep { get; private set; }
        public IMainWindowModel UIModel { get; private set; }

        public DeviceHashCodeService(ICrudDaoDevice deviceTableHandle,IGetUnixTimestamp TimeKeep,IMainWindowModel model,IHasher hasher)
        {
            if (deviceTableHandle == null)
                throw new ArgumentNullException("Handle objekta koji radi sa tabelom uredjaja ne sme biti null");

            if (TimeKeep == null)
                throw new ArgumentNullException("Objekat klase TimeKeeper ne sme biti null");

            if (model == null)
                throw new ArgumentNullException("Proesledjen model ne sme biti null");
            if (hasher == null)
                throw new ArgumentNullException("Hasher ne sme biti null");


            this.deviceTableHandle = deviceTableHandle;
            this.Hasher = hasher;
            this.TimeKeep = TimeKeep;
            this.UIModel = model;
        }

        /// <summary>
        /// Metoda koja registruje uredjaj odnosno pravi upisuje red u tabeli uredjaja
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetDeviceCode(DeviceType type)
        {
            int code;
            do
            {
                code = Hasher.GetDeviceCode();

            } while (deviceTableHandle.Exists(code));

            deviceTableHandle.Insert(new AMSDatabaseAccess.Device() { Id = code, WakeUpTime = TimeKeep.GetUnixTimestamp(), Type = (int)type, UpTime = 0, NumberOfChanges = 0 });
            this.UIModel.AddDevice(code, 0, type);
            return code;
        }
    }
}
