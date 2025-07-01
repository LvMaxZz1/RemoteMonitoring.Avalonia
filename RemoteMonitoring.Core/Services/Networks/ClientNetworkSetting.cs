using System.ComponentModel;
using RemoteMonitoring.Core.Base;

namespace RemoteMonitoring.Core.Services.Networks;

public class ClientNetworkSetting : IJsonFileSetting
{
    public string JsonFilePath => "./Services/Networks/client-network-setting.json";

    public string IpAddress { get; set; } = string.Empty;
    
    public int Port { get; set; }

    [Description("是否优先使用直接内存")]
    public bool PreferDirect { get; set; }

    [Description("堆内存池最大持有内存块数量")]
    public int NHeapArena { get; set; }

    [Description("直接内存池最大持有内存块数量")]
    public int NDirectArena { get; set; }

    [Description("内存页")]
    public int PageSize { get; set; }

    [Description("小缓存大小(增大会增加碎片内存风险)")]
    public int TinyCacheSize { get; set; }
    
    [Description("PoolChunk的最大阶数（控制单个Chunk的总大小 计算方式 PageSize * 2^maxOrder = 8KB * 2048 = 16MB）")]
    public int MaxOrder { get; set; }
    
    [Description("小型缓存队列容量（加速 8KB 以下分配）")]
    public int SmallCacheSize { get; set; }
    
    [Description("常规缓存队列容量（优化 8KB-16MB 分配）")]
    public int NormalCacheSize { get; set; }
}