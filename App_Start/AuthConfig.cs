using Microsoft.Web.WebPages.OAuth;
using VKAuth;

namespace Informer.App_Start
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // следует обновить сайт. Дополнительные сведения: http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");
            OAuthWebSecurity.RegisterClient(
                new VkontakteClient(), "Вконтакте", null
            );


            OAuthWebSecurity.RegisterFacebookClient(
                appId: "341764512535547",
                appSecret: "f436e1078f6a1fdb2a45c439f626c47f");

            OAuthWebSecurity.RegisterGoogleClient();


        }
    }
}
