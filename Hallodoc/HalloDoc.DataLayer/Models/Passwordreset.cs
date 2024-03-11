using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hallodoc;

[Table("passwordreset")]
public partial class Passwordreset
{
    [Key]
    [Column("token")]
    [StringLength(256)]
    public string Token { get; set; } = null!;

    [Column("email")]
    [StringLength(256)]
    public string? Email { get; set; }

    [Column("isupdated", TypeName = "bit(1)")]
    public BitArray? Isupdated { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }
}
