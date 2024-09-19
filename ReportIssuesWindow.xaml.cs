using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Windows.Documents;

namespace PROG_ST10082700_MESSI
{
    public partial class ReportIssuesWindow : Window
    {
        private List<IssueReport> reportedIssues = new List<IssueReport>();
        private string attachedFilePath;
        private const long MAX_FILE_SIZE = 5 * 1024 * 1024; // 5 MB in bytes 

        public ReportIssuesWindow()
        {
            InitializeComponent();
        }


        private void BtnAttachFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Allowed files (*.png;*.jpeg;*.jpg;*.doc;*.docx;*.pdf)|*.png;*.jpeg;*.jpg;*.doc;*.docx;*.pdf" // Filter and allow for image, Word document, and PDF files only!!!
            };
            
            

           // Check if the file is valid
            if (openFileDialog.ShowDialog() == true)
            {
                if (IsValidFileType(openFileDialog.FileName))
                {
                    if (IsValidFileSize(openFileDialog.FileName))
                    {
                        attachedFilePath = openFileDialog.FileName;
                        txtAttachedFileName.Text = Path.GetFileName(attachedFilePath);
                    }
                    else
                    {
                        MessageBox.Show("The uploaded file exceeds the maximum allowed size of 5 MB.",
                                        "File Too Large", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Please upload a valid file type (Only Image, Word document, or PDF!!!!).",
                                    "Invalid File Type", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


        // Check if the file type selected/uploaded is allowed
        private bool IsValidFileType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".png" || extension == ".jpeg" || extension == ".jpg"
                   || extension == ".doc" || extension == ".docx" || extension == ".pdf";
        }

        // Check if the file size is less than or equal to the maximum allowed size
        private bool IsValidFileSize(string filePath)
        {
            try
            {
                long fileSize = new FileInfo(filePath).Length;
                return fileSize <= MAX_FILE_SIZE;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking file size: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }



        /*  private string FormatFileSize(long bytes)
         {
             string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
             int counter = 0;
             decimal number = (decimal)bytes;
             while (Math.Round(number / 1024) >= 1)
             {
                 number /= 1024;
                 counter++;
             }
             return string.Format("{0:n1} {1}", number, suffixes[counter]);
         }*/ // Was trying things out with this method but it was not needed may be be needed in the future part of the project


        // Submit the issue report
        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                IssueReport newIssue = new IssueReport
                {
                    Title = txtIssueTitle.Text,
                    Location = txtLocation.Text,
                    Category = (cmbCategory.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    Description = new TextRange(rtbDescription.Document.ContentStart, rtbDescription.Document.ContentEnd).Text,
                    AttachedFilePath = attachedFilePath,
                    ReportDate = DateTime.Now
                };

                reportedIssues.Add(newIssue);
                MessageBox.Show("Issue reported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
            }
        }

        // Clear the form
        private void BtnBackToHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        // Validate the form fields
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtIssueTitle.Text))
            {
                MessageBox.Show("Please enter an issue title.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Please enter a location.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }


        // Clear the form fields
        private void ClearForm()
        {
            txtIssueTitle.Clear();
            txtLocation.Clear();
            cmbCategory.SelectedIndex = -1;
            rtbDescription.Document.Blocks.Clear();
            txtAttachedFileName.Text = "No file chosen";
            attachedFilePath = null;
        }
    }

    public class IssueReport
    {
        public string Title { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string AttachedFilePath { get; set; }
        public DateTime ReportDate { get; set; }
    }
}