using DldAssistant.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DldAssistant.Ultity
{
    internal class AnalyseParam
    {
        /// <summary>
        /// 解析返回数据
        /// </summary>
        /// <param name="resultData"></param>
        /// <returns></returns>
        public static List<ActParam> AnalyseResponse(string resultData)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(resultData);
            }
            catch (Exception e)
            {
                int index = resultData.IndexOf("<p>");
                if (index != -1)
                {
                    resultData = resultData.Substring(0, index) + resultData.Substring(index + 3);
                }
                doc.LoadXml(resultData);
            }
            var lstFBNode = doc.SelectNodes("wml/card/a");
            List<ActParam> lstAct = new List<ActParam>();
            foreach (XmlNode fbNode in lstFBNode)
            {
                var param = new ActParam(fbNode.Attributes["href"].Value, fbNode.InnerText);
                lstAct.Add(param);
            }

            return lstAct;
        }






        /// <summary>
        /// 选择合适的操作
        /// </summary>
        /// <param name="lstActParam"></param>
        public static string SelectHref(List<ActParam> lstActParam)
        {
            if(lstActParam.Count == 1)
            {
                return lstActParam[0].url_goal;
            }

            //战斗优先 --等级优先
            //奇遇其次
            //商店最后
            var rebuildParam = lstActParam.Select(d => new { text = d.text, org = d }).ToList();

            //结束回忆
            var endOper = rebuildParam.FindAll(d => d.text.Contains("结束回忆"));
            if (endOper.Any())
            {
                return endOper.OrderByDescending(d => d.text).First().org.url_goal;
            }

            //下一天优先
            var lastday = rebuildParam.FindAll(d => d.text.Contains("下一天"));
            if (lastday.Any())
            {
                return lastday.OrderByDescending(d => d.text).First().org.url_goal;
            }

            //战斗优先
            var fights = rebuildParam.FindAll(d => d.text.Contains("战斗"));
            if(fights.Any())
            {
                return fights.OrderByDescending(d => d.text).First().org.url_goal;
            }

            //奇遇其次
            var meetings = rebuildParam.FindAll(d => d.text.Contains("奇遇"));
            if (meetings.Any())
            {
                return meetings.OrderByDescending(d => d.text).First().org.url_goal;
            }

            //剩下的暂时都选第一个吧
            return lstActParam[0].url_goal;
        }
    }
}
