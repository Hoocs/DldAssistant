using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Dictionary<string, string> dicParam { get; private set; } = new Dictionary<string, string>();

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
            int index = url_goal.IndexOf("?");
            if (index < 0)
            {
                return;
            }

            //解析
            string needStr = url_goal.Substring(index + 1);
            var lstApm = needStr.Split('&');
            foreach (var item in lstApm)
            {
                var nodes = item.Split('=');
                dicParam[nodes[0]] = nodes[1];
            }
        }
    }
}
