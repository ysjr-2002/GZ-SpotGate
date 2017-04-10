using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// RequestParam 的摘要说明
/// </summary>
public class requestParam
{
    public requestParam()
    {
        doorip = "172.21.4.31";
        barcode = "2017041000018";
        type = "P";
    }

    public string doorip { get; set; }

    public string barcode { get; set; }

    public string type { get; set; }
}