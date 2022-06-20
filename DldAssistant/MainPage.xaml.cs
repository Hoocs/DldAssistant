using DldAssistant.Model;
using DldAssistant.Ultity;
using System.Xml;

namespace DldAssistant;

public partial class MainPage : ContentPage
{
    string cookie = "";

    int count = 0;
    readonly string jhdmUrl = "https://dld.qzapp.z.qq.com/qpet/cgi-bin/phonepk?zapp_uin=&sid=&channel=0&g_ut=1&cmd=jianghudream&op=beginInstance&ins_id=";
    HttpClient http = new HttpClient(new HttpClientHandler
    {
        AutomaticDecompression = System.Net.DecompressionMethods.All
    });

    public MainPage()
    {
        InitializeComponent();
        http.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        http.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
        //http.DefaultRequestHeaders.Add("Cookie", "iip=0; RK=ffiUbwLbFT; ptcz=fa5aa0b196fe3a8eaf15fd6019e90642e13d02c69a0f2dd4bf84ef8ebdf94c08; pgv_pvid=d; o_cookie=834492623; pac_uid=1_834492623; uin=o0834492623; skey=@9PNc57ZpN");

        //aqqrImg.HeightRequest = 100;
        //qrImg.WidthRequest = 100;
        //qrImg.Aspect = Aspect.AspectFit;
    }

    private async void qrImg_Loaded(object sender, EventArgs e)
    {
        QzoneLogin qzoneLogin = new QzoneLogin();
        try
        {
            //txtCookie.Text = await qzoneLogin.LoadQR(qrImg, AttachView);
        }
        catch (Exception ex)
        {
            AttachView("加载二维码失败：" + ex.Message);

        }
    }

    private void btnReLoadQR_Clicked(object sender, EventArgs e)
    {
        txtCookie.Text = null;
        //qrImg.Source = null;
        qrImg_Loaded(sender, e);
        //QzoneLogin qzoneLogin = new QzoneLogin();
        //qzoneLogin.LoadQR(qrImg, AttachView);
    }

    //循环执行
    private async void OnRepeatUrlClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtCookie.Text))
        {
            await DisplayAlert("", "请输入Cookie", "ok");
            txtCookie.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtRepeatUrl.Text.Trim()))
        {
            await DisplayAlert("", "请输入要循环的Url", "ok");
            txtRepeatUrl.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtNum.Text) || !int.TryParse(txtNum.Text, out int num) || num <= 0)
        {
            await DisplayAlert("", "循环次数格式错误", "ok");
            txtNum.Focus();
            return;
        }

        cookie = txtCookie.Text;
        http.DefaultRequestHeaders.Remove("Cookie");
        http.DefaultRequestHeaders.Add("Cookie", cookie);

        AttachView("循环链接开始");
        for (int i = 0; i < num; i++)
        {
            try
            {
                var beginJH = await http.GetStringAsync(txtRepeatUrl.Text.Trim());
            }
            catch (Exception ex)
            {
                AttachView($"第{i + 1}次失败：" + ex.Message);
            }
            AttachView($"第{i + 1}次已完成");
        }

        AttachView("循环链接结束");
    }

    private async void OnJHCMClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtCookie.Text))
        {
            await DisplayAlert("", "请输入Cookie", "ok");
            txtCookie.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtJHCMId.Text) || !int.TryParse(txtJHCMId.Text, out int numId) || numId <= 0)
        {
            await DisplayAlert("", "长梦Id格式错误", "ok");
            txtJHCMId.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtNum.Text))
        {
            await DisplayAlert("", "请输入要循环的次数", "ok");
            txtNum.Focus();
            return;
        }

        if (!int.TryParse(txtNum.Text, out int num))
        {
            await DisplayAlert("", "循环次数格式错误！", "ok");
            txtNum.Focus();
            return;
        }

        cookie = txtCookie.Text;
        http.DefaultRequestHeaders.Remove("Cookie");
        http.DefaultRequestHeaders.Add("Cookie", cookie);

        AttachView($"开始");
        string opJhdmUrl = jhdmUrl + txtJHCMId.Text;
        for (int i = 0; i < num; i++)
        {
            try
            {
                await DoOperater(opJhdmUrl);
            }
            catch (Exception ex)
            {
                AttachView($"{txtJHCMId.Text}第{i + 1}次失败：" + ex.Message);
            }
            AttachView($"{txtJHCMId.Text}第{i + 1}次已完成");
            count++;
            //CounterLabel.Text = $"Current count: {count}";

            //SemanticScreenReader.Announce(CounterLabel.Text);
        }
        AttachView($"结束");
    }

    private async Task DoOperater(string url)
    {
        bool bEnd = url.Contains("op=endInstance", StringComparison.CurrentCultureIgnoreCase);
        var beginJH = await http.GetStringAsync(url);
        var result = beginJH.Replace("&nbsp;", "");

        if (!result.Contains("大乐斗"))
        {
            AttachView("登录失效");
            await DisplayAlert("", "登录失效", "ok");
            return;
        }
        if (result.Contains("很抱歉，系统繁忙，请稍后再试!"))
        {
            await DoOperater(jhdmUrl);
            return;
        }

        if (result.Contains("香炉不足"))
        {
            bEnd = true;
        }

        if (bEnd)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            string achieve = doc.SelectSingleNode("wml/card/br").NextSibling.Value?.Trim();
            AttachView(achieve);
            AttachView("操作结束");
            return;
        }

        List<ActParam> lstAct = AnalyseParam.AnalyseResponse(result);

        //战斗优先
        var href = AnalyseParam.SelectHref(lstAct);
        href = href.StartsWith("http") ? href : "https:" + href;
        await DoOperater(href);
    }

    private int iTextRow = 0;
    private void AttachView(string text)
    {
        if (iTextRow++ > 100)
        {
            iTextRow = 1;
            this.edText.Text = string.Empty;
        }
        this.edText.Text = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}：{text}{Environment.NewLine}{this.edText.Text}";
    }

    private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            return;
        }
        bool isValid = e.NewTextValue.ToCharArray().All(x => char.IsDigit(x));
        ((Entry)sender).Text = isValid ? e.NewTextValue : e.NewTextValue.Remove(e.NewTextValue.Length - 1);
    }

}

