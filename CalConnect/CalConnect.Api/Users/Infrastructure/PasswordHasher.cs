using System.Security.Cryptography;

namespace CalConnect.Api.Users.Infrastructure;

public class PasswordHasher
{
    private const int SaltSize = 16; // salt 的大小（以字节为单位）
    private const int HashSize = 32; // hash 的大小（以字节为单位）
    private const int Iterations = 10000; // 哈希算法的迭代次数

    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;
    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        string[] parts = passwordHash.Split("-");
        byte[] hash = Convert.FromHexString(parts[0]);
        byte[] salt = Convert.FromHexString(parts[1]);

        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
        
        //return inputHash.SequenceEqual(hash);
        return CryptographicOperations.FixedTimeEquals(hash, inputHash);
    }
}