# LearnLubbers

LearnLubbers (formerly Canvas_Like) is a comprehensive web-based learning management system (LMS) designed to facilitate online education. Built with ASP.NET Core and Entity Framework, it provides a robust platform for instructors and students to manage classes, assignments, submissions, grading, payments, and more. The system features a clean, responsive interface with role-based access control to ensure appropriate permissions for different user types.

## Features

### For Instructors

- **Class Management**: Create, update, and delete classes with details like department, course number, schedule, capacity, and location.
- **Assignment Management**: Create, publish, and manage assignments with customizable titles, descriptions, due dates, and point values.
- **File Attachments**: Upload and manage files for assignments and course materials.
- **Grading System**: Grade student submissions with a flexible scoring system and provide feedback.
- **Calendar Integration**: Manage class schedules, recurring events, and important dates with a comprehensive calendar system.
- **Student Progress Tracking**: Monitor student performance and submission status across assignments.

### For Students

- **Class Registration**: Browse available classes, register for courses, and view registered classes.
- **Assignment Submissions**: Submit assignments through file uploads or text entries and track submission status.
- **To-Do List**: View upcoming assignments and deadlines in chronological order.
- **Grades & Performance**: View grades, class performance, and progress visualizations.
- **Calendar Access**: Access a personalized calendar showing class schedules and assignment due dates.
- **Tuition Payment**: Make secure online payments for tuition using Stripe integration.

### General Features

- **Authentication and Authorization**: Secure role-based access control for instructors and students using ASP.NET Identity.
- **User Profiles**: Customizable user profiles with personal information and profile pictures.
- **Session Management**: Efficient storage of user-specific data like classes and to-dos in session.
- **Responsive Design**: Mobile-friendly UI built with Bootstrap for cross-device compatibility.
- **PDF Generation**: Generate receipts for payments and other documents using DinkToPdf.
- **Email Notifications**: Send automated emails for important events and notifications using MailKit.
- **Data Visualization**: Visual representation of grades and performance metrics using Google Charts.

## Technologies Used

- **Backend**: ASP.NET Core 8.0, C#, Entity Framework Core
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap, jQuery
- **Database**: Microsoft SQL Server
- **Authentication**: ASP.NET Identity
- **Payment Processing**: Stripe API
- **PDF Generation**: DinkToPdf
- **Email Services**: MailKit, MimeKit
- **Calendar**: FullCalendar.js
- **Charts**: Google Charts
- **Testing**: MSTest, Moq, Selenium

## Project Structure

```plaintext
LearnLubbers/
├── Canvas_Like/                # Main application
│   ├── Areas/                  # Identity and account management
│   │   └── Identity/           # User authentication and profile management
│   ├── Pages/                  # Razor Pages for UI
│   │   ├── Account/            # Account management and payment processing
│   │   ├── Assignments/        # Assignment creation, submission, and grading
│   │   ├── Calendar/           # Calendar and scheduling functionality
│   │   ├── Classes/            # Class management and registration
│   │   └── Registration/       # Course registration for students
│   ├── wwwroot/                # Static files (CSS, JS, images)
│   │   ├── attachments/        # Uploaded assignment attachments
│   │   ├── images/             # System and user images
│   │   ├── lib/                # Client-side libraries
│   │   ├── receipts/           # Generated payment receipts
│   │   └── Submissions/        # Student assignment submissions
│   └── Program.cs              # Application entry point and configuration
├── Canvas_Like.Tests/          # Unit and integration tests
│   ├── Selenium/               # Browser automation tests
│   └── UnitTests/              # Unit tests for application logic
├── DataAccess/                 # Data access layer
│   ├── DbInitializer/          # Database seeding and initialization
│   ├── Migrations/             # EF Core migrations
│   ├── ApplicationDbContext.cs # Database context
│   └── UnitOfWork.cs           # Repository pattern implementation
├── Infrastructure/             # Domain models and interfaces
│   ├── Interfaces/             # Service and repository interfaces
│   └── Models/                 # Domain entities
├── Utility/                    # Helper classes and constants
│   ├── EmailSender.cs          # Email service implementation
│   └── SD.cs                   # Static details and constants
└── README.md                   # Project documentation
```

## Installation and Setup

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or later (recommended)

### Steps

1. Clone the repository
2. Open the solution in Visual Studio
3. Update the connection string in `appsettings.json` if needed
4. Run the following commands in the Package Manager Console:

   ```powershell
   Update-Database
   ```

5. Configure Stripe API keys in `appsettings.json` for payment processing
6. Run the application

## Testing

The solution includes both unit tests and Selenium-based UI tests. To run the tests:

1. Open the Test Explorer in Visual Studio
2. Click "Run All Tests" or select specific tests to run

## Contributors

This project was developed as part of a software engineering course at Weber State University.
