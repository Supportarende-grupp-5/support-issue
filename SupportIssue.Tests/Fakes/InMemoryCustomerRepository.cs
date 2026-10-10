using SupportIssue.Application.Customers;
using SupportIssue.Domain.Customers;

namespace SupportIssue.Tests.Fakes
{
    public class InMemoryCustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers = new List<Customer>();

        public void Add(Customer customer)
        {
            _customers.Add(customer);
        }

        public IReadOnlyList<Customer> GetAll()
        {
            return _customers.ToList();
        }

        public Customer? GetById(Guid id)
        {
            foreach (var customer in _customers)
            {
                if (customer.Id == id)
                {
                    return customer;
                }
            }

            return null;
        }

        public void Update(Customer customer)
        {
            // Not needed in our tests
        }
    }
}
