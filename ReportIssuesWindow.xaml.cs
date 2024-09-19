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

        public ReportIssuesWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnAttachFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                attachedFilePath = openFileDialog.FileName;
                txtAttachedFileName.Text = System.IO.Path.GetFileName(attachedFilePath);  // Using fully qualified name
            }
        }

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

        private void BtnBackToHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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