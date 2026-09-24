namespace DemoDiagnostic;
public class Product
{
    string name;
    int id;
    double price;
    char[] details = new char[10_000];

    public int Id => id;
    public string Name => name;
    public double Price => price;

    public Product(int id, string name, double price)
    {
        this.id = id;
        this.name = name;
        this.price = price;
    }
}
