namespace UchaSe
{
    public class HelperMethods
    {
        public string GenerateRandomEmail()
        {
            var random = new Random();
            string randomEmail = $"{GenerateRandomUser()}@abv.bg";
            return randomEmail;
        }

        public string GenerateRandomMobilePhone()
        {
            var random = new Random();
            string phoneNumber = $"888{random.Next(100000, 999999)}";
            return phoneNumber;
        }
        
        public string GenerateRandomUser()
        {
            var random = new Random();
            string randomUser = $"user{random.Next(1, 9999)}";
            return randomUser;
        }

        public string GenerateRandomPassword()
        {
            var random = new Random();
            string password = $"password{random.Next(100,999)}";
            return password;
        }
    }
}
