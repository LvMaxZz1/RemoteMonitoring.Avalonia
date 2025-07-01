namespace RemoteMonitoring.Core.Services.Networks.Base.Enums;

public enum MachineLinkType : byte
{
    Client = 0x01,
    
    Console = 0x02,
    
    Server = 0x03
}

public static class MachineLinkTypeExtensions
{
    public static bool TryGetMachineLinkType(this byte bt, out MachineLinkType? machineLinkType)
    {
        // 将 byte 转换为 int
        int machineLinkTypeInt = bt;
        
        if (Enum.IsDefined(typeof(MachineLinkType), machineLinkTypeInt))
        {
            machineLinkType = (MachineLinkType)machineLinkTypeInt;
            return true;
        }
        
        machineLinkType = null;
        return false;

    }
}