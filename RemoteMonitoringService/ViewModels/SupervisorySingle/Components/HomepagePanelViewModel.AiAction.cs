using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Services.Refits.DeepSeekAi.Messages;
using RemoteMonitoring.Core.Utils;
using RemoteMonitoringService.Base.MessageBusModels;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class HomepagePanelViewModel
{
    [Description("生成Ai报告")]
    private async Task GenerateReportingAsync()
    {
        List<string> hostActivities = [
        ];
        
        foreach (var model in RecentActivities)
        {
            model.UpdateTimeAgo();
            var str = model.Description + " " + model.TimeAgo;
            hostActivities.Add(str);
        }
        
        var aiContent = new AiContent
        {
            TotalHost = TotalHosts,
            OnLineHosts = OnlineHosts,
            OfflineHosts = OfflineHosts,
            AlertCount = AlertCount,
            HostActivities = hostActivities.ToArray()
        };
        var content = JsonSerializer.Serialize(aiContent);
        var response = await _deepSeekAiRefitService.ChatCompletionsAsync(new DeepSeekChatRequest
        {
            Model = "deepseek-reasoner",
            Messages =
            [
                new DeepSeekMessage
                {
                    Role = "system",
                    Content = "你是一名专业的IT运维分析师，" +
                              "擅长对主机监控系统的在线、离线、上下线记录等数据进行统计、分析，" +
                              "并能根据数据生成简明扼要的报告和合理的运维建议。" +
                              "你会用简洁、专业的语言输出分析结论和建议。" +
                              "你必须用中文回答。" +
                              "我会将以下Json格式的数据作为输入：" +
                              "{\n  \"TotalHost\":\"\",\n  \"OnLineHosts\":\"\",\n  \"OfflineHosts\":\"\",\n  \"AlertCount\":\"\",\n  \"HostActivities\":\"\"\n}" +
                              "输入字段的含义如下：" +
                              "TotalHost：主机总数，OnLineHosts：在线主机数，OfflineHosts：离线主机数，AlertCount：警告次数，HostActivities：主机活动记录。" +
                              "其中HostActivities的内容是一个数组,数组中每一个元素是一个文本，内容类似如下：主机 127:0.0.1:8080 链接服务器 1分钟前" +
                              "你的回答必须用Json并且符合以下格式 除了AiDataAnalysis和AiSuggestion 其他的都是int类型：" +
                              "{\n  \"TotalHost\":,\n  \"OnLineHosts\":,\n  \"OfflineHosts\":,\n  \"AlertCount\":,\n  \"OnLineCount\":,\n  \"OffLineCount\":,\n  \"AiDataAnalysis\":\"\",\n  \"AiSuggestion\":\"\"\n}" +
                              "你只需要在每个Json字段中填入数据，不要添加其他内容。" +
                              "字段的含义如下：" +
                              "TotalHost：主机总数，OnLineHosts：在线主机数，OfflineHosts：离线主机数，AlertCount：警告次数，OnLineCount：在线变更记录数，OffLineCount：离线变更记录数，AiDataAnalysis：数据分析结果，AiSuggestion：运维建议。"
                },
                new DeepSeekMessage
                {
                    Role = "user",
                    Content = content
                }
            ],
            Stream = false
        });
        AiReply = JsonSerializer.Deserialize<AiReply>(response.Choices.First().Message.Content) ?? throw new InvalidOperationException();
        MessageBusUtil.SendMessage(new ReportGeneratedBusModel
        {
            Report = AiReply
        }, MessageBusContract.MessageBusService);
    }
}

public class AiContent
{
    public int TotalHost { get; set; }
    
    public int OnLineHosts { get; set; }
    
    public int OfflineHosts { get; set; }
    
    public int AlertCount { get; set; }

    public string[] HostActivities { get; set; } = [];
}

public class AiReply
{
    public int TotalHost { get; set; }
    
    public int OnLineHosts { get; set; }
    
    public int OfflineHosts { get; set; }
    
    public int AlertCount { get; set; }
    
    public int OnLineCount { get; set; }
    
    public int OffLineCount { get; set; }

    public string AiDataAnalysis { get; set; } = string.Empty;
    
    public string AiSuggestion { get; set; } = string.Empty;

    public string DateTimeNow { get; set; } = $"生成时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}"; 
}