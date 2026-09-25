using SupportIssue.Domain.Customers;
namespace SupportIssue.Application.Customers;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public void CreateCustomer(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Kundens namn måste fyllas i");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Kundens email måste fyllas i");
        }

        if (!email.Contains("@"))
        {
            throw new ArgumentException("E-postadressen måste vara giltig");
        }

        Customer customer = new Customer();

        customer.Id = Guid.NewGuid();
        customer.Name = name;
        customer.Email = email;

        _customerRepository.Add(customer);
    }
    public IReadOnlyList<Customer> GetAllCustomers()
    {
        return _customerRepository.GetAll();
    }
    public Customer? GetCustomerById(Guid id)
    {
        return _customerRepository.GetById(id);
    }

    public void UpdateCustomer(Guid id, string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Kundens namn måste fyllas i");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Kundens email måste fyllas i");
        }

        if (!email.Contains("@"))
        {
            throw new ArgumentException("E-postadressen måste vara giltig");
        }

        Customer? customer = _customerRepository.GetById(id);

        if (customer == null)
        {
            throw new ArgumentException("Kunden kunde inte hittas.");
        }

        customer.Name = name;
        customer.Email = email;

        _customerRepository.Update(customer);
    }
}