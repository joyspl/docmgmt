using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class DropdownClient
{
    public List<SelectLists> Getdropdown(Dropdown dropdowntype)
    {
        var lst = new List<SelectLists>();
        try
        {
            lst.Add(new SelectLists() { Text = string.Format("Select {0}", Utility.GetCustomDescription(dropdowntype)), Value = default(long) });
            lst.AddRange(DBOperations<SelectLists>.GetAllOrByRange(new SelectLists() { Opmode = (int)dropdowntype }, Constant.usp_Dropdown));
            return lst;
        }
        catch
        {
            return lst;
        }
    }
}

public class SelectLists
{
    public long Value { get; set; }
    public string Valum { get; set; }
    public string Text { get; set; }

    public int Opmode { get; set; }
}