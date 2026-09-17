using Microsoft.Maui.Controls;

namespace KurtDhylanMotoShopInventory;

public class MainPage : ContentPage
{
    readonly Entry search = new() { Placeholder = "Search products or services...", Margin = new Thickness(16, 8) };
    readonly VerticalStackLayout productList = new() { Spacing = 10 };
    readonly VerticalStackLayout serviceList = new() { Spacing = 10 };

    public MainPage()
    {
        Title = "KURT DHYLAN MOTO SHOP";
        BackgroundColor = Color.FromArgb("#101114");

        var title = new Label
        {
            Text = "KURT DHYLAN\nMOTO SHOP",
            FontSize = 28,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 20, 0, 4)
        };

        var subtitle = new Label
        {
            Text = "INVENTORY • PRODUCTS • SERVICES",
            FontSize = 12,
            TextColor = Color.FromArgb("#B7BDC9"),
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 14)
        };

        search.TextChanged += (_, _) => RefreshLists();

        var addProduct = new Button { Text = "+ ADD PRODUCT", BackgroundColor = Color.FromArgb("#1E88E5"), TextColor = Colors.White };
        addProduct.Clicked += async (_, _) => await AddProduct();

        var addService = new Button { Text = "+ ADD SERVICE", BackgroundColor = Color.FromArgb("#7E57C2"), TextColor = Colors.White };
        addService.Clicked += async (_, _) => await AddService();

        var clear = new Button { Text = "CLEAR ALL", BackgroundColor = Color.FromArgb("#33363D"), TextColor = Colors.White };
        clear.Clicked += (_, _) =>
        {
            Inventory.Products.Clear();
            Inventory.Services.Clear();
            RefreshLists();
        };

        var buttons = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 8,
            Margin = new Thickness(16, 0, 16, 8)
        };
        buttons.Add(addProduct, 0, 0);
        buttons.Add(addService, 1, 0);
        buttons.Add(clear, 2, 0);

        var productsHeader = Header("PRODUCTS");
        var servicesHeader = Header("SERVICES");

        var scroll = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children = { title, subtitle, search, buttons, productsHeader, productList, servicesHeader, serviceList }
            }
        };

        Content = scroll;
        RefreshLists();
    }

    Label Header(string text) => new()
    {
        Text = text,
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        TextColor = Colors.White,
        Margin = new Thickness(16, 12, 16, 2)
    };

    async Task AddProduct()
    {
        var name = await DisplayPromptAsync("Add Product", "Product name:");
        if (string.IsNullOrWhiteSpace(name)) return;
        var brand = await DisplayPromptAsync("Add Product", "Brand:");
        var model = await DisplayPromptAsync("Add Product", "Model:");
        var priceText = await DisplayPromptAsync("Add Product", "Price (₱):", keyboard: Keyboard.Numeric);
        var stockText = await DisplayPromptAsync("Add Product", "Stocks:", keyboard: Keyboard.Numeric);

        decimal.TryParse(priceText, out var price);
        int.TryParse(stockText, out var stock);
        Inventory.Products.Add(new Product(name, brand ?? "", model ?? "", price, stock));
        RefreshLists();
    }

    async Task AddService()
    {
        var labor = await DisplayPromptAsync("Add Service", "Labor / service name:");
        if (string.IsNullOrWhiteSpace(labor)) return;
        var priceText = await DisplayPromptAsync("Add Service", "Price (₱):", keyboard: Keyboard.Numeric);
        decimal.TryParse(priceText, out var price);
        Inventory.Services.Add(new Service(labor, price));
        RefreshLists();
    }

    void RefreshLists()
    {
        var q = search.Text?.Trim() ?? "";
        productList.Clear();
        serviceList.Clear();

        foreach (var p in Inventory.Products.Where(x => Match($"{x.Name} {x.Brand} {x.Model}", q)))
        {
            productList.Add(Card(
                $"{p.Name}  •  {p.Brand} {p.Model}",
                $"₱{p.Price:N2}    STOCK: {p.Stock}",
                async () =>
                {
                    Inventory.Products.Remove(p);
                    RefreshLists();
                    await DisplayAlert("Product", "Product removed.", "OK");
                }));
        }

        foreach (var s in Inventory.Services.Where(x => Match(x.Name, q)))
        {
            serviceList.Add(Card(
                s.Name,
                $"LABOR PRICE: ₱{s.Price:N2}",
                async () =>
                {
                    Inventory.Services.Remove(s);
                    RefreshLists();
                    await DisplayAlert("Service", "Service removed.", "OK");
                }));
        }

        if (productList.Count == 0) productList.Add(Empty("No products found."));
        if (serviceList.Count == 0) serviceList.Add(Empty("No services found."));
    }

    static bool Match(string text, string q) =>
        string.IsNullOrWhiteSpace(q) || text.Contains(q, StringComparison.OrdinalIgnoreCase);

    View Card(string title, string details, Func<Task> remove)
    {
        var del = new Button { Text = "⋮", FontSize = 22, BackgroundColor = Colors.Transparent, TextColor = Colors.White };
        del.Clicked += async (_, _) => await remove();

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(44)
            },
            Padding = 14,
            BackgroundColor = Color.FromArgb("#1A1C21"),
            Margin = new Thickness(16, 0, 16, 0)
        };
        grid.Add(new VerticalStackLayout
        {
            Spacing = 4,
            Children =
            {
                new Label { Text = title, FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Colors.White },
                new Label { Text = details, FontSize = 13, TextColor = Color.FromArgb("#AEB4BF") }
            }
        }, 0, 0);
        grid.Add(del, 1, 0);
        return grid;
    }

    Label Empty(string text) => new()
    {
        Text = text,
        TextColor = Color.FromArgb("#777D88"),
        HorizontalOptions = LayoutOptions.Center,
        Margin = new Thickness(0, 4, 0, 4)
    };
}

public record Product(string Name, string Brand, string Model, decimal Price, int Stock);
public record Service(string Name, decimal Price);

public static class Inventory
{
    public static List<Product> Products { get; } = new();
    public static List<Service> Services { get; } = new();
}
