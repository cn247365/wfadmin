using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace WebApp.Models.WfModel
{
// <copyright file="YADSCCostCenterMetadata.cs" tool="martCode MVC5 Scaffolder">
// Copyright (c) 2020 All Rights Reserved
// </copyright>
// <author>neo.zhu</author>
// <date>2020-10-28 14:51:18 </date>
// <summary>Class representing a Metadata entity </summary>
    //[MetadataType(typeof(YADSCCostCenterMetadata))]
    public partial class YADSCCostCenter
    {
    }

    public partial class YADSCCostCenterMetadata
    {
        [Required(ErrorMessage = "Please enter : 系统主键")]
        [Display(Name = "Id",Description ="系统主键",Prompt = "系统主键",ResourceType = typeof(resource.YADSCCostCenter))]
        public int Id { get; set; }

        [Display(Name = "CostCenter",Description ="CostCenter",Prompt = "CostCenter",ResourceType = typeof(resource.YADSCCostCenter))]
        [MaxLength(50)]
        public string CostCenter { get; set; }

        [Display(Name = "CostCenterName",Description ="CostCenterName",Prompt = "CostCenterName",ResourceType = typeof(resource.YADSCCostCenter))]
        [MaxLength(50)]
        public string CostCenterName { get; set; }

        [Display(Name = "CCMGlobalID",Description ="CCMGlobalID",Prompt = "CCMGlobalID",ResourceType = typeof(resource.YADSCCostCenter))]
        [MaxLength(50)]
        public string CCMGlobalID { get; set; }

        [Display(Name = "CCMShortName",Description ="CCMShortName",Prompt = "CCMShortName",ResourceType = typeof(resource.YADSCCostCenter))]
        [MaxLength(50)]
        public string CCMShortName { get; set; }

        [Display(Name = "CCMDisplayName",Description ="CCMDisplayName",Prompt = "CCMDisplayName",ResourceType = typeof(resource.YADSCCostCenter))]
        [MaxLength(50)]
        public string CCMDisplayName { get; set; }

        [Display(Name = "BOMGlobalID",Description ="BOMGlobalID",Prompt = "BOMGlobalID",ResourceType = typeof(resource.YADSCCostCenter))]
        [MaxLength(50)]
        public string BOMGlobalID { get; set; }

        [Display(Name = "BOMShortName",Description ="BOMShortName",Prompt = "BOMShortName",ResourceType = typeof(resource.YADSCCostCenter))]
        [MaxLength(50)]
        public string BOMShortName { get; set; }

        [Display(Name = "BOMDisplayName",Description ="BOMDisplayName",Prompt = "BOMDisplayName",ResourceType = typeof(resource.YADSCCostCenter))]
        [MaxLength(50)]
        public string BOMDisplayName { get; set; }

        [Display(Name = "OverallResopnsible",Description ="OverallResopnsible",Prompt = "OverallResopnsible",ResourceType = typeof(resource.YADSCCostCenter))]
        [MaxLength(50)]
        public string OverallResopnsible { get; set; }

        [Display(Name = "BusLineFunHeadDisplayName",Description ="BusLineFunHeadDisplayName",Prompt = "BusLineFunHeadDisplayName",ResourceType = typeof(resource.YADSCCostCenter))]
        [MaxLength(50)]
        public string BusLineFunHeadDisplayName { get; set; }

        [Display(Name = "BusLineFunHeadGlobalID",Description ="BusLineFunHeadGlobalID",Prompt = "BusLineFunHeadGlobalID",ResourceType = typeof(resource.YADSCCostCenter))]
        [MaxLength(50)]
        public string BusLineFunHeadGlobalID { get; set; }

        [Display(Name = "CreatedDate",Description ="创建时间",Prompt = "创建时间",ResourceType = typeof(resource.YADSCCostCenter))]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "CreatedBy",Description ="创建用户",Prompt = "创建用户",ResourceType = typeof(resource.YADSCCostCenter))]
        [MaxLength(20)]
        public string CreatedBy { get; set; }

        [Display(Name = "LastModifiedDate",Description ="最后更新时间",Prompt = "最后更新时间",ResourceType = typeof(resource.YADSCCostCenter))]
        public DateTime LastModifiedDate { get; set; }

        [Display(Name = "LastModifiedBy",Description ="最后更新用户",Prompt = "最后更新用户",ResourceType = typeof(resource.YADSCCostCenter))]
        [MaxLength(20)]
        public string LastModifiedBy { get; set; }

        [Required(ErrorMessage = "Please enter : 租户主键")]
        [Display(Name = "TenantId",Description ="租户主键",Prompt = "租户主键",ResourceType = typeof(resource.YADSCCostCenter))]
        public int TenantId { get; set; }

    }

}
