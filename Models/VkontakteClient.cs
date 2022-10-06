using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using DotNetOpenAuth.AspNet;
using AOAuthNET;
using System.Web.Script.Serialization;

namespace VKAuth
{
    public class VkontakteClient : IAuthenticationClient
    {
        public string AppId;
        public string AppSecret;
        public string VkAuthUrl;
        public string VKtoken;

        public VkontakteClient()
        {
            AppId = "3688628";
            AppSecret = "uGEexgupF7Phx38tPGbX";
            VkAuthUrl = "http://api.vkontakte.ru/oauth/authorize";
            VKtoken = "https://api.vkontakte.ru/oauth/access_token";
        }
        string IAuthenticationClient.ProviderName
        {
            get { return "vkontakte"; }
        }

        void IAuthenticationClient.RequestAuthentication(HttpContextBase context, Uri returnUrl)
        {
            var url = returnUrl.ToString();
            var vk = new OAuth2(AppId, AppSecret, VkAuthUrl, VKtoken, url);
            vk.GetAuthCode(new Dictionary<string, string> { { "display", "popup" } });
        }

/*
        private Dictionary<string, object> DeserializeToDictionary(string jo)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(jo);
            var values2 = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> d in values)
            {
                values2.Add(d.Key,
                            d.Value.GetType().FullName.Contains("Newtonsoft.Json.Linq.JObject")
                                ? DeserializeToDictionary(d.Value.ToString())
                                : d.Value);
            }
            return values2;
        }
*/

        AuthenticationResult IAuthenticationClient.VerifyAuthentication(HttpContextBase context)
        {
            var code = context.Request["code"];
            if (context.Request.Url != null)
            {
                var returnUrl = context.Request.Url.AbsoluteUri;
                returnUrl = returnUrl.Substring(0, returnUrl.LastIndexOf("&code=", StringComparison.Ordinal));

                var res = new AuthenticationResult(false);

                if (!string.IsNullOrEmpty(code))
                {
                    var vk = new OAuth2(AppId, AppSecret, VkAuthUrl, VKtoken, returnUrl) {Code = code};
                    var token = vk.GetAccessToken(new Dictionary<string, string> { { "client_secret", AppSecret } }, OAuth2.AccessTokenType.JsonDictionary);
                    if (token != null)
                    {
                        if (token.dictionary_token != null)
                        {
                            string result = OAuth2UserData.GetVKUserData(token.dictionary_token["access_token"], new Dictionary<string, string> { { "uid", token.dictionary_token["user_id"] } });
                            var jss = new JavaScriptSerializer();
                            var table = jss.Deserialize<dynamic>(result);
                            string uid = ((int)table["response"][0]["uid"]).ToString(CultureInfo.InvariantCulture);
                            string uname = (string)table["response"][0]["first_name"] + " " + (string)table["response"][0]["last_name"];
                            res = new AuthenticationResult(true, "vkontakte", uid, uname, null);
                        }
                    }
                }

                return res;
            }
            return null;
        }
    }
}