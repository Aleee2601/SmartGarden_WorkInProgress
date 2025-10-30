namespace SmartGarden.App.Helpers
{
    public static class SecureStorageHelper
    {
        private const string TokenKey = "auth_token";
        private const string UserIdKey = "user_id";
        private const string UserEmailKey = "user_email";

        public static async Task<string?> GetTokenAsync()
        {
            try
            {
                return await SecureStorage.Default.GetAsync(TokenKey);
            }
            catch
            {
                return null;
            }
        }

        public static async Task SetTokenAsync(string token)
        {
            try
            {
                await SecureStorage.Default.SetAsync(TokenKey, token);
            }
            catch
            {
                // Handle error
            }
        }

        public static async Task<int?> GetUserIdAsync()
        {
            try
            {
                var userIdStr = await SecureStorage.Default.GetAsync(UserIdKey);
                if (int.TryParse(userIdStr, out int userId))
                    return userId;
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static async Task SetUserIdAsync(int userId)
        {
            try
            {
                await SecureStorage.Default.SetAsync(UserIdKey, userId.ToString());
            }
            catch
            {
                // Handle error
            }
        }

        public static async Task<string?> GetUserEmailAsync()
        {
            try
            {
                return await SecureStorage.Default.GetAsync(UserEmailKey);
            }
            catch
            {
                return null;
            }
        }

        public static async Task SetUserEmailAsync(string email)
        {
            try
            {
                await SecureStorage.Default.SetAsync(UserEmailKey, email);
            }
            catch
            {
                // Handle error
            }
        }

        public static async Task ClearAllAsync()
        {
            try
            {
                SecureStorage.Default.RemoveAll();
            }
            catch
            {
                // Handle error
            }
        }
    }
}
