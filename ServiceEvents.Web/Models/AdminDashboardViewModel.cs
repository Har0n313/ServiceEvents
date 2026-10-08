using ServiceEvents.Application.DTOs.EventDTO;
using ServiceEvents.Application.DTOs.UserDTO;
using System.Collections.Generic;

namespace ServiceEvents.Web.Models
{
    public class AdminDashboardViewModel
    {
        public int EventsCount { get; set; }

        public int UsersCount { get; set; }

        public int RegistrationsCount { get; set; }

        public IReadOnlyCollection<EventResponse>? RecentEvents { get; set; }

        public IReadOnlyCollection<UserResponse>? RecentUsers { get; set; }
    }
}
