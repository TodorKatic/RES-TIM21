using System.Configuration;
using System.Windows;
using AMS.DataAdapters;
using AMSDatabaseAccess;
using AMSDatabaseAccess.DatabaseCRUD;
using AMSDatabaseAccess.DatabaseCRUD.CRUDImplementation;
using Xceed.Wpf.AvalonDock.Controls;
namespace AMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private AMSSimulator simulator;
        private BaseToViewDataAdapter DataAdapter;
        private AMSDatabaseEntities database;

        private ICrudDaoDevice deviceTableHandle;
        private ICrudDaoDeviceData dataTableHandle;
        private IMainWindowModel MainUIModel;
        private IChartWindowModel ChartUIModel;

        private ChartWindow ChartingWindow;

        public MainWindow()
        {

            this.database = new AMSDatabaseEntities();
            this.deviceTableHandle = new DeviceCRUD(database);
            this.dataTableHandle = new DevicesDataCRUD(database);
            this.DataAdapter = new BaseToViewDataAdapter(this.dataTableHandle, this.deviceTableHandle,uint.Parse(ConfigurationManager.AppSettings["AnalogChangesNumber"])
                , uint.Parse(ConfigurationManager.AppSettings["DigitalChangesNumber"]), uint.Parse(ConfigurationManager.AppSettings["WorkingHoursDigital"]), 
                uint.Parse(ConfigurationManager.AppSettings["WorkingHoursAnalog"]), uint.Parse(ConfigurationManager.AppSettings["WorkingHoursController"]),
                uint.Parse(ConfigurationManager.AppSettings["AppToRealSecondRation"]));

            this.MainUIModel = new Model(this.DataAdapter);

            this.DataContext = MainUIModel;
            this.MainUIModel.GetAllDevices();
            simulator = new AMSSimulator(deviceTableHandle, dataTableHandle,MainUIModel);

            InitializeComponent();
        }

        private void ShowChartBtn_Click(object sender, RoutedEventArgs e)
        {
            if(MainUIModel.Validate())
            {

                ChartUIModel = new ChartWindowModel();

                long lowerTimestamp = this.DataAdapter.ConvertToUnixTimestamp(MainUIModel.StartDate);
                long higherTimestamp = this.DataAdapter.ConvertToUnixTimestamp(MainUIModel.EndDate);

                int key = MainUIModel.Selected.DeviceCode;
                float min;
                float max;
                float diff;
                float mean;

                this.DataAdapter.GetGraphDetails(key, lowerTimestamp, higherTimestamp, out max, out min, out mean, out diff);

                this.ChartUIModel.DeviceCode = key;
                this.ChartUIModel.Difference = diff;
                this.ChartUIModel.MaxValue = max;
                this.ChartUIModel.MinValue = min;
                this.ChartUIModel.MeanValue = mean;
                this.ChartUIModel.DeviceData = this.DataAdapter.GetGraphData(key, lowerTimestamp, higherTimestamp);

                this.ChartingWindow = new ChartWindow(this.ChartUIModel);

                MainUIModel.Reset();
                MainUIModel.Selected = null;

                this.ChartingWindow.Show();
            }
        }
    }
}
