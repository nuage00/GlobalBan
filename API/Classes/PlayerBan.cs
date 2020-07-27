// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable UnusedMember.Global

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustalorc.GlobalBan.API.Classes
{
    public class PlayerBan
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("PlayerId", TypeName = "BIGINT UNSIGNED")]
        [Required]
        public ulong PlayerId { get; set; }

        [Required] public long Ip { get; set; }

        [Required]
        [StringLength(64)]
        [DefaultValue("")]
        public string Hwid { get; set; }

        [Column("AdminId", TypeName = "BIGINT UNSIGNED")]
        [Required]
        [DefaultValue(0ul)]
        public ulong AdminId { get; set; }

        [Required]
        [StringLength(512)]
        [DefaultValue("N/A")]
        public string Reason { get; set; }

        [Required]
        [DefaultValue(uint.MaxValue)]
        public long Duration { get; set; }

        [Required] public int ServerId { get; set; }

        [Required] public DateTime TimeOfBan { get; set; }

        [Required] [DefaultValue(false)] public bool IsUnbanned { get; set; }
    }
}