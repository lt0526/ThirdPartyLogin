using LoginThird.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LoginThird
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        const string qqAppId = "1106166424";//verifying 1106122016
        const string qqAppSecret = "BzDGR8gdp6WdDNmn"; //verifying CiUIacxuV0sZiOcV

        public MainPage()
        {
            this.InitializeComponent();
            
        }

        private async void WeiboBtn_Click(object sender, RoutedEventArgs e)
        {
            string authenticationURL = string.Format("https://api.weibo.com/oauth2/authorize?client_id=2588421916&response_type=code&redirect_uri=https://www.baidu.com");
            try
            {
                WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(authenticationURL), new Uri("https://www.baidu.com"));

                if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var responseData = webAuthenticationResult.ResponseData;
                    var codeStr = ParseAuthorizationResponse(responseData);

                    if (codeStr.Length > 0)
                    {
                        await GetWeibosync(codeStr);
                        
                    } 
                    else
                    {
                        string outputStr = "Cancel Error returned by Weibo : " + webAuthenticationResult.ResponseErrorDetail.ToString();
                        MessageDialog message = new MessageDialog(outputStr);
                        await message.ShowAsync();
                    }
                }
                else if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    string outputStr = "HTTP Error returned by Weibo : " + webAuthenticationResult.ResponseErrorDetail.ToString();
                    MessageDialog message = new MessageDialog(outputStr);
                    await message.ShowAsync();
                }
                else
                {
                    string outputStr = "Error returned by Weibo : " + webAuthenticationResult.ResponseErrorDetail.ToString();
                    MessageDialog message = new MessageDialog(outputStr);
                    await message.ShowAsync();
                }
            } catch (Exception ex)
            {
                MessageDialog message = new MessageDialog(ex.Message);
                await message.ShowAsync();
            }
            
        }

        private async Task GetWeibosync(string webAuthResultResponseData)
        {

            MyProgressBar.IsActive = true;
            //Get Access Token first
            try
            {
                string tokenStr = string.Format("https://api.weibo.com/oauth2/access_token?client_id=2588421916&client_secret=5ed7510cee73af034d6e51bbb6854ef1&grant_type=authorization_code&redirect_uri=https://www.baidu.com&code={0}", webAuthResultResponseData);

                Dictionary<string, string> pairs = new Dictionary<string, string>();
                var formContent = new HttpFormUrlEncodedContent(pairs);

                var httpFilter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
                httpFilter.CacheControl.ReadBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.MostRecent;
                var client = new Windows.Web.Http.HttpClient(httpFilter);

                Windows.Web.Http.HttpResponseMessage response = await client.PostAsync(new Uri(tokenStr), formContent);
                client.Dispose();
                //{ { "access_token":"2.00HuTKYCamkKpC16a40cd5b9JDIAfB","remind_in":"157679999","expires_in":157679999,"uid":"2337159323"} }

                string responseBody = await response.Content.ReadAsStringAsync();
                var accessToken = JsonConvert.DeserializeObject<AccessTokenModel>(responseBody);

                //get user's detail info
                await getPersonalInfo(accessToken);
            }
            catch (Exception ex)
            {
                MyProgressBar.IsActive = false;
            }
        }


        private string ParseAuthorizationResponse(string responseData)
        {
            if (responseData.Contains("?code="))
            {
                var authorizationCodeIndex = responseData.IndexOf("?code=", StringComparison.Ordinal) + 6;
                return responseData.Substring(authorizationCodeIndex, responseData.Length - authorizationCodeIndex);
            }
            return "";
        }

        private async Task getPersonalInfo(AccessTokenModel tokenModel)
        {
            var httpClient = new Windows.Web.Http.HttpClient();

            string infoStr = string.Format("https://api.weibo.com/2/users/show.json?access_token={0}&uid={1}", tokenModel.access_token, tokenModel.uid);
            var response = await httpClient.GetAsync(new Uri(infoStr));
            string responseBody = await response.Content.ReadAsStringAsync();
            UserInfoModel infoModel = JsonConvert.DeserializeObject<UserInfoModel>(responseBody);

            NickName.Text = infoModel.name;
            Id.Text = infoModel.idstr;
            Location.Text = infoModel.location;

            MyProgressBar.IsActive = false;
        }

        private async void QQBtn_Click(object sender, RoutedEventArgs e)
        {
            string authenticationURL = string.Format("https://graph.qq.com/oauth/show?which=Login&display=pc&response_type=code&client_id={0}&redirect_uri=https://www.baidu.com&state=test", qqAppId);
            //string authenticationURL = string.Format("https://graph.qq.com/oauth2.0/authorize?response_type=token&client_id=1106122016&redirect_uri=https://www.baidu.com&scope=test");
            try
            {
                WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(authenticationURL), new Uri("https://www.baidu.com"));

                if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var responseData = webAuthenticationResult.ResponseData;
                    var codeStr = ParseAuthorizationResponse(responseData);

                    if (codeStr.Length > 0)
                    {
                        await getQQAsync(codeStr);

                    }
                    else
                    {
                        string outputStr = "Cancel Error returned by QQ : " + webAuthenticationResult.ResponseErrorDetail.ToString();
                        MessageDialog message = new MessageDialog(outputStr);
                        await message.ShowAsync();
                    }
                }
                else if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    string outputStr = "HTTP Error returned by QQ : " + webAuthenticationResult.ResponseErrorDetail.ToString();
                    MessageDialog message = new MessageDialog(outputStr);
                    await message.ShowAsync();
                }
                else
                {
                    string outputStr = "Error returned by QQ : " + webAuthenticationResult.ResponseErrorDetail.ToString();
                    MessageDialog message = new MessageDialog(outputStr);
                    await message.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                MessageDialog message = new MessageDialog(ex.Message);
                await message.ShowAsync();
            }
        }

        private async Task getQQAsync(string codeStr)
        {
            MyProgressBar.IsActive = true;
            //Get Access Token first
            try
            {

                string authenticCode = codeStr.Split('&')[0];
                string tokenStr = string.Format("https://graph.qq.com/oauth2.0/token?grant_type=authorization_code&client_id={0}&client_secret={1}&code={2}&redirect_uri=https://www.baidu.com", qqAppId, qqAppSecret, authenticCode);

                var httpFilter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
                httpFilter.CacheControl.ReadBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.MostRecent;
                var client = new Windows.Web.Http.HttpClient(httpFilter);

                Windows.Web.Http.HttpResponseMessage response = await client.GetAsync(new Uri(tokenStr));
                client.Dispose();
                //{ { "access_token":"2.00HuTKYCamkKpC16a40cd5b9JDIAfB","remind_in":"157679999","expires_in":157679999,"uid":"2337159323"} }

                string responseBody = await response.Content.ReadAsStringAsync();
                string accessTokenStr = responseBody.Split('&')[0];

                //get user's detail info
                await getQQPersonalInfo(accessTokenStr.Split('=')[1]);
            }
            catch (Exception ex)
            {
                MyProgressBar.IsActive = false;
            }
        }

        private async Task getQQPersonalInfo(string token)
        {
            var httpClient = new Windows.Web.Http.HttpClient();

            string infoStr = string.Format("https://graph.qq.com/oauth2.0/me?access_token={0}", token);
            var response = await httpClient.GetAsync(new Uri(infoStr));
            string responseBody = await response.Content.ReadAsStringAsync();
            string callbackBody = responseBody.Substring(9);
            callbackBody = callbackBody.Split(')')[0];
            AccessTokenModel tokenModel = JsonConvert.DeserializeObject<AccessTokenModel>(callbackBody);

            //NickName.Text = infoModel.name;
            //Id.Text = infoModel.idstr;
            //Location.Text = infoModel.location;

            //get info by openId
            infoStr = string.Format("https://graph.qq.com/user/get_user_info?access_token={0}&oauth_consumer_key={1}&openid={2}", token, qqAppId, tokenModel.openid);
            response = await httpClient.GetAsync(new Uri(infoStr));
            responseBody = await response.Content.ReadAsStringAsync();

            UserInfoModel model = JsonConvert.DeserializeObject<UserInfoModel>(responseBody);
            //ret 100031 msg

            MyProgressBar.IsActive = false;
            if (model.ret != "100031")
            {

            } else
            {
                MessageDialog message = new MessageDialog(model.msg);
                await message.ShowAsync();
            }
        }

        private void PushBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AnimationPage));
        }
    }
}
