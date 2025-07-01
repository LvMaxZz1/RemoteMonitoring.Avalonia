using System.ComponentModel;
using System.Security.Cryptography;
using RemoteMonitoring.Core.Base;

namespace RemoteMonitoring.Core.Utils;

public class AesEncryption
{
    [Description("加密方法")]
    public static (string encryptedPassword, string key, string iv) Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException(nameof(plainText));

        using var aesAlg = Aes.Create();
        aesAlg.GenerateKey();
        aesAlg.GenerateIV();

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using var swEncrypt = new StreamWriter(csEncrypt);

        swEncrypt.Write(plainText);
        swEncrypt.Flush();
        csEncrypt.FlushFinalBlock();

        return (
            Convert.ToBase64String(msEncrypt.ToArray()),
            Convert.ToBase64String(aesAlg.Key),
            Convert.ToBase64String(aesAlg.IV)
        );
    }

    [Description("解密方法")] 
    public static string Decrypt(string encryptedPassword, string base64Key, string base64Iv)
    {
        if (string.IsNullOrEmpty(encryptedPassword))
            throw new BusinessException(nameof(encryptedPassword) + " is null", BusinessExceptionTypeEnum.DataStatusNotAllow);
        
        if (string.IsNullOrEmpty(base64Key))
            throw new BusinessException(nameof(base64Key)+ " is null", BusinessExceptionTypeEnum.DataStatusNotAllow);
        
        if (string.IsNullOrEmpty(base64Iv))
            throw new BusinessException(nameof(base64Iv)+ " is null", BusinessExceptionTypeEnum.DataStatusNotAllow);
        try
        {
            var key = Convert.FromBase64String(base64Key);
            var iv = Convert.FromBase64String(base64Iv);
            var cipherText = Convert.FromBase64String(encryptedPassword);

            using var aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            using var decrypt = aesAlg.CreateDecryptor(key, iv);
            using var msDecrypt = new MemoryStream(cipherText);
            using var csDecrypt = new CryptoStream(msDecrypt, decrypt, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
        catch
        {
            return encryptedPassword;
        }
    }
}