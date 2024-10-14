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
            ReportIssuesWindow report = new ReportIssuesWindow();
            report.Show();
        }

        private void btnStatus_Click(object sender, RoutedEventArgs e)
        {
            //WILL IMPLEMENT SOON
        }
    }
}