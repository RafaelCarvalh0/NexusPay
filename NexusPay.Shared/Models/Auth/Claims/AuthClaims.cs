using System.Data;

namespace NexusPay.Shared.Models.Auth.Claims
{
    public class AuthClaims
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Role { get; private set; }

        public static AuthClaims FromDataRow(DataRow row)
        {
            return new AuthClaims
            {
                Id = Guid.Parse(row["ID"].ToString()!),
                Name = row["NAME"].ToString()!,
                Email = row["EMAIL"].ToString()!,
                Role = row["ROLE"].ToString()!
            };
        }
    }
}
