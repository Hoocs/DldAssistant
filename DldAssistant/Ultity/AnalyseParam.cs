using DldAssistant.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        /// 解析与好友乐斗数据
        /// </summary>
        /// <param name="resultData"></param>
        /// <returns></returns>
        public static ValueTuple<string, int> AnalyseFightResponse(string resultData)
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
            var texts = doc.SelectNodes("/wml/card//text()");
            StringBuilder sb = new();
            int tl = 0;
            foreach(XmlNode node in texts)
            {
                string v = node.Value.Replace("\t", "").Replace(" ","").TrimEnd();
                v = Regex.Replace(v, @"\s{1,}", "\n");
                sb.Append(v);
                if(v.Contains("当前体力值"))
                {
                    string vTl = Regex.Match(v, @"\d+").Value;
                    int.TryParse(vTl, out tl);
                    break;
                }
            }

            return new (sb.ToString(), tl);
        }



        /// <summary>
        /// 选择合适的操作
        /// </summary>
        /// <param name="lstActParam"></param>
        public static ActParam SelectActParam(List<ActParam> lstActParam)
        {
            if(lstActParam.Count == 1)
            {
                return lstActParam[0];
            }

            //战斗优先 --等级优先
            //奇遇其次
            //商店最后
            var rebuildParam = lstActParam.Select(d => new { text = d.text, org = d }).ToList();

            //结束回忆
            var endOper = rebuildParam.FindAll(d => d.text.Contains("结束回忆"));
            if (endOper.Any())
            {
                return endOper.OrderByDescending(d => d.text).First().org;
            }

            //下一天优先
            var lastday = rebuildParam.FindAll(d => d.text.Contains("下一天"));
            if (lastday.Any())
            {
                return lastday.OrderByDescending(d => d.text).First().org;
            }

            //战斗优先
            var fights = rebuildParam.FindAll(d => d.text.Contains("战斗"));
            if(fights.Any())
            {
                return fights.OrderByDescending(d => d.text).First().org;
            }

            //奇遇其次
            var meetings = rebuildParam.FindAll(d => d.text.Contains("奇遇"));
            if (meetings.Any())
            {
                return meetings.OrderByDescending(d => d.text).First().org;
            }

            //解释身份
            var other = rebuildParam.FindAll(d => d.text.Contains("解释身份") || d.text.Contains("淋雨前行") || d.text.Contains("原地休息") || d.text.Contains("佯装离去") || d.text.Contains("盘问身份")
            || d.text.Contains("锦囊2") || d.text.Contains("重金求见") || d.text.Contains("相约明日") || d.text.Contains("筹备计划"));
            if (other.Any())
            {
                return other.OrderByDescending(d => d.text).First().org;
            }

            //剩下的暂时都选第一个吧
            return lstActParam[0];
        }
    }
}
