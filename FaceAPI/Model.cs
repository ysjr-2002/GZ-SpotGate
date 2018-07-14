using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAPI
{
    internal class KoalaLogin
    {
        public int code { get; set; }

        public LoginData data { get; set; }
    }

    internal class error
    {
        public int code { get; set; }

        public temp data { get; set; }

        public string desc { get; set; }
    }

    internal class temp
    {
        public string msg { get; set; }
    }

    /// <summary>
    /// 登录数据
    /// </summary>
    internal class LoginData
    {
        public string company { get; set; }

        public string contact { get; set; }

        public string email { get; set; }

        public int id { get; set; }

        public bool password_reseted { get; set; }

        public string phone { get; set; }

        public int role_id { get; set; }

        public string username { get; set; }
    }

    /// <summary>
    /// 用户
    /// </summary>
    public class SubjectData
    {
        public string remark { get; set; }

        public int subject_type { get; set; }

        public string description { get; set; }

        public string title { get; set; }

        public long timestamp { get; set; }

        public long start_time { get; set; }

        public long end_time { get; set; }

        public string avatar { get; set; }

        public string job_number { get; set; }

        public object birthday { get; set; }

        public object entry_date { get; set; }

        public string department { get; set; }

        public string phone { get; set; }

        public int id { get; set; }

        public int gender { get; set; }

        public string name { get; set; }

        public int[] photo_ids { get; set; }

        public photo[] photos { get; set; }

        public string email { get; set; }
    }

    public class photo
    {
        public int company_id { get; set; }

        public int id { get; set; }

        public float quality { get; set; }

        public int subject_id { get; set; }

        public string url { get; set; }

        public int version { get; set; }
    }

    public class Subject
    {
        public int code { get; set; }

        public SubjectData data { get; set; }
    }

    public class UploadPhoto
    {
        public int code { get; set; }
        public UpdatePhotoData data { get; set; }
    }

    public class UpdatePhotoData
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public string subject_id { get; set; }
        public string url { get; set; }
        public float quality { get; set; }
    }

    public class AvatarPhoto
    {
        public int code { get; set; }
        public AvatarData data { get; set; }
    }

    public class AvatarData
    {
        public string url { get; set; }
    }

    public class SubjectList
    {
        public SubjectList()
        { }

        public int code { get; set; }
        public List<SubjectData> data { get; set; }
    }

    public class Visitor
    {
        public Visitor()
        {
            ///0: 其他, 1: 面试, 2: 商务, 3: 亲友, 4: 快递送货
            purpose = 1;
            interviewee = "总经理";
            var st = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            start_time = (int)((DateTime.Now - st).TotalSeconds);
            end_time = (int)((DateTime.Now - st).TotalSeconds);
            name = "ysj";
        }
        public string come_from { get; set; }
        public string description { get; set; }
        public int end_time { get; set; }
        public string interviewee { get; set; }
        public string name { get; set; }
        public string photo { get; set; }
        public int purpose { get; set; }
        public string remark { get; set; }
        public int start_time { get; set; }
        public string vip { get; set; }
    }
}
