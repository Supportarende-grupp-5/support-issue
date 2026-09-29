using SupportIssue.Application.Customers;
using SupportIssue.Domain.Customers;
using System.Text.Json;

namespace SupportIssue.Infrastructure.Customers;

public class JsonCustomerRepository : ICustomerRepository
{
    private readonly string _filePath;

    public JsonCustomerRepository(string filePath)
    {
        _filePath = filePath;
    }

    private List<Customer> LoadCustomers()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Customer>();
        }

        try
        {
            string json = File.ReadAllText(_filePath);


            List<Customer>? customers =
                JsonSerializer.Deserialize<List<Customer>>(json);

            return customers ?? new List<Customer>();
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("Filen innehåller ogiltig Json");
        }
        catch (IOException)
        {
            throw new InvalidOperationException("Filen kunde inte läsas");
        }

    }

    private void SaveCustomers(List<Customer> customers)
    {
        try
        {
            string json = JsonSerializer.Serialize(customers);
            File.WriteAllText(_filePath, json);
        }
        catch (IOException)
        {
            throw new InvalidOperationException("Filen kunde inte sparas");
        }
    }

    public void Add(Customer customer)
    {
        List<Customer> customers = LoadCustomers();

        customers.Add(customer);
        SaveCustomers(customers);
    }

    public IReadOnlyList<Customer> GetAll()
    {
        return LoadCustomers();
    }

    public Customer? GetById(Guid id)
    {
        List<Customer> customers = LoadCustomers();
        Customer? customer = customers.FirstOrDefault(c => c.Id == id);
        return customer;
    }

    public void Update(Customer customer)
    {
        List<Customer> customers = LoadCustomers();
        int index = customers.FindIndex(c => c.Id == customer.Id);

        if (index == -1)
        {
            throw new ArgumentException("Kunden kunde inte hittas");
        }
        customers[index] = customer;
        SaveCustomers(customers);
    }
}