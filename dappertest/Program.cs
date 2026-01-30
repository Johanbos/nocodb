// See https://aka.ms/new-console-template for more information
using System.Data;
using MySqlConnector;
using Dapper;
using System.Data.Common;

SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
var connString = "Server=localhost;Database=ecommerce_db;User Id=root;Password=localhost;";
var sql = "SELECT * FROM products";
var products = new List<Product>();
try
{
    using (IDbConnection connection = new MySqlConnection(connString))
    {
        products = (await connection.QueryAsync<Product>(sql)).ToList();
        Console.WriteLine($"Total Products: {products.Count}");

        var newProduct = new Product
        (   id: 0, 
            product_name: "New Dapper Product",
            description: "This is a product added using Dapper ORM",
            price: 29.99M,
            sku: "DPPR-001",
            category: null,
            cost: null,
            is_active: true,
            created_at: DateTime.UtcNow,
            updated_at: null
        );
        var id = await connection.InsertAsync(newProduct);
        Console.WriteLine($"New Product ID: {id}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

[Table("products")]
public class Product(int id, string? product_name, string? description, string? category, decimal price, decimal? cost, string sku, bool is_active, DateTime created_at, DateTime? updated_at)
{
    public int Id { get; private set; } = id;
    public string? product_name { get; private set; } = product_name;
    public string? description { get; private set; } = description;
    public decimal price { get; private set; } = price;
    public string sku { get; private set; } = sku;
    public string? category { get; private set; } = category;
    public decimal? cost { get; private set; }  = cost;
    public bool is_active { get; private set; } = is_active;
    public DateTime created_at { get; private set; } = created_at;
    public DateTime? updated_at { get; private set; } = updated_at;
}