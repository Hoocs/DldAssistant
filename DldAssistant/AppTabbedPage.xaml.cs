namespace DldAssistant;

public partial class AppTabbedPage
{
	public AppTabbedPage()
	{
		InitializeComponent();
    }

	private void TabbedPage_Loaded(object sender, EventArgs e)
    {
        var wp = webPage.RootPage as WebPage;
        var mp = mainPage.RootPage as MainPage;
        mp.SetWebView(wp.webView1);
    }
}