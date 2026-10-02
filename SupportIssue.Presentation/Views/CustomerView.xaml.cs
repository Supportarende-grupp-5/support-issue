using System.Windows;
using System.Windows.Controls;
using SupportIssue.Application.Customers;
using SupportIssue.Domain.Customers;

namespace SupportIssue.Presentation.Views;

public partial class CustomerView : UserControl
{
    private readonly CustomerService _customerService;
    private Customer? _selectedCustomer;

    public CustomerView(CustomerService customerService)
    {
        InitializeComponent();

        _customerService = customerService;

        LoadCustomers();
    }

    private void LoadCustomers()
        
    {
        try
        {
            CustomersDataGrid.ItemsSource = _customerService.GetAllCustomers();
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
    {
        var customerName = NameTextBox.Text;
        var customerEmail = EmailTextBox.Text;

        try
        {
            _customerService.CreateCustomer(customerName, customerEmail);

            LoadCustomers();

            NameTextBox.Clear();
            EmailTextBox.Clear();

            MessageBox.Show("Kunden har lagts till.");
        }

        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message);
        }
       
    }

    private void CustomersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CustomersDataGrid.SelectedItem is Customer customer)
        {
            _selectedCustomer = customer;

            NameTextBox.Text = customer.Name;
            EmailTextBox.Text = customer.Email;
        }
    }

    private void UpdateCustomerButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedCustomer == null)
        {
            MessageBox.Show("Välj en kund att uppdatera.");
            return;
        }

        try
        {
            var updatedName = NameTextBox.Text;
            var updatedEmail = EmailTextBox.Text;

            _customerService.UpdateCustomer(_selectedCustomer.Id, updatedName, updatedEmail);

            LoadCustomers();
            NameTextBox.Clear();
            EmailTextBox.Clear();

            _selectedCustomer = null;
            CustomersDataGrid.SelectedItem = null;
            MessageBox.Show("Kunden har uppdaterats.");

        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}