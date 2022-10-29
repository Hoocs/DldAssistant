using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DldAssistant.Ultity
{
    public class DldUrlInfo
    {
        /// <summary>
        /// 基础URL
        /// </summary>
        public static string dldBaseUrl = "https://dld.qzapp.z.qq.com/qpet/cgi-bin/phonepk";

        /// <summary>
        /// JHCM
        /// </summary>
        public static NameValueCollection JHCMQueryString = HttpUtility.ParseQueryString("zapp_uin=&sid=&channel=0&g_ut=1&cmd=jianghudream&op=beginInstance&ins_id=");

        public static NameValueCollection JHCMFRQueryString = HttpUtility.ParseQueryString("zapp_uin=&sid=&channel=0&g_ut=1&cmd=jianghudream&op=getFirstReward&ins_id=");

        /// <summary>
        /// 分享链接
        /// </summary>
        public static string sharegame = "zapp_uin=&B_UID=0&sid=&channel=0&g_ut=1&cmd=sharegame&subtype=1";

        /// <summary>
        /// 斗神塔分享
        /// </summary>
        public static string sharetower = "zapp_uin=&sid=&channel=0&g_ut=1&cmd=sharegame&subtype=2&shareinfo=4";

        /// <summary>
        /// 斗神塔
        /// </summary>
        public static string towerfight = "zapp_uin=&B_UID=0&sid=&channel=0&g_ut=1&cmd=towerfight&type=3";

        /// <summary>
        /// 每日领奖
        /// </summary>
        public static string dailygift = "zapp_uin=&B_UID=0&sid=&channel=0&g_ut=1&cmd=dailygift";

        /// <summary>
        /// 经验木简
        /// </summary>
        public static string jymj = "zapp_uin=&sid=&channel=0&g_ut=1&cmd=use&id=3178&store_type=0";

        /// <summary>
        /// 好友列表
        /// </summary>
        public static string friendlist = "zapp_uin=&sid=&channel=0&g_ut=1&cmd=friendlist";

        /// <summary>
        /// 个人资料
        /// </summary>
        public static string totalinfo = "zapp_uin=&B_UID=0&sid=&channel=0&g_ut=1&cmd=totalinfo&type=1";
    }
}
