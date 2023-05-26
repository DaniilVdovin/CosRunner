using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace Assets.Scripts.GoogleServices
{
    public class GoogleClientConfig
    {
        private static bool Authenticated;
        GoogleClientConfig()
        {
          
        }

        public static void Auth()
        {
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }
        internal static void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                // Continue with Play Games Services
                Authenticated = true;
            }
            else
            {
                // Disable your integration with Play Games Services or show a login button
                // to ask users to sign-in. Clicking it should call
                Authenticated = false;
                PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
                
            }
        }
    }

}
