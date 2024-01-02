namespace IWantApp.Endpoints.Products;

public record ProductResponse(string name, string category, string description, decimal price, bool hasStock, bool active);
public record ProductResponseId(Guid id, string name, string category, string description, decimal price, bool hasStock, bool active);
