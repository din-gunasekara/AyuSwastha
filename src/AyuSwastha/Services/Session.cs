using AyuSwastha.Models;

namespace AyuSwastha.Services
{
    /// <summary>Holds the currently signed-in user for the lifetime of the app.</summary>
    public static class Session
    {
        public static User CurrentUser { get; set; }

        public static bool IsAuthenticated => CurrentUser != null;

        public static void Clear() => CurrentUser = null;
    }
}
