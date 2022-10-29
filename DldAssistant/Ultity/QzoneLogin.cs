using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DldAssistant.Ultity
{
    /// <summary>
    /// QQ空间登录
    /// </summary>
    internal class QzoneLogin
    {
        private string xlogin_url = "https://xui.ptlogin2.qq.com/cgi-bin/xlogin?";
        private string qrshow_url = "https://ssl.ptlogin2.qq.com/ptqrshow?";
        private string qrlogin_url = "https://ssl.ptlogin2.qq.com/ptqrlogin?";

        static string pt_login_sig = string.Empty;
        static string ptqrtoken = string.Empty;

        /// <summary>
        /// 登录参数
        /// </summary>
        Dictionary<string, string> pt_login_sigParam = new Dictionary<string, string>()
        {
            {"proxy_url","https://qzs.qq.com/qzone/v6/portal/proxy.html"},
            {"daid","5"},
            {"hide_title_bar","1"},
            {"low_login","0"},
            {"qlogin_auto_login","1"},
            {"no_verifyimg","1"},
            {"link_target","blank"},
            {"appid","549000912"},
            {"style","22"},
            {"target","self"},
            {"s_url","https://qzs.qq.com/qzone/v5/loginsucc.html?para=izone"},
            {"pt_qr_app","手机QQ空间"},
            {"pt_qr_link","https://z.qzone.com/download.html"},
            {"self_regurl","https://qzs.qq.com/qzone/v6/reg/index.html"},
            {"pt_qr_help_link","https://z.qzone.com/download.html"},
            {"pt_no_auth","0"}
        };

        /// <summary>
        /// 获取token参数
        /// </summary>
        Dictionary<string, string> ptqrtokenParam = new Dictionary<string, string>()
        {
            {"appid","549000912"},
            {"e","2"},
            {"l","M"},
            {"s","3"},
            {"d","72"},
            {"v","4"},
            {"t", new Random().NextSingle().ToString()},
            {"daid","5"},
            {"pt_3rd_aid","0"}
        };

        /// <summary>
        /// 检测状态参数
        /// </summary>
        Dictionary<string, string> checkQrStatusParam = new Dictionary<string, string>()
        {
            {"u1","https://qzs.qq.com/qzone/v5/loginsucc.html?para=izone"},
            {"ptqrtoken", ptqrtoken},
            {"ptredirect","0"},
            {"h","1"},
            {"t","1"},
            {"g","1"},
            {"from_ui","1"},
            {"ptlang","2052"},
            {"action","0-0-" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()},
            {"js_ver","20010217"},
            {"js_type","1"},
            {"login_sig", pt_login_sig },
            {"pt_uistyle","40"},
            {"aid","549000912"},
            {"daid","5"}
        };

        /// <summary>
        /// 计算qrtoken
        /// </summary>
        /// <param name="qrsig"></param>
        /// <returns></returns>
        int hash33(string qrsig)
        {
            int e = 0;
            for (int n = 0; n < qrsig.Length; ++n)
            {
                e += (e << 5) + qrsig[n];
            }
            return 2147483647 & e;
        }

        string GetUrlWithDicParam(string url, Dictionary<string, string> dicParam)
        {
            var param = string.Join("&", dicParam.Select(d => d.Key + "=" + d.Value));
            return url + param;
        }

        public async ValueTask<string> LoadQR(Image qrImg, Action<string> act)
        {
            System.Net.CookieContainer cookieContainer = new System.Net.CookieContainer();
            HttpClient httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.All,
                CookieContainer = cookieContainer,
            }) ;
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"100\", \"Microsoft Edge\";v=\"100\"");
            httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "iframe");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-site");
            httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 Edg/100.0.1185.44");


            string url = GetUrlWithDicParam(xlogin_url, pt_login_sigParam);
            var login = await httpClient.GetAsync(url);
            //var thecookie = login.Headers.GetValues("Set-Cookie").FirstOrDefault(d=>d.Contains("pt_login_sig"));
            //pt_login_sig = System.Text.RegularExpressions.Regex.Match(thecookie, @"pt_login_sig=(.*?);").Result("$1");
            pt_login_sig = cookieContainer.GetCookies(new Uri(xlogin_url))["pt_login_sig"].Value;
            checkQrStatusParam["login_sig"] = pt_login_sig;
            act("pt_login_sig：" + pt_login_sig);
            //获取QRCode
            url = GetUrlWithDicParam(qrshow_url, ptqrtokenParam);
            var qrcode = await httpClient.GetAsync(url);
            //thecookie = qrcode.Headers.GetValues("Set-Cookie").FirstOrDefault(d => d.Contains("qrsig"));
            //var qrsig = System.Text.RegularExpressions.Regex.Match(thecookie, @"qrsig=(.*?);").Result("$1");
            var qrsig = cookieContainer.GetCookies(new Uri(xlogin_url))["qrsig"].Value;
            ptqrtoken = hash33(qrsig).ToString();
            checkQrStatusParam["ptqrtoken"] = ptqrtoken;
            act("ptqrtoken：" + ptqrtoken);

            var stream = await qrcode.Content.ReadAsStreamAsync();
            qrImg.Source = ImageSource.FromStream(() => { return stream; });

            url = GetUrlWithDicParam(qrlogin_url, checkQrStatusParam);
            while(true)
            {
                var respone = await httpClient.GetAsync(url);
                var checkResponse = await respone.Content.ReadAsStringAsync();
                act(checkResponse);
                if (checkResponse.Contains("二维码已失效") || string.IsNullOrWhiteSpace(checkResponse) 
                    || checkResponse.Contains("本次登录已被拒绝") || checkResponse.Contains("登录成功"))
                {
                    if(checkResponse.Contains("登录成功"))
                    {
                        var RK = cookieContainer.GetAllCookies()["RK"].Value;
                        var ptcz = cookieContainer.GetAllCookies()["ptcz"].Value;
                        var uin = cookieContainer.GetAllCookies()["uin"].Value;
                        var skey = cookieContainer.GetAllCookies()["skey"].Value;
                        string QQ = "";
                        var o_cookie = QQ;//cookieContainer.GetAllCookies()["o_cookie"]?.Value;
                        var pac_uid = $"1_{QQ}";//cookieContainer.GetAllCookies()["pac_uid"]?.Value;
                        var Cookie = $"iip=0;RK={RK};ptcz={ptcz};o_cookie={o_cookie};pac_uid={pac_uid};uin={uin};skey={skey};";
                        return Cookie;
                    }

                    break;
                }
                await Task.Delay(2000);
            }

            return string.Empty;
        }

    }  
}
