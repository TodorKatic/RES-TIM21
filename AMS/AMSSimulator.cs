using AMS.Communication;
using AMS.HashCodeUtil;
using AMSDatabaseAccess.DatabaseCRUD;
using Common;
using Common.ControllerDataModel;
using Common.HashCodeUtils;
using System;
using System.Collections.Concurrent;
using System.Configuration;

namespace AMS
{
    public class AMSSimulator
    {

        private string AMSControllerDataEndpoint;
        private string AMSDeviceDataEndpoint;
        private string AMSDeviceHashCodeEndpoint;
        private string AMSControllerHashCodeEndpoint;
        private string AMSEndpointsGetterServiceEndpoint;
        private string CharSet;
        private int DeviceCodeLength;


        private ServiceHost<EndpointsGetterService, IGetEndpoints> EndpointGetterHost;
        private ServiceHost<DeviceDataReceiverService, ISendDeviceData> DeviceDataReceiverHost;
        private ServiceHost<ControllerDataReceiverService, ISendControllerData> ControllerDataReceiverHost;
        private ServiceHost<DeviceHashCodeService, IHasherDevice> DeviceRegistrator;
        private ServiceHost<ControllerHashCodeService, IHasherController> ControllerRegistrator;

        private EndpointsGetterService _endpointServiceInstance;
        private DeviceDataReceiverService _deviceDataServiceInstance;
        private DeviceHashCodeService _deviceHashCodeServiceInstance;

        private ControllerDataReceiverService _controllerDataServiceInstance;
        private ControllerHashCodeService _controllerHashCodeServiceInstance;

        private IChannelFactory<ICheckAlive> AliveCheckerChannelFactory;

        //item1 je usluga na kojoj se proverava da li je endpoint ziv
        //item2 je endpoint koji se salje uredjaima pri konigurisanju
        private ConcurrentDictionary<int, Tuple<string, string>> AliveEndpoints;


        public ICrudDaoDevice deviceTableHandle { get; private set; }
        public ICrudDaoDeviceData dataTableHandle { get; private set; }
        private IGetUnixTimestamp TimeKeep;

        public AMSSimulator(ICrudDaoDevice deviceTableHandle, ICrudDaoDeviceData dataTableHandle,IMainWindowModel model)
        {
            if (model == null)
                throw new ArgumentNullException("Prosledjeni model ne sme biti null");

            if (deviceTableHandle == null)
                throw new ArgumentNullException("Handle objekta koji radi sa tabelom uredjaja ne sme biti null");

            if (dataTableHandle == null)
                throw new ArgumentNullException("Handle objekta koji radi sa tabelom podataka ne sme biti null");



            this.AliveCheckerChannelFactory = new ChannelFactory<ICheckAlive>();
            this.TimeKeep = new TimeKeeper();

            this.deviceTableHandle = deviceTableHandle;
            this.dataTableHandle = dataTableHandle;

            this.AliveEndpoints = new ConcurrentDictionary<int, Tuple<string, string>>();

            this.AMSDeviceDataEndpoint = ConfigurationManager.AppSettings["DeviceDataAMSService"];
            this.AMSControllerDataEndpoint = ConfigurationManager.AppSettings["ControllerDataAMSService"];
            this.AMSDeviceHashCodeEndpoint = ConfigurationManager.AppSettings["HashCodeAMSServiceDevice"];
            this.AMSControllerHashCodeEndpoint = ConfigurationManager.AppSettings["HashCodeAMSServiceController"];
            this.AMSEndpointsGetterServiceEndpoint = ConfigurationManager.AppSettings["EndpointsGetterAMSService"];
            this.CharSet = ConfigurationManager.AppSettings["Chars"];
            this.DeviceCodeLength = int.Parse(ConfigurationManager.AppSettings["DeviceCodeLength"]);

            this._controllerDataServiceInstance = new ControllerDataReceiverService(this.dataTableHandle, this.deviceTableHandle,
                uint.Parse(ConfigurationManager.AppSettings["AppToRealSecondRation"]),model);
            this._deviceHashCodeServiceInstance = new DeviceHashCodeService(this.deviceTableHandle,TimeKeep,model,new Hasher(this.DeviceCodeLength,this.CharSet));
            this._endpointServiceInstance = new EndpointsGetterService(this.AliveEndpoints, this.AMSDeviceDataEndpoint,this.AliveCheckerChannelFactory);
            this._controllerHashCodeServiceInstance = new ControllerHashCodeService(this.deviceTableHandle, this.AliveEndpoints,TimeKeep,model,new Hasher(this.DeviceCodeLength,this.CharSet));
            this._deviceDataServiceInstance = new DeviceDataReceiverService(this.dataTableHandle, this.deviceTableHandle,model, uint.Parse(ConfigurationManager.AppSettings["AppToRealSecondRation"]));


            this.ControllerRegistrator = new ServiceHost<ControllerHashCodeService, IHasherController>(this.AMSControllerHashCodeEndpoint, _controllerHashCodeServiceInstance);
            this.DeviceRegistrator = new ServiceHost<DeviceHashCodeService, IHasherDevice>(this.AMSDeviceHashCodeEndpoint, _deviceHashCodeServiceInstance);
            this.EndpointGetterHost = new ServiceHost<EndpointsGetterService, IGetEndpoints>(this.AMSEndpointsGetterServiceEndpoint, _endpointServiceInstance);
            this.ControllerDataReceiverHost = new ServiceHost<ControllerDataReceiverService, ISendControllerData>(this.AMSControllerDataEndpoint, _controllerDataServiceInstance);
            this.DeviceDataReceiverHost = new ServiceHost<DeviceDataReceiverService, ISendDeviceData>(this.AMSDeviceDataEndpoint, _deviceDataServiceInstance);

            this.ControllerRegistrator.Open();
            this.DeviceRegistrator.Open();
            this.EndpointGetterHost.Open();
            this.ControllerDataReceiverHost.Open();
            this.DeviceDataReceiverHost.Open();
        }
    }
}
