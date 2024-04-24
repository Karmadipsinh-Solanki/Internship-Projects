using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment;

[Table("Student")]
public partial class Student
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "character varying")]
    public string FirstName { get; set; } = null!;

    [Column(TypeName = "character varying")]
    public string? LastName { get; set; }

    public int CourseId { get; set; }

    public int Age { get; set; }

    [Column(TypeName = "character varying")]
    public string Email { get; set; } = null!;

    public bool Gender { get; set; }

    [Column(TypeName = "character varying")]
    public string Course { get; set; } = null!;

    [Column(TypeName = "character varying")]
    public string? Grade { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("Students")]
    public virtual Course CourseNavigation { get; set; } = null!;
}
