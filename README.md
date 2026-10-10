# support-issue
Customer management - Fredrik
Ticket registration - Devran	
Ticket handling - Gustav
Ticket overview and dashboard - Suleiiman

# About our application
SupportIssue is a support ticket management application built with C# and WPF.
The application allows users to register customers, create support tickets and manage existing ticketes.

# Project structure
The solution is divided into five projects

1. SupportIssue.Domain – Models and domain-related logic.
  
2. SupportIssue.Application – Services, validation and repository interfaces.

3. SupportIssue.Infrastructure – JSON repositories for saving and loading data.

4. SupportIssue.Presentation – WPF views, navigation and user interaction.

5. SupportIssue.Tests – Unit and integration tests.
   
We used a layered structure inspired by DDD to keep the different responsibilities separated. Repository interfaces and dependency injection are used to connect the layers.

# Start & build application
- Clone or download the repository.
- Open support-issue.slnx in Visual Studio.
- Make sure the .NET 10 SDK is installed.
- Set SupportIssue.Presentation as the startup project.
- Build and run the application.
The application requires Windows because it uses WPF.

# Data storage
Customer and ticket information is stored locally in two JSON files:

customer.json – Customer data
tickets.json – Ticket data

The file paths are configured in App.xaml.cs. If the files don't exist, the application starts with empty data.

# Ticket overview and dashboard
The **Sök ärenden** tab shows all tickets, newest first.
- Search by ticket title or customer name. Upper/lower case does not matter.
- Filter by status: Alla, Nytt, Pågående or Löst.
- Search and filter can be used together. **Visa alla** resets both.

The **Översikt** tab (dashboard) shows how many tickets have each status. The numbers are updated every time the tab is opened.

The logic is in `TicketOverviewService` (Application layer). The screens only call the service, so the logic can be tested without the user interface.

# Testing
We used xUnit for automated testing. The tests cover ticket validation, JSON storage and communication between different parts of the application.

To run the tests, open Test Explorer in Visual Studio and select Run All Tests.
All automated tests passed after merging our branches.

We also tested the application manually by creating and updating customers, registering tickets and managing existing tickets. The main features worked as expected.

We worked in separate Git branches and merged our changes through GitHub. During the merge, we ran into a few conflicts, mainly because some code had been duplicated. We went through the conflicts together, removed the duplicates and made sure the application still worked afterwards.
After merging, we built the solution and ran all tests successfully.

The ticket overview is tested in `TicketOverviewServiceTests` with in-memory fake repositories (case-insensitive search, status filter, search + filter together, and status counts).

# Requirements checklist

Customer registration and updating ✓
Ticket registration and management ✓
Customer and ticket association ✓
Input validation ✓
JSON storage ✓
Layered architecture and dependency injection ✓
WPF interface and navigation ✓
Automated tests ✓
Manual testing ✓
Final UI styling ✓
Ticket search and status filter ✓
Dashboard with ticket count per status ✓

# AI-usage
We used ChatGPT during development to help us understand some programming concepts and find possible solutions when we got stuck. For example, we used it to better understand dependency injection, interfaces, DDD and how the different layers of the application work together. It was also useful for explaining error messages and discussing ways to improve our code. 
