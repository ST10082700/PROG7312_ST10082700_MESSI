using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_ST10082700_MESSI.Models
{

    /// <summary>
    /// Represents a service issue report with complete tracking and management capabilities.
    /// Implements IComparable for use in tree structures.
    /// </summary>
    public class IssueReport : IComparable<IssueReport>
    {
        #region Properties
        // Basic Information
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }

        // Status and Priority
        public string Status { get; set; }
        public string Priority { get; set; }

        // Timestamps
        public DateTime ReportDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? ResolutionDate { get; set; }

        // File Attachments
        public byte[] AttachedFileContent { get; set; }
        public string AttachedFileName { get; set; }

        // Assignment and Tracking
        public string AssignedTo { get; set; }
        public int ResponseTimeHours { get; set; }
        public string Comments { get; set; }
        #endregion

        #region Constructors
        public IssueReport()
        {
            // Generate unique ID using timestamp
            Id = $"SR{DateTime.Now:yyyyMMddHHmmss}";

            // Set initial timestamps
            ReportDate = DateTime.Now;
            LastUpdated = DateTime.Now;

            // Set default values
            Status = "Pending";
            ResponseTimeHours = 24; // Default 24-hour response time
        }
        #endregion

        #region Methods
        /// <summary>
        /// Updates the status of the issue and related timestamps
        /// </summary>
        public void UpdateStatus(string newStatus)
        {
            Status = newStatus;
            LastUpdated = DateTime.Now;

            // Set resolution date if status is completed or closed
            if (newStatus.ToLower() == "completed" || newStatus.ToLower() == "closed")
            {
                ResolutionDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Assigns priority based on category and description content
        /// </summary>
        public void AssignPriority()
        {
            // Emergency keywords for high-priority identification
            string[] emergencyKeywords = {
                "emergency", "urgent", "critical",
                "dangerous", "immediate", "severe"
            };

            // Check for emergency keywords in title or description
            bool hasEmergencyKeywords = emergencyKeywords.Any(keyword =>
                (Title?.ToLower().Contains(keyword) ?? false) ||
                (Description?.ToLower().Contains(keyword) ?? false));

            // Determine priority based on category and keywords
            Priority = (Category?.ToLower(), hasEmergencyKeywords) switch
            {
                ("utilities", true) => "Critical",
                ("utilities", false) => "High",
                ("roads", true) => "Critical",
                ("roads", false) => "High",
                ("sanitation", true) => "High",
                ("sanitation", false) => "Medium",
                (_, true) => "High",    // Any category with emergency keywords
                _ => "Low"              // Default priority
            };
        }

        /// <summary>
        /// Adds a comment to the issue with timestamp
        /// </summary>
        public void AddComment(string comment)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string newComment = $"[{timestamp}] {comment}";

            Comments = string.IsNullOrEmpty(Comments)
                ? newComment
                : $"{Comments}\n{newComment}";

            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Calculates the time elapsed since the issue was reported
        /// </summary>
        public TimeSpan GetAge()
        {
            return DateTime.Now - ReportDate;
        }

        /// <summary>
        /// Checks if the issue has exceeded its response time
        /// </summary>
        public bool HasExceededResponseTime()
        {
            return GetAge().TotalHours > ResponseTimeHours;
        }

        /// <summary>
        /// Implementation of IComparable for use in tree structures
        /// Compares based on priority and age
        /// </summary>
        public int CompareTo(IssueReport other)
        {
            if (other == null) return 1;

            // First compare by priority
            int priorityComparison = ComparePriority(this.Priority, other.Priority);
            if (priorityComparison != 0) return priorityComparison;

            // If same priority, compare by age (older first)
            return other.ReportDate.CompareTo(this.ReportDate);
        }

        /// <summary>
        /// Helper method to compare priorities
        /// </summary>
        private int ComparePriority(string priority1, string priority2)
        {
            int GetPriorityValue(string priority) => priority?.ToLower() switch
            {
                "critical" => 4,
                "high" => 3,
                "medium" => 2,
                "low" => 1,
                _ => 0
            };

            return GetPriorityValue(priority1).CompareTo(GetPriorityValue(priority2));
        }

        /// <summary>
        /// Creates a summary of the issue
        /// </summary>
        public string GetSummary()
        {
            StringBuilder summary = new StringBuilder();
            summary.AppendLine($"Issue ID: {Id}");
            summary.AppendLine($"Title: {Title}");
            summary.AppendLine($"Category: {Category}");
            summary.AppendLine($"Status: {Status}");
            summary.AppendLine($"Priority: {Priority}");
            summary.AppendLine($"Location: {Location}");
            summary.AppendLine($"Reported: {ReportDate:yyyy-MM-dd HH:mm:ss}");
            summary.AppendLine($"Last Updated: {LastUpdated:yyyy-MM-dd HH:mm:ss}");

            if (!string.IsNullOrEmpty(AssignedTo))
                summary.AppendLine($"Assigned To: {AssignedTo}");

            if (ResolutionDate.HasValue)
                summary.AppendLine($"Resolved: {ResolutionDate.Value:yyyy-MM-dd HH:mm:ss}");

            return summary.ToString();
        }
        #endregion
    }
}
