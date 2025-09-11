using System.Web;

namespace Client_model.Helpers
{
    // Lightweight helper to read the current logged-in client id from Session.
    // Use int? to safely represent "no client logged in".
    public static class ClientContext
    {
        private const string SessionKeyClientId = "ClientID";
        public static int? CurrentClientId
        {
            get
            {
                var ctx = HttpContext.Current;
                if (ctx?.Session == null) return null;
                var v = ctx.Session[SessionKeyClientId];
                if (v == null) return null;
                if (v is int) return (int)v;
                int parsed;
                return int.TryParse(v.ToString(), out parsed) ? parsed : (int?)null;
            }
        }

        public static object CurrentClient { get; internal set; }

    }
}
