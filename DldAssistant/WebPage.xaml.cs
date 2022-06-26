using System.Net;

namespace DldAssistant;

public partial class WebPage : ContentPage
{
	public WebView webView1 => webView;
    public WebPage()
	{
		InitializeComponent();
        webView.Source = "https://dld.qzapp.z.qq.com/qpet/cgi-bin/phonepk?zapp_uin=&sid=&channel=0&g_ut=1";
    }

}