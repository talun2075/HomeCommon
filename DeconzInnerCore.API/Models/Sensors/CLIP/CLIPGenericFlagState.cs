namespace InnerCore.Api.DeConz.Models.Sensors.CLIP
{
    public interface ICLIPGenericFlagState : IGeneralSensorState
    {
        bool? Flag { get; set; }
    }
}