
namespace Common.ControllerDataModel.DataModelInterfaces
{
    public interface IMakeControllerData
    {
        ControllerData MakeControllerData(IController controller, IGetUnixTimestamp TimeKeep);
    }
}
