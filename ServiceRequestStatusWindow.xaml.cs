
using System;
using System.Windows;
using System.Windows.Controls;
using PROG_ST10082700_MESSI.Models;
using PROG_ST10082700_MESSI.Services;
using PROG_ST10082700_MESSI.Trees;
using PROG_ST10082700_MESSI.DataStructures;

namespace PROG_ST10082700_MESSI
{
    /// <summary>
    /// Interaction logic for ServiceRequestStatusWindow.xaml
    /// </summary>
    public partial class ServiceRequestStatusWindow : Window
    {
        private readonly ServiceRequestAVLTree _avlTree;
        private readonly ServiceRequestRedBlackTree _rbTree;
        private readonly ServiceRequestHeap _priorityHeap;
        private readonly ServiceRequestGraph _relationshipGraph;
        private readonly IIssueReportService _issueReportService;
        private ServiceRequest[] _currentDisplayRequests;
        private int _currentDisplaySize;
        private const string SearchPlaceholder = "Enter issue title";

        public ServiceRequestStatusWindow(IIssueReportService issueReportService)
        {
            InitializeComponent();
            _issueReportService = issueReportService;
            _avlTree = new ServiceRequestAVLTree();
            _rbTree = new ServiceRequestRedBlackTree();
            _priorityHeap = new ServiceRequestHeap();
            _relationshipGraph = new ServiceRequestGraph();
            _currentDisplayRequests = new ServiceRequest[100];
            _currentDisplaySize = 0;

            // Initialize UI
            txtSearch.Text = SearchPlaceholder;
            cmbStatusFilter.SelectedIndex = 0;

            InitializeData();
            UpdateDisplay();
        }

        private void InitializeData()
        {
            try
            {
                var issues = _issueReportService.GetAllIssues();
                foreach (var issue in issues)
                {
                    var request = new ServiceRequest
                    {
                        Id = issue.Id,
                        Description = issue.Title,
                        Status = issue.Status ?? "Pending",
                        SubmissionDate = issue.ReportDate,
                        Priority = issue.Priority ?? "Medium",
                        Location = issue.Location,
                        Category = issue.Category
                    };

                    // Add to all data structures
                    _avlTree.Insert(request);
                    _priorityHeap.Insert(request);
                    _rbTree.Insert(request);
                    _relationshipGraph.AddRequest(request);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading service requests: {ex.Message}",
                              "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateDisplay()
        {
            string selectedStatus = ((ComboBoxItem)cmbStatusFilter.SelectedItem)?.Content.ToString();
            string searchText = txtSearch.Text;

            if (searchText == SearchPlaceholder)
                searchText = string.Empty;

            if (string.IsNullOrEmpty(searchText) && selectedStatus == "All")
            {
                // Use priority heap for default display
                DisplayPrioritizedRequests();
            }
            else
            {
                // Use graph and trees for filtered view
                DisplayFilteredRequests(selectedStatus, searchText);
            }
        }

        private void DisplayPrioritizedRequests()
        {
            var prioritizedRequests = _priorityHeap.GetSorted();
            _currentDisplaySize = Math.Min(prioritizedRequests.Length, _currentDisplayRequests.Length);
            Array.Copy(prioritizedRequests, _currentDisplayRequests, _currentDisplaySize);
            UpdateRequestsList();
        }

        private void DisplayFilteredRequests(string status, string searchText)
        {
            _currentDisplaySize = 0;

            // Use AVL tree for efficient traversal
            _avlTree.InorderTraversal(request =>
            {
                if (_currentDisplaySize >= _currentDisplayRequests.Length)
                    return;

                bool statusMatch = status == "All" || request.Status == status;
                bool searchMatch = string.IsNullOrEmpty(searchText) ||
                                 request.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                 request.Id.Contains(searchText, StringComparison.OrdinalIgnoreCase);

                if (statusMatch && searchMatch)
                {
                    _currentDisplayRequests[_currentDisplaySize++] = request;
                }
            });

            UpdateRequestsList();
        }

        private void UpdateRequestsList()
        {
            var displayArray = new ServiceRequest[_currentDisplaySize];
            Array.Copy(_currentDisplayRequests, displayArray, _currentDisplaySize);
            RequestsList.ItemsSource = displayArray;
        }

        #region Event Handlers
        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == SearchPlaceholder)
            {
                txtSearch.Text = string.Empty;
            }
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = SearchPlaceholder;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text != SearchPlaceholder)
            {
                // Try exact ID search first using Red-Black tree
                var request = _rbTree.Search(new ServiceRequest { Id = txtSearch.Text });
                if (request != null)
                {
                    _currentDisplaySize = 1;
                    _currentDisplayRequests[0] = request;
                    UpdateRequestsList();

                    // Show related requests
                    var related = _relationshipGraph.GetRelatedRequests(request.Id);
                    if (related.Length > 0)
                    {
                        MessageBox.Show($"Found {related.Length} related requests.",
                                      "Related Requests",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Information);
                    }
                }
                else
                {
                    // If no exact match, show filtered results
                    UpdateDisplay();
                }
            }
        }

        private void cmbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDisplay();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            InitializeData();
            UpdateDisplay();
        }

        private void BtnBackToHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Helper Methods
        private string FormatDate(DateTime date)
        {
            return date.ToString("MM/dd/yyyy HH:mm:ss");
        }

        private string GetStatusColor(string status)
        {
            return status?.ToLower() switch
            {
                "pending" => "#FFA500",   // Orange
                "in progress" => "#4169E1",// Royal Blue
                "completed" => "#32CD32",  // Lime Green
                _ => "#808080"            // Gray
            };
        }
        #endregion
    }
}
