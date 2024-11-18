using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PROG_ST10082700_MESSI
{
    public partial class EventsWindow : Window
    {
        private readonly IEventService _eventService;
        private readonly IRecommendationService _recommendationService;

        public EventsWindow(IEventService eventService, IRecommendationService recommendationService)
        {
            InitializeComponent();
            _eventService = eventService;
            _recommendationService = recommendationService;
            LoadEvents();
        }

        private void LoadEvents()
        {
            EventsList.ItemsSource = _eventService.GetAllEvents();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchBox.Text.ToLower();
            var searchResults = _eventService.SearchEvents(searchTerm);
            EventsList.ItemsSource = searchResults;

            UpdateRecommendations(searchTerm);
        }

        private void UpdateRecommendations(string searchTerm)
        {
            var recommendations = _recommendationService.GetRecommendations(searchTerm);
            RecommendedEvents.ItemsSource = recommendations;
        }

        private void BtnBackToHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }




    public interface IEventService
    {
        IEnumerable<Event> GetAllEvents();
        IEnumerable<Event> SearchEvents(string searchTerm);
    }

    public interface IRecommendationService
    {
        IEnumerable<Event> GetRecommendations(string searchTerm);
    }

    public class EventService : IEventService
    {
        private readonly SortedDictionary<DateTime, List<Event>> _eventsByDate;

        public EventService()
        {
            _eventsByDate = new SortedDictionary<DateTime, List<Event>>();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            AddEvent(new Event("Road Maintenance", DateTime.Now.AddDays(1), "Roads"));
            AddEvent(new Event("Water Supply Update", DateTime.Now.AddDays(2), "Utilities"));
            AddEvent(new Event("Waste Management Meeting", DateTime.Now.AddDays(3), "Sanitation"));
            AddEvent(new Event("Community Safety Workshop", DateTime.Now.AddDays(4), "Other"));
            AddEvent(new Event("Crime Protection Workshop", DateTime.Now.AddDays(5), "Other"));
            AddEvent(new Event("Water and Sanitation Innovation Event", DateTime.Now.AddDays(6), "Sanitation"));
            AddEvent(new Event("Road Safety Awareness Campaign", DateTime.Now.AddDays(7), "Roads"));
            AddEvent(new Event("Fixing Pothholes Day", DateTime.Now.AddDays(8), "Roads"));

        }

        private void AddEvent(Event newEvent)
        {
            if (!_eventsByDate.ContainsKey(newEvent.Date.Date))
            {
                _eventsByDate[newEvent.Date.Date] = new List<Event>();
            }
            _eventsByDate[newEvent.Date.Date].Add(newEvent);
        }

        public IEnumerable<Event> GetAllEvents()
        {
            return _eventsByDate.SelectMany(kvp => kvp.Value).OrderBy(e => e.Date);
        }

        public IEnumerable<Event> SearchEvents(string searchTerm)
        {
            return GetAllEvents().Where(e =>
                e.Name.ToLower().Contains(searchTerm) ||
                e.Category.ToLower().Contains(searchTerm));
        }
    }

    public class RecommendationService : IRecommendationService
    {
        private readonly IEventService _eventService;
        private readonly Queue<string> _searchHistory;
        private readonly Dictionary<string, int> _searchFrequency;

        public RecommendationService(IEventService eventService)
        {
            _eventService = eventService;
            _searchHistory = new Queue<string>();
            _searchFrequency = new Dictionary<string, int>();
        }

        public IEnumerable<Event> GetRecommendations(string searchTerm)
        {
            UpdateSearchHistory(searchTerm);

            var frequentSearches = _searchFrequency.OrderByDescending(kvp => kvp.Value)
                .Take(3)
                .Select(kvp => kvp.Key);

            return frequentSearches
                .SelectMany(term => _eventService.SearchEvents(term))
                .Distinct()
                .Take(5);
        }

        private void UpdateSearchHistory(string searchTerm)
        {
            if (_searchHistory.Count >= 10)
            {
                _searchHistory.Dequeue();
            }
            _searchHistory.Enqueue(searchTerm);

            if (!_searchFrequency.ContainsKey(searchTerm))
            {
                _searchFrequency[searchTerm] = 0;
            }
            _searchFrequency[searchTerm]++;
        }
    }

    public class Event
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }

        public Event(string name, DateTime date, string category)
        {
            Name = name;
            Date = date;
            Category = category;
        }
    }
}