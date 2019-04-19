using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

public class Document : ModelBase
{
    public long DocID { get; set; }
    public long SubTypeID { get; set; }
    public string SubTypeName { get; set; }
    public long ProjectID { get; set; }
    public string ProjectName { get; set; }
    public long DeptID { get; set; }
    public string DeptName { get; set; }
    public int TaggingType { get; set; }
    public string DocNumber { get; set; }
    public string FileNo { get; set; }
    public int IsUploadApproved { get; set; }

    // added 18.4.19
    public long ApprovedBy { get; set; }
    // added 18.4.19
    public string ApprovedByFullName { get; set; }
    public DateTime ApprovedOn
    {
        get
        {
            return this._CreatedOn.HasValue ? this._CreatedOn.Value : DateTime.Now;
        }
        set
        {
            this._CreatedOn = value;
        }
    }
    public long FinYearID { get; set; }
    public string FinYear { get; set; }
    public long ParentDocID { get; set; }
    public long CreatedBy { get; set; }
    public string CreatedByFullName { get; set; }
    public DateTime CreatedOn
    {
        get
        {
            return this._CreatedOn.HasValue ? this._CreatedOn.Value : DateTime.Now;
        }
        set
        {
            this._CreatedOn = value;
        }
    }
    public long ModifiedBy { get; set; }
    public string ModifiedByFullName { get; set; }
    public DateTime ModifiedOn
    {
        get
        {
            return this._ModifiedOn.HasValue ? this._ModifiedOn.Value : DateTime.Now;
        }
        set
        {
            this._ModifiedOn = value;
        }
    }
    public string DocInfo1 { get; set; }
    public string DocInfo2 { get; set; }
    public string DocInfo3 { get; set; }
    public string DocInfo4 { get; set; }
    public string DocInfo5 { get; set; }
    public string DocInfo6 { get; set; }
    public string DocInfo7 { get; set; }
    public string DocInfo8 { get; set; }
    public string DocInfo9 { get; set; }
    public string DocInfo10 { get; set; }
    public DateTime DocDate
    {
        get
        {
            return this._DocDate.HasValue ? this._DocDate.Value : DateTime.Now;
        }
        set
        {
            this._DocDate = value;
        }
    }
    public int IsDeleted { get; set; }
    public long TotalAttachment { get; set; }
    public string AttachInfo1 { get; set; }
    public string AttachInfo2 { get; set; }
    public string AttachInfo3 { get; set; }
    public string AttachInfo4 { get; set; }

    [XmlIgnore, JsonIgnore]
    public int Opmode { get; set; }

    public long RowStartIndex { get; set; }
    public long RowEndIndex { get; set; }
    public string strSortFields { get; set; }
    public string strCondition { get; set; }
    public long TotalRecords { get; set; }
    public long RowNumber { get; set; }

    [DataLayer.outputparam(ParamType = DataLayer.ParamType.Output)]
    public string OutputVar { get; set; }
}

public class AttachmentFile : ModelBase
{
    public long AttachmentID { get; set; }
    public long DocID { get; set; }
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string FileExtenssion { get; set; }
    public int IsViewerSupported { get; set; }
    public int IsDeleted { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreatedOn
    {
        get
        {
            return this._CreatedOn.HasValue ? this._CreatedOn.Value : DateTime.Now;
        }
        set
        {
            this._CreatedOn = value;
        }
    }
    public long ModifiedBy { get; set; }
    public DateTime ModifiedOn
    {
        get
        {
            return this._ModifiedOn.HasValue ? this._ModifiedOn.Value : DateTime.Now;
        }
        set
        {
            this._ModifiedOn = value;
        }
    }
    public string AttachInfo1 { get; set; }
    public string AttachInfo2 { get; set; }
    public string AttachInfo3 { get; set; }
    // added 19.04.19
    public string AttachInfo4 { get; set; }
    public string FileSizeBytes { get; set; }

    [XmlIgnore, JsonIgnore]
    public int Opmode { get; set; }
}

public class AttachInfoMap
{
    public long ID { get; set; }
    public long SubTypeID { get; set; }
    public string AttachInfo1Name { get; set; }
    public string AttachInfo2Name { get; set; }
    public string AttachInfo3Name { get; set; }
    // added 19.04.19
    public string AttachInfo4Name { get; set; }

    [XmlIgnore, JsonIgnore]
    public int Opmode { get; set; }
}