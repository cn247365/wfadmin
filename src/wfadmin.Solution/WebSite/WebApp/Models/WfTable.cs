using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Repository.Pattern.Ef6;

namespace WebApp.Models
{
  public partial class WfTable:Entity
  {
    [Display(Name ="Catalog",Description = "Catalog")]
    [MaxLength(32)]
    [Required]
    public string Table_Catalog { get; set; }
    [Display(Name = "Schema", Description = "Schema")]
    [MaxLength(32)]
    [Required]
    public string Table_Schema { get; set; }
    [Display(Name = "Table Name", Description = "Table Name")]
    [MaxLength(128)]
    [Required]
    public string Table_Name { get; set; }
    [Display(Name = "Type", Description = "Type")]
    [MaxLength(32)]
    [Required]
    public string Table_Type { get; set; }
    [Display(Name = "Workflow Name", Description = "Workflow Name")]
    [MaxLength(128)]
    public string WorkflowName { get; set; }
    [Display(Name = "Description", Description = "Description")]
    [MaxLength(128)]
    public string Description { get; set; }
    [Display(Name = "Allowed Add", Description = "Allowed Add")]
    public bool AllowedAdd { get; set; }
    [Display(Name = "Allowed Edit", Description = "Allowed Edit")]
    public bool AllowedEdit { get; set; }
    [Display(Name = "Allowed Delete", Description = "Allowed Delete")]
    public bool AllowedDelete { get; set; }
    [Display(Name = "Allowed Query", Description = "Allowed Query")]
    public bool AllowedQuery { get; set; }
    [Display(Name = "Operation Manual", Description = "Operation Manual")]
    [MaxLength(256)]
    public string OperationManual { get; set; }
    [Display(Name = "Action Url", Description = "Action Url")]
    [MaxLength(128)]
    public string Url { get; set; }
  }
}
