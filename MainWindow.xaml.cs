using System.Windows;
using System.Windows.Controls;

namespace PROG_ST10082700_MESSI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnEvent_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of EventService
            IEventService eventService = new EventService();

            // Create an instance of RecommendationService, passing in the EventService
            IRecommendationService recommendationService = new RecommendationService(eventService);

            // Create the EventsWindow, passing in the services
            EventsWindow eventsWindow = new EventsWindow(eventService, recommendationService);

            // Show the window
            eventsWindow.Show();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of IssueReportService
            IIssueReportService issueReportService = new IssueReportService();

            // Create an instance of FileService
            IFileService fileService = new FileService();

            // Create an instance of ValidationService
            IValidationService validationService = new ValidationService();

            // Create the ReportIssuesWindow, passing in the services
            ReportIssuesWindow report = new ReportIssuesWindow(issueReportService, fileService, validationService);

            // Show the window
            report.Show();
        }

        private void btnStatus_Click(object sender, RoutedEventArgs e)
        {
            //WILL IMPLEMENT SOON
        }
    }
}