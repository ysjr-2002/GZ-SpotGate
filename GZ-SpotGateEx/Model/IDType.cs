using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGateEx.Model
{
    /// <summary>
    /// 凭证类型
    /// </summary>
    public enum IDType : int
    {
        //IC卡
        IC = 0,
        //身份证
        ID = 1,
        //人脸
        Face = 2,
        //二维码
        BarCode = 3,
        Upload,
        Init,
    }
}
