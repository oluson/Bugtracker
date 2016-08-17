﻿
using Bugtracker.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class NotificationsViewModel
    {
        public string ProjectName { get; set; }
        public string TicketName { get; set; }
        public string Type { get; set; }
        public string Recipients { get; set; }
        public string SendDate { get; set; }
        public string Message { get; set; }
    }

    public class UserInfoViewModel
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList AllRoles { get; set; }
        public List<string> SelectedRoles { get; set; }
        public IEnumerable<Tickets> AssignedTickets { get; set; }
        public IEnumerable<Projects> AssignedProjects { get; set; }
    }

    //public class AddRemoveRolesViewModel
    //{
    //    public ApplicationUser User { get; set; }
    //    public MultiSelectList Roles { get; set; }
    //    public string[] SelectedRoles { get; set; }
    //public List<string> SelectedRoles { get; set; }
    //public IEnumerable<SelectListItem> SelectedRoles { get; set; }
    //}

    public class AssignUserViewModel
    {
        public ApplicationUser User { get; set; }
        public SelectList Projects { get; set; }
        public int? SelectedProjectId { get; set; }
        public SelectList Tickets { get; set; }
        public int SelectedTicketId { get; set; }
        public IEnumerable<Tickets> CurrentTickets { get; set; }
    }

    public class AddRemoveUsersViewModel
    {
        public string RoleName { get; set; }
        public MultiSelectList Users { get; set; }
        public IEnumerable<string> SelectedUsers { get; set; }
    }

    public class RolesIndexViewModel
    {
        public IEnumerable<ApplicationUser> Submitters { get; set; }
        public IEnumerable<ApplicationUser> Developers { get; set; }
        public IEnumerable<ApplicationUser> ProjectManagers { get; set; }
        public IEnumerable<ApplicationUser> Administrators { get; set; }
    }
}