using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginThird.Model
{
    public class QQAccessModel
    {
        public string access_token { set; get; }
        public string expires_in { set; get; }

        public string refresh_token { set; get; }
    }
}
