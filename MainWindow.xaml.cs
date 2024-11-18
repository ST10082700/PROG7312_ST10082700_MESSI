using System.Windows;
using System.Windows.Controls;
using PROG_ST10082700_MESSI.Services;
using PROG_ST10082700_MESSI.Trees;
using PROG_ST10082700_MESSI.DataStructures;
using PROG_ST10082700_MESSI.Models;

namespace PROG_ST10082700_MESSI
{
    public partial class MainWindow : Window
    {
        private readonly IIssueReportService _issueReportService;
        private readonly IFileService _fileService;
        private readonly IValidationService _validationService;
        private readonly ServiceRequestManager _requestManager;


        public MainWindow()
        {
            InitializeComponent();

            // Initialize the services
            _issueReportService = new IssueReportService() as IIssueReportService;
            _fileService = new FileService();
            _validationService = new ValidationService();

            // Initialize the request manager with its dependencies
            _requestManager = new ServiceRequestManager(
                _issueReportService,
                new ServiceRequestAVLTree(),
                new ServiceRequestRedBlackTree(),
                new ServiceRequestHeap(),
                new ServiceRequestGraph()
            );
        }

        private void btnEvent_Click(object sender, RoutedEventArgs e)
        {
            IEventService eventService = new EventService();
            IRecommendationService recommendationService = new RecommendationService(eventService);
            EventsWindow eventsWindow = new EventsWindow(eventService, recommendationService);
            eventsWindow.Show();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var report = new ReportIssuesWindow(
                    _issueReportService,
                    _fileService,
                    _validationService
                );

                // Check if _issueReportService is not null
                if (_issueReportService is IssueReportService issueReportService)
                {
                    // Subscribe to issue changes through the service
                    issueReportService.IssueAdded += (issue) =>
                    {
                        var request = new ServiceRequest
                        {
                            Id = issue.Id,
                            Description = issue.Title,
                            Status = issue.Status,
                            SubmissionDate = issue.ReportDate,
                            Priority = issue.Priority,
                            Location = issue.Location,
                            Category = issue.Category
                        };

                        _requestManager.AddRequest(request);
                    };
                }
                else
                {
                   // MessageBox.Show("Issue report service is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                report.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Report Issues window: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }


        private void btnStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var statusWindow = new ServiceRequestStatusWindow(_requestManager);
                statusWindow.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Status window: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }

    public class ServiceRequestManager
    {
        private readonly IIssueReportService _issueReportService;
        private readonly ServiceRequestAVLTree _avlTree;
        private readonly ServiceRequestRedBlackTree _rbTree;
        private readonly ServiceRequestHeap _priorityHeap;
        private readonly ServiceRequestGraph _relationshipGraph;

        public ServiceRequestManager(
            IIssueReportService issueReportService,
            ServiceRequestAVLTree avlTree,
            ServiceRequestRedBlackTree rbTree,
            ServiceRequestHeap priorityHeap,
            ServiceRequestGraph relationshipGraph)
        {
            _issueReportService = issueReportService;
            _avlTree = avlTree;
            _rbTree = rbTree;
            _priorityHeap = priorityHeap;
            _relationshipGraph = relationshipGraph;
        }

        public void AddRequest(ServiceRequest request)
        {
            _avlTree.Insert(request);
            _rbTree.Insert(request);
            _priorityHeap.Insert(request);
            _relationshipGraph.AddRequest(request);
        }

        public ServiceRequest[] GetPrioritizedRequests()
        {
            return _priorityHeap.GetSorted();
        }

        public ServiceRequest[] GetRelatedRequests(string requestId)
        {
            return _relationshipGraph.GetRelatedRequests(requestId);
        }

        public void UpdateRequestStatus(string id, string newStatus)
        {
            var request = _rbTree.Search(new ServiceRequest { Id = id });
            if (request != null)
            {
                request.Status = newStatus;
                AddRequest(request); // Re-add to update all structures
            }
        }

        public ServiceRequest[] GetRequestsByStatus(string status)
        {
            var requests = new ServiceRequest[100];
            int count = 0;

            _avlTree.InorderTraversal(request =>
            {
                if (count < requests.Length &&
                    (status == "All" || request.Status == status))
                {
                    requests[count++] = request;
                }
            });

            var result = new ServiceRequest[count];
            System.Array.Copy(requests, result, count);
            return result;
        }

        public ServiceRequest? SearchRequest(string searchText)
        {
            var request = _rbTree.Search(new ServiceRequest { Id = searchText });
            if (request != null) return request;

            ServiceRequest? found = null;
            _avlTree.InorderTraversal(r =>
            {
                if (found == null && r.Description.Contains(searchText,
                    System.StringComparison.OrdinalIgnoreCase))
                {
                    found = r;
                }
            });

            return found;
        }
    }
}
