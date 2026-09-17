using Microsoft.Maui.Controls;

namespace KurtDhylanMotoShopInventory;

public partial class App : Application
{
    public App()
    {
        MainPage = new NavigationPage(new MainPage());
    }
}
