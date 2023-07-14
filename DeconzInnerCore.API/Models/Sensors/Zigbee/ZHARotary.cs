using InnerCore.Api.DeConz.Models.Sensors;

namespace DeconzInnerCore.API.Models.Sensors.Zigbee
{
    public interface ZHARotary: IGeneralSensorState
    {
        int? Rotaryevent { get; set; }
        int? Expectedrotation { get; set; }
        int? Expectedeventduration { get; set; }
    }
}
