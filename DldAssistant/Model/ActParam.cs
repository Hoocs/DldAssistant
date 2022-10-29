using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DldAssistant.Model
{
    /// <summary>
    /// 操作参数
    /// </summary>
    [DebuggerDisplay("{text}, Url：{url_goal}")]
    internal class ActParam
    {
        /// <summary>
        /// 目标URL
        /// </summary>
        public string url_goal { get; set; }
        public string text { get; set; }
        public NameValueCollection dicParam = new NameValueCollection();

        public ActParam(string url,string text)
        {
            url_goal = url;
            this.text = text;
            AnalyseParam();
        }

        public void AnalyseParam()
        {
            if(string.IsNullOrWhiteSpace(url_goal))
            {
                return;
            }

            dicParam = HttpUtility.ParseQueryString(url_goal);
        }
    }
}
