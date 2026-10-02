using System.Windows;
using System.Windows.Controls;
using SupportIssue.Application.Customers;


namespace SupportIssue.Presentation.Views;

public partial class CustomerView : UserControl
{
    private readonly CustomerService _customerService;

    public CustomerView(CustomerService customerService)
    {
        InitializeComponent();

        _customerService = customerService;

        LoadCustomers();
    }

    private void LoadCustomers()
    {
        CustomersDataGrid.ItemsSource = _customerService.GetAllCustomers();
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
    }
}