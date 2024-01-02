    namespace IWantApp.Endpoints.Products;

public record ProductRequest(string name, Guid categoryId, string description, decimal price, bool hasStock, bool active);
