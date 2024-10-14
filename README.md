# Municipal Services Application | Kaapstad Municipality 

## Project Overview
This application is designed to streamline municipal services in South Africa (in city called Kaapstad), providing an efficient and user-friendly platform for citizens to access and request various municipal services. The application enables residents to report issues, access information about local events and announcements, and receive updates on the status of their service requests.

## Features
1. Report Issues
   - Users can submit detailed reports about municipal issues.
   - Supports file attachments (images, documents, PDFs).
   - Categorizes issues for efficient processing.(by roads, sanitation, utilities and others)

2. Local Events and Announcements
   - Displays upcoming local events and important announcements.
   - Allows users to search for events based on categories and dates.
   - Provides personalized event recommendations based on user search history.

3. Service Request Status (Coming Soon)
   - Will allow users to track the progress of their submitted requests.

## Technology Stack
- Language: C#
- Framework: .NET (WPF for desktop application)
- UI: XAML

## Setup and Installation
1. Clone the repository:

https://github.com/ST10082700/PROG7312_ST10082700_MESSI.git

2. Open the solution file (`PROG_ST10082700_MESSI.sln`) in Visual Studio.
3. Restore NuGet packages if necessary.
4. Build the solution.
5. Run the application.

## Usage
### Reporting an Issue
1. Launch the application and click on "Report Issues".
2. Fill in the required fields:
- Issue Title
- Location
- Category
- Description
3. Optionally attach a file (image, document, or PDF).
4. Click "Submit" to report the issue.

## ReportIssuesWindow Functionality

The ReportIssuesWindow is a key component of our Municipal Services Application, designed to allow citizens to report various issues to the municipality. Here are the main features:

1. Issue Reporting Form:
   - Users can enter a title for their issue
   - Location of the issue can be specified
   - A dropdown menu allows selection of the issue category (e.g., Sanitation, Roads, Utilities, Other)
   - A rich text box is provided for detailed description of the issue

2. File Attachment:
   - Users can attach files (images, documents, PDFs) related to the issue
   - The system validates the file type and size before attachment
   - Maximum file size is limited to 5 MB
   - Supported file types include: PNG, JPEG, JPG, DOC, DOCX, and PDF

3. Form Validation:
   - The system ensures all required fields are filled before submission
   - Validates that a category is selected
   - Checks if the attached file (if any) meets the type and size requirements

4. Issue Submission:
   - On successful validation, the issue is added to the system
   - User receives a confirmation message upon successful submission

5. File Download Option (Experimental):
   - A feature to download the attached file is included
   - Note: This feature is currently in a trial phase and may not be fully functional

6. User Interface:
   - Clear labeling and intuitive design for ease of use
   - Responsive layout adapting to different screen sizes

7. Navigation:
   - A 'Back to Home' button allows users to return to the main menu

The ReportIssuesWindow implements SOLID principles and uses dependency injection for improved maintainability and testability. It interacts with several services (IssueReportService, FileService, and ValidationService) to handle various aspects of the issue reporting process.

Note: The file download functionality is currently experimental and may not work as expected. It was implemented as a proof of concept and requires further development and testing.

### Accessing Local Events and Announcements
1. From the main menu, click on "Local Events and Announcements".
2. Browse the list of upcoming events.
3. Use the search bar to find specific events:
- Enter keywords related to the event name or category.
- The search functionality will filter events based on your input.
4. View recommended events:
- The system analyzes your search history to suggest relevant events.
- Recommendations appear at the bottom of the events window.

## Project Structure
- `MainWindow.xaml/.cs`: The main entry point of the application.
- `ReportIssuesWindow.xaml/.cs`: Handles the issue reporting functionality.
- `EventsWindow.xaml/.cs`: Manages the display and interaction with local events and announcements.

## Key Components of Events Functionality
- `EventService`: Manages event data and provides methods for retrieving and searching events.
- `RecommendationService`: Analyzes user search patterns to provide personalized event recommendations.
- `Event`: Represents an individual event with properties like Name, Date, and Category.

## Data Structures Used in Events Feature
- `SortedDictionary<DateTime, List<Event>>`: Efficiently organizes events by date.
- `HashSet<string>`: Manages unique categories for quick lookup.
- `Queue<string>`: Tracks recent user searches for recommendation purposes.
- `Dictionary<string, int>`: Counts search term frequency to improve recommendations.

## Contributing
This project is part of an educational assignment. While it's not open for public contributions, feedback and suggestions are welcome.

## License
This project is created for educational purposes and is not licensed for commercial use.

## Contact
For any queries regarding this project, please contact:
Hakim Messi - st10082700@vcconnect.edu.za | messimali@outlook.com

---

This project is developed as part of the PROG7312 POE at IIE Varsity College.
