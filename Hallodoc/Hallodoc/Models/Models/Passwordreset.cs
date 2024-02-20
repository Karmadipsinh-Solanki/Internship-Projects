using System;
using System.Collections;
using System.Collections.Generic;

namespace Hallodoc.Models.Models;

public partial class Passwordreset
{
    public string Token { get; set; } = null!;

    public string? Email { get; set; }

    public BitArray? Isupdated { get; set; }

    public DateTime? Createddate { get; set; }
}
