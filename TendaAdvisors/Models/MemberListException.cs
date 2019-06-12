using System;

namespace TendaAdvisors.Models
{
    public class MemberListException
    {
        public int Id { get; set; }
        public string MemberId { get; set; }
        public string Initials { get; set; }
        public string MemberSurname { get; set; }
        public string IdNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public string Reason { get; set; }
    }
}