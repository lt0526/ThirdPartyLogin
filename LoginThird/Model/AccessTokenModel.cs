using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginThird.Model
{
    public class AccessTokenModel
    {
        //{ { "access_token":"2.00HuTKYCamkKpC16a40cd5b9JDIAfB","remind_in":"157679999","expires_in":157679999,"uid":"2337159323"} }

        public string access_token { get; set; }
        public string remind_in { set; get; }
        public string expires_in { set; get; }
        public string uid { set; get; }

        public string client_id { set; get; }

        public string openid { set; get; }
    }
}
