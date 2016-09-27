
namespace CAF.IM.Services
{
    public interface ICryptoService
    {
        byte[] Protect(byte[] plainText);
        byte[] Unprotect(byte[] payload);
        string CreateSalt();
        string CreateToken(string value);
        string GetValueFromToken(string token);
    }
}