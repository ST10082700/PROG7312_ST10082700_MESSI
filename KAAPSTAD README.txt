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

## Data Structures Implementation

### 1. Tree Structures
#### a. AVL Tree Implementation
```csharp
public class ServiceRequestAVLTree
{
    // Implementation details...
}
```
- **Role**: Maintains a self-balancing binary search tree for service requests
- **Efficiency Contribution**:
  - O(log n) time complexity for insertions and searches
  - Automatically balances after modifications
  - Perfect for maintaining sorted service requests
- **Example Usage**:
```csharp
var avlTree = new ServiceRequestAVLTree();
avlTree.Insert(new ServiceRequest { Id = "SR001" });
var request = avlTree.Search("SR001");
```

#### b. Red-Black Tree Implementation
```csharp
public class ServiceRequestRedBlackTree
{
    // Implementation details...
}
```
- **Role**: Provides efficient search and insertion operations with less rigid balancing
- **Efficiency Contribution**:
  - O(log n) operations with fewer rotations than AVL
  - Better for frequent insertions
  - Excellent for ID-based lookups
- **Example Usage**:
```csharp
var rbTree = new ServiceRequestRedBlackTree();
rbTree.Insert(new ServiceRequest { Id = "SR002" });
```

### 2. Heap Structure
```csharp
public class ServiceRequestHeap
{
    // Implementation details...
}
```
- **Role**: Manages priority-based organization of service requests
- **Efficiency Contribution**:
  - O(log n) for insertions and extractions
  - Automatically maintains priority order
  - Perfect for priority-based queuing
- **Priority Calculation**:
```csharp
Priority Score = Base Priority + Age Factor + Category Weight
```

### 3. Graph Structure
```csharp
public class ServiceRequestGraph
{
    // Implementation details...
}
```
- **Role**: Manages relationships between service requests
- **Efficiency Contribution**:
  - Efficient relationship tracking
  - O(1) edge lookup
  - Supports related request discovery
- **Relationship Weights**:
  - Location-based (40%)
  - Category-based (30%)
  - Time proximity (20%)
  - Status similarity (10%)

## Service Request Status Feature

### Core Components

1. **Data Structure Manager**
```csharp
public class ServiceRequestManager
{
    private readonly ServiceRequestAVLTree _avlTree;
    private readonly ServiceRequestHeap _priorityHeap;
    private readonly ServiceRequestGraph _relationshipGraph;
    // Implementation...
}
```

2. **Status Tracking System**
- Uses AVL Tree for sorted display
- Heap for priority management
- Graph for relationship tracking

### Efficiency Features

1. **Search Operations**
- ID-based search: O(log n) using Red-Black Tree
- Status-based filtering: O(log n) using AVL Tree
- Priority-based retrieval: O(log n) using Heap

2. **Update Operations**
- Status updates: O(log n)
- Priority adjustments: O(log n)
- Relationship updates: O(1)

## Setup and Building

### Prerequisites
- Visual Studio 2022
- .NET Framework 4.7.2 or later
- Windows OS

### Building the Application
1. Clone the repository
2. Open `PROG_ST10082700_MESSI.sln`
3. Restore NuGet packages
4. Build solution (F5 or Ctrl+Shift+B)

### Running Tests
1. Open Test Explorer in Visual Studio
2. Run all tests to verify functionality

## Usage Examples

### 1. Submitting Service Request
```csharp
var request = new ServiceRequest
{
    Id = "SR001",
    Title = "Water Leak",
    Priority = "High",
    Location = "Main Street"
};
_serviceRequestManager.AddRequest(request);
```

### 2. Checking Request Status
```csharp
var status = _serviceRequestManager.GetRequestStatus("SR001");
var related = _serviceRequestManager.GetRelatedRequests("SR001");
```

### 3. Priority-based Processing
```csharp
var highPriority = _serviceRequestManager.GetPrioritizedRequests();
```

## Performance Considerations

1. **Memory Usage**
- Fixed-size arrays instead of dynamic lists
- Efficient node structures
- Minimal object duplication

2. **Time Complexity**
- Search operations: O(log n)
- Insert operations: O(log n)
- Relationship queries: O(1)

## Contributing
This project is part of an educational assignment. While it's not open for public contributions, feedback and suggestions are welcome.

## License
This project is created for educational purposes and is not licensed for commercial use.

## Contact
For any queries regarding this project, please contact:
Hakim Messi - st10082700@vcconnect.edu.za | messimali@outlook.com

---

This project is developed as part of the PROG7312 POE at IIE Varsity College.