using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OfficialBallot.Models
{
    public class Proposal
    {
        [Key]
        public int ProposalId { get; set; }

        [Required(ErrorMessage = "Proposal Name is required.")]
        [MinLength(2, ErrorMessage = "Proposal Name must be at least 2 characters long.")]
        [Display(Name = "Proposal Name: ")]
        public string ProposalName { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [Display(Name = "Description: ")]
        public string Description { get; set; }
        public int OfficialId { get; set; }
        public User Official { get; set; }
        public int VoterId { get; set; }
        public List<RSVP> ProposalVoters { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}