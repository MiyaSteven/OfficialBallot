using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OfficialBallot.Models
{
    public class RSVP
    {
        [Key]
        public int VoterId { get; set; }
        // public int UserId { get; set; }
        public User Voter { get; set; }
        public int ProposalId { get; set; }
        public Proposal Proposal { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}