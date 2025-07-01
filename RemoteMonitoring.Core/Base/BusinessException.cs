using System.ComponentModel;
using System.Reflection;

namespace RemoteMonitoring.Core.Base;

public class BusinessException : Exception
{
    public BusinessException(string msg,
        BusinessExceptionTypeEnum exceptionType = BusinessExceptionTypeEnum.NotSpecified,
        string? exceptionName = "") : base(
        GetFullExceptionMessage(msg))
    {
        Type = exceptionType;
        Name += exceptionName;
    }

    public BusinessException(IEnumerable<string> msg,
        BusinessExceptionTypeEnum exceptionType = BusinessExceptionTypeEnum.NotSpecified,
        string? exceptionName = "") : base(
        GetFullExceptionMessage(msg.ToArray()))
    {
        Type = exceptionType;
        Name += exceptionName;
    }

    public BusinessExceptionTypeEnum Type { get; set; }

    public string? Name { get; private set; } = nameof(BusinessException);

    public string TypeName => GetTypeName(Type);


    private static string GetTypeName(BusinessExceptionTypeEnum type)
    {
        var businessExceptionTypeStateType = typeof(BusinessExceptionTypeEnum);
        var businessExceptionTypeStateTypeField = businessExceptionTypeStateType.GetField(type.ToString())!;
        var descriptionAttr = businessExceptionTypeStateTypeField.GetCustomAttribute(typeof(DescriptionAttribute));
        if (descriptionAttr is DescriptionAttribute description) return description.Description;
        return type.ToString();
    }

    private static string GetFullExceptionMessage(params string[] msg)
    {
        return string.Join(",", msg);
    }
}

public enum BusinessExceptionTypeEnum
{
    [Description(nameof(BusinessException))]
    NotSpecified,

    /// <summary>
    ///     參數有誤
    /// </summary>
    [Description("參數有誤")] ParameterError,

    /// <summary>
    ///     程序配置異常
    /// </summary>
    [Description("程序配置異常")] ConfigurationError,

    /// <summary>
    ///     身份驗證不通過
    /// </summary>
    [Description("身份驗證不通過")] UnauthorizedIdentity,

    /// <summary>
    ///     程序不兼容性
    /// </summary>
    [Description("程序不兼容性")] Incompatible,

    /// <summary>
    ///     數據不存在
    /// </summary>
    [Description("數據不存在")] DataNotExists,

    /// <summary>
    ///     數據重複
    /// </summary>
    [Description("數據重複")] DataDuplication,

    /// <summary>
    ///     權限不足
    /// </summary>
    [Description("權限不足")] Forbidden,

    /// <summary>
    ///     數據狀態不允許
    /// </summary>
    [Description("數據狀態不允許")] DataStatusNotAllow,

    /// <summary>
    ///     操作已過時
    /// </summary>
    [Description("操作過時")] OperationExpired,

    /// <summary>
    ///     操作太頻繁
    /// </summary>
    [Description("操作太頻繁")] FrequentOperation,
}