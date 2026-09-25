using SupportIssue.Domain.Customers;
namespace SupportIssue.Application.Customers;

public interface ICustomerRepository
{
    void Add(Customer customer);
    IReadOnlyList<Customer> GetAll();
    Customer? GetById(Guid id);
    void Update(Customer customer);

}