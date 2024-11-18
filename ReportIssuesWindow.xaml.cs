using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.IO;
using Microsoft.Win32;
using PROG_ST10082700_MESSI.Models;
using PROG_ST10082700_MESSI.DataStructures;
using PROG_ST10082700_MESSI.Trees;
using PROG_ST10082700_MESSI.Services;

namespace PROG_ST10082700_MESSI
{
    public partial class ReportIssuesWindow : Window
    {
        private readonly IIssueReportService _issueReportService;
        private readonly IFileService _fileService;
        private readonly IValidationService _validationService;

        public ReportIssuesWindow(IIssueReportService issueReportService,
                                IFileService fileService,
                                IValidationService validationService)
        {
            InitializeComponent();
            _issueReportService = issueReportService;
            _fileService = fileService;
            _validationService = validationService;
        }

        private void BtnAttachFile_Click(object sender, RoutedEventArgs e)
        {
            var fileInfo = _fileService.OpenFile();
            if (fileInfo != null && _validationService.IsValidFile(fileInfo))
            {
                UpdateAttachmentUI(fileInfo);
            }
        }

        private void UpdateAttachmentUI(FileInfo fileInfo)
        {
            try
            {
                if (fileInfo == null) return;

                if (txtAttachedFileName != null)
                    txtAttachedFileName.Text = fileInfo.Name;

                if (btnDownloadAttachment != null)
                    btnDownloadAttachment.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error updating attachment UI: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (_validationService.ValidateForm(txtIssueTitle.Text, txtLocation.Text, cmbCategory.SelectedItem))
            {
                try
                {
                    // Validate all required fields first
                    if (string.IsNullOrWhiteSpace(txtIssueTitle?.Text))
                    {
                        MessageBox.Show("Please enter an issue title.", "Validation Error",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtLocation?.Text))
                    {
                        MessageBox.Show("Please enter a location.", "Validation Error",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (cmbCategory?.SelectedItem == null)
                    {
                        MessageBox.Show("Please select a category.", "Validation Error",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Then use the validation service
                    if (_validationService.ValidateForm(
                        txtIssueTitle.Text.Trim(),
                        txtLocation.Text.Trim(),
                        cmbCategory.SelectedItem))
                    {
                        var newIssue = CreateIssueReport();
                        if (newIssue != null)
                        {
                            _issueReportService.AddIssue(newIssue);
                            MessageBox.Show(
                                $"Issue reported successfully!\nRequest ID: {newIssue.Id}",
                                "Success",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                            );
                            ClearForm();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error submitting issue: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private IssueReport CreateIssueReport()
        {
            try
            {
                // Validate UI controls
                if (rtbDescription?.Document == null)
                {
                    throw new InvalidOperationException("Description control not initialized.");
                }

                var description = new TextRange(
                    rtbDescription.Document.ContentStart,
                    rtbDescription.Document.ContentEnd
                ).Text;

                var categoryContent = (cmbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (string.IsNullOrWhiteSpace(categoryContent))
                {
                    throw new InvalidOperationException("Invalid category selection.");
                }

                var issue = new IssueReport
                {
                    Title = txtIssueTitle.Text.Trim(),
                    Location = txtLocation.Text.Trim(),
                    Category = categoryContent,
                    Description = description.Trim(),
                    AttachedFileContent = _fileService.GetAttachedFileContent(),
                    AttachedFileName = _fileService.GetAttachedFileName()
                };

                // Validate the created issue
                if (!_validationService.ValidateIssueReport(issue))
                {
                    MessageBox.Show(
                        "Failed to create valid issue report. Please check all fields.",
                        "Validation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return null;
                }

                issue.AssignPriority();
                return issue;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error creating issue report: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return null;
            }
        }

        private void BtnBackToHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClearForm()
        {
            try
            {
                if (txtIssueTitle != null) txtIssueTitle.Clear();
                if (txtLocation != null) txtLocation.Clear();
                if (cmbCategory != null) cmbCategory.SelectedIndex = -1;
                if (rtbDescription?.Document != null) rtbDescription.Document.Blocks.Clear();
                if (txtAttachedFileName != null) txtAttachedFileName.Text = "No file chosen";
                _fileService?.ClearAttachedFile();
                if (btnDownloadAttachment != null)
                    btnDownloadAttachment.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error clearing form: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void BtnDownloadAttachment_Click(object sender, RoutedEventArgs e)
        {
            _fileService.DownloadAttachedFile();
        }
    }

    public interface IIssueReportService
    {
        void AddIssue(IssueReport issue);
        IssueReport GetIssueById(string id);
        IssueReport[] GetAllIssues();
        IssueReport[] GetIssuesByStatus(string status);
        void UpdateIssueStatus(string id, string newStatus);
        void AssignIssue(string id, string assignedTo);
        IssueReport[] GetRelatedIssues(string issueId);
        IssueReport[] GetHighPriorityIssues();
    }

    public interface IFileService
    {
        FileInfo OpenFile();
        byte[] GetAttachedFileContent();
        string GetAttachedFileName();
        void ClearAttachedFile();
        void DownloadAttachedFile();
    }

    public interface IValidationService
    {
        bool IsValidFile(FileInfo fileInfo);
        bool ValidateForm(string title, string location, object category);
        bool ValidateIssueReport(IssueReport issue);
    }

    public class FileService : IFileService
    {
        private byte[] attachedFileContent;
        private string attachedFileName;
        public static readonly long MAX_FILE_SIZE = 5 * 1024 * 1024; // 5 MB

        public FileInfo OpenFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Allowed files (*.png;*.jpeg;*.jpg;*.doc;*.docx;*.pdf)|" +
                        "*.png;*.jpeg;*.jpg;*.doc;*.docx;*.pdf"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                attachedFileContent = File.ReadAllBytes(fileInfo.FullName);
                attachedFileName = fileInfo.Name;
                return fileInfo;
            }

            return null;
        }

        public byte[] GetAttachedFileContent() => attachedFileContent;
        public string GetAttachedFileName() => attachedFileName;

        public void ClearAttachedFile()
        {
            attachedFileContent = null;
            attachedFileName = null;
        }

        public void DownloadAttachedFile()
        {
            if (attachedFileContent == null || string.IsNullOrEmpty(attachedFileName))
            {
                MessageBox.Show(
                    "No file attached to download.",
                    "No Attachment",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                FileName = attachedFileName,
                Filter = "Allowed files (*.png;*.jpeg;*.jpg;*.doc;*.docx;*.pdf)|" +
                        "*.png;*.jpeg;*.jpg;*.doc;*.docx;*.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, attachedFileContent);
                MessageBox.Show(
                    "File downloaded successfully!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
    }

    public class ValidationService : IValidationService
    {
        public bool IsValidFile(FileInfo fileInfo)
        {
            if (!IsValidFileType(fileInfo.Extension))
            {
                MessageBox.Show(
                    "Please upload a valid file type (Only Image, Word document, or PDF).",
                    "Invalid File Type",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return false;
            }

            if (!IsValidFileSize(fileInfo.Length))
            {
                MessageBox.Show(
                    "The uploaded file exceeds the maximum allowed size of 5 MB.",
                    "File Too Large",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return false;
            }

            return true;
        }

        public bool ValidateForm(string title, string location, object category)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show(
                    "Please enter an issue title.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return false;
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                MessageBox.Show(
                    "Please enter a location.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return false;
            }

            if (category == null)
            {
                MessageBox.Show(
                    "Please select a category.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return false;
            }

            return true;
        }

        public bool ValidateIssueReport(IssueReport issue)
        {
            if (issue == null) return false;

            if (string.IsNullOrWhiteSpace(issue.Title) ||
                string.IsNullOrWhiteSpace(issue.Location) ||
                string.IsNullOrWhiteSpace(issue.Category))
                return false;

            return true;
        }

        private bool IsValidFileType(string extension)
        {
            string[] validExtensions = { ".png", ".jpeg", ".jpg", ".doc", ".docx", ".pdf" };
            return Array.Exists(validExtensions, e => e.Equals(extension.ToLower()));
        }

        private bool IsValidFileSize(long fileSize)
        {
            return fileSize <= FileService.MAX_FILE_SIZE;
        }
    }
}
