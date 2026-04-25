using Boardgame.Data.Entities;
using Boardgame.Services;
using Boardgame.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Boardgame.ViewModel
{
    internal class EventsVM : ViewModelBase
    {
        private ObservableCollection<EventEntity> _events = [];
        public ObservableCollection<EventEntity> Events
        {
            get => _events;
            set { _events = value; OnPropertyChanged(); }
        }

        private ObservableCollection<EventEntity> _upcomingEvents = [];
        public ObservableCollection<EventEntity> UpcomingEvents
        {
            get => _upcomingEvents;
            set { _upcomingEvents = value; OnPropertyChanged(); }
        }

        private ObservableCollection<EventEntity> _pastEvents = [];
        public ObservableCollection<EventEntity> PastEvents
        {
            get => _pastEvents;
            set { _pastEvents = value; OnPropertyChanged(); }
        }

        private EventEntity? _selectedEvent;
        public EventEntity? SelectedEvent
        {
            get => _selectedEvent;
            set { _selectedEvent = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasSelectedEvent)); RefreshAttendees(); }
        }
        public bool HasSelectedEvent => SelectedEvent is not null;

        private ObservableCollection<EventAttendeeEntity> _attendees = [];
        public ObservableCollection<EventAttendeeEntity> Attendees
        {
            get => _attendees;
            set { _attendees = value; OnPropertyChanged(); }
        }

        // Form
        private string _newEventName = string.Empty;
        public string NewEventName { get => _newEventName; set { _newEventName = value; OnPropertyChanged(); } }
        private string _newDescription = string.Empty;
        public string NewDescription { get => _newDescription; set { _newDescription = value; OnPropertyChanged(); } }
        private string _newLocation = string.Empty;
        public string NewLocation { get => _newLocation; set { _newLocation = value; OnPropertyChanged(); } }
        private DateTime _newEventDate = DateTime.Today.AddDays(7);
        public DateTime NewEventDate { get => _newEventDate; set { _newEventDate = value; OnPropertyChanged(); } }
        private string _newStartTime = "18:00";
        public string NewStartTime { get => _newStartTime; set { _newStartTime = value; OnPropertyChanged(); } }
        private string _newEndTime = "22:00";
        public string NewEndTime { get => _newEndTime; set { _newEndTime = value; OnPropertyChanged(); } }
        private string _newPlannedGames = string.Empty;
        public string NewPlannedGames { get => _newPlannedGames; set { _newPlannedGames = value; OnPropertyChanged(); } }
        private string _newAttendeeName = string.Empty;
        public string NewAttendeeName { get => _newAttendeeName; set { _newAttendeeName = value; OnPropertyChanged(); } }

        private string _statusMessage = string.Empty;
        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }

        public ICommand CreateEventCommand { get; }
        public ICommand DeleteEventCommand { get; }
        public ICommand AddAttendeeCommand { get; }
        public ICommand CancelEventCommand { get; }
        public ICommand CompleteEventCommand { get; }

        public EventsVM()
        {
            CreateEventCommand = new RelayCommand(CreateEvent, _ => !string.IsNullOrWhiteSpace(NewEventName));
            DeleteEventCommand = new RelayCommand(DeleteEvent, _ => SelectedEvent is not null);
            AddAttendeeCommand = new RelayCommand(AddAttendee, _ => SelectedEvent is not null && !string.IsNullOrWhiteSpace(NewAttendeeName));
            CancelEventCommand = new RelayCommand(SetCancelled, _ => SelectedEvent is not null);
            CompleteEventCommand = new RelayCommand(SetCompleted, _ => SelectedEvent is not null);
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var all = await Db.I.GetEventsAsync(Session.CurrentUserId);
                Events = new ObservableCollection<EventEntity>(all);
                UpcomingEvents = new ObservableCollection<EventEntity>(all.Where(e => e.EventDate >= DateTime.Today));
                PastEvents = new ObservableCollection<EventEntity>(all.Where(e => e.EventDate < DateTime.Today));
            }
            catch (Exception ex) { StatusMessage = $"Blad: {ex.Message}"; }
        }

        private async void CreateEvent(object? _)
        {
            try
            {
                var evt = await Db.I.CreateEventAsync(
                    Session.CurrentUserId, NewEventName, NewDescription, NewLocation,
                    NewEventDate, NewStartTime, NewEndTime, NewPlannedGames);

                Events.Add(evt);
                SelectedEvent = evt;
                NewEventName = string.Empty; NewDescription = string.Empty;
                NewLocation = string.Empty; NewPlannedGames = string.Empty;
                StatusMessage = $"Wydarzenie \"{evt.Name}\" utworzone!";
                LoadData();
            }
            catch (Exception ex) { StatusMessage = $"Blad: {ex.Message}"; }
        }

        private async void AddAttendee(object? _)
        {
            if (SelectedEvent is null) return;
            try
            {
                await Db.I.AddAttendeeAsync(SelectedEvent.Id, NewAttendeeName, Session.CurrentDisplayName);
                NewAttendeeName = string.Empty;
                StatusMessage = "Uczestnik dodany.";
                LoadData();
            }
            catch (Exception ex) { StatusMessage = $"Blad: {ex.Message}"; }
        }

        private async void SetCancelled(object? _)
        {
            if (SelectedEvent is null) return;
            await Db.I.UpdateEventStatusAsync(SelectedEvent.Id, EventStatusDb.Cancelled);
            StatusMessage = "Wydarzenie anulowane.";
            LoadData();
        }

        private async void SetCompleted(object? _)
        {
            if (SelectedEvent is null) return;
            await Db.I.UpdateEventStatusAsync(SelectedEvent.Id, EventStatusDb.Completed);
            StatusMessage = "Wydarzenie zakonczone.";
            LoadData();
        }

        private async void DeleteEvent(object? _)
        {
            if (SelectedEvent is null) return;
            await Db.I.DeleteEventAsync(SelectedEvent.Id, Session.CurrentUserId);
            Events.Remove(SelectedEvent);
            SelectedEvent = null;
            StatusMessage = "Wydarzenie usuniete.";
            LoadData();
        }

        private void RefreshAttendees()
        {
            Attendees = SelectedEvent is not null
                ? new ObservableCollection<EventAttendeeEntity>(SelectedEvent.Attendees)
                : [];
        }
    }
}
