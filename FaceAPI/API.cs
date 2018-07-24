﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FaceAPI
{
    class API
    {
        const string root = "https://v2.koalacam.net";
        static string login_url = root + "/auth/login";
        static string subjectlist_url = root + "/mobile-admin/subjects";
        static string subject_url = root + "/subject";
        static string subjectdelete_url = root + "/subject/";
        static string subjectphoto_url = root + "/subject/photo";
        static string avatar_url = root + "/subject/avatar";
        static string event_url = root + "/event/user";
        static string padvisitor_url = root + "/pad/add-visitor";

        string session = "";

        /// <summary>
        /// 登录设备
        /// </summary>
        public Task<bool> Login(string username, string password)
        {
            return Task.Factory.StartNew(() =>
            {
                var dict = new Dictionary<string, string>();
                dict.Add("username", username);
                dict.Add("password", password);

                var request = new HttpRequest();
                var responseStr = request.Login(login_url, dict, out session);
                if (!session.IsEmpty())
                {
                    return true;
                }
                return false;
            });
        }

        public Task<int> CreateSubjectWithPhotos(string name, string uniqueId, string avatarurl, int[] photo_ids)
        {
            return Task.Factory.StartNew(() =>
            {
                SubjectData subject = new SubjectData
                {
                    //0:员工 1:访客 2:VIP
                    subject_type = 0,
                    name = name,
                    gender = 2,
                    //avatar = "@" + (avatarurl),
                    department = "研发部",
                    job_number = uniqueId,
                    photo_ids = photo_ids
                    //birthday = DateTime.Now.Date.ToUnix(),
                    //entry_date = DateTime.Now.Date.AddDays(1).ToUnix(),
                    //start_time = DateTime.Now.Date.ToUnix(),
                    //end_time = DateTime.Now.Date.AddDays(1).ToUnix()
                };

                var request = new HttpRequest();
                var responseStr = request.PostJson(subject_url, HttpMethod.Post.ToString(), session, subject);
                if (responseStr.IsEmpty())
                {
                    return 0;
                }
                var error = responseStr.Deserialize<error>();
                var json = responseStr.Deserialize<Subject>();
                return json.data.id;
            });
        }

        /// <summary>
        /// 上传识别用户图像
        /// </summary>
        /// <param name="emp"></param>
        public Task<UploadPhoto> UpdatePhoto(string path)
        {
            return Task.Factory.StartNew(() =>
            {
                var dict = new Dictionary<string, string>();
                var image = path.FileToByte();
                var request = new HttpRequest();
                var responseStr = request.PostPhoto(subjectphoto_url, "photo", image, session, dict);
                var json = responseStr.Deserialize<UploadPhoto>();
                return json;
            });
        }

        public Task<AvatarPhoto> UpdateAvatar(string path)
        {
            return Task.Factory.StartNew(() =>
            {
                var dict = new Dictionary<string, string>();
                var image = path.FileToByte();
                var request = new HttpRequest();
                var responseStr = request.PostPhoto(avatar_url, "avatar", image, session, dict);
                var json = responseStr.Deserialize<AvatarPhoto>();
                return json;
            });
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        public bool DeleteSubject(int id)
        {
            var url = subjectdelete_url + id;
            var request = new HttpRequest();
            request.Delete(url, session);
            return true;
        }

        public string GetSubject()
        {
            var request = new HttpRequest();
            var responseStr = request.Get(subjectlist_url, session);
            return responseStr;
        }

        public string GetEventUser()
        {
            var request = new HttpRequest();
            var responseStr = request.Get(event_url, session);
            return responseStr;
        }

        public Subject GetUser(int id)
        {
            var request = new HttpRequest();
            var url = subject_url + "/" + id;
            var responseStr = request.Get(url, session);
            return responseStr.Deserialize<Subject>();
        }

        public void UpdateUser(SubjectData user)
        {
            var request = new HttpRequest();
            var url = subject_url + "/" + user.id;
            var responseStr = request.PostJson(url, HttpMethod.Put.ToString(), session, user);
            if (responseStr.IsEmpty())
            {
                return;
            }
            var error = responseStr.Deserialize<error>();
            var json = responseStr.Deserialize<Subject>();
        }

        public string CreateVisitor(string name, string filepath)
        {
            var st = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            var start_time = (int)((DateTime.Now.AddHours(-1) - st).TotalSeconds);
            var end_time = (int)((DateTime.Now.AddHours(1) - st).TotalSeconds);

            var bytes = filepath.FileToByte();
            Dictionary<string, string> dicts = new Dictionary<string, string>();
            dicts.Add("come_from", "come_from");
            dicts.Add("description", "description");
            dicts.Add("end_time", end_time.ToString());
            dicts.Add("interviewee", "interviewee");
            dicts.Add("name", name);
            dicts.Add("purpose", "1");
            dicts.Add("remark", "remark");
            dicts.Add("start_time", start_time.ToString());

            var request = new HttpRequest();
            var responseStr = request.PostPhoto(padvisitor_url, "photo", bytes, session, dicts);
            if (responseStr.IsEmpty())
            {
                return string.Empty;
            }
            return responseStr;
        }
    }
}
