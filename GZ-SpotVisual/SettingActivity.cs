using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GZ_SpotVisual
{
    [Activity()]
    public class SettingActivity : RootActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.Setting);

            var btnSave = this.FindViewById<Button>(Resource.Id.btnSave);
            var koala = this.FindViewById<EditText>(Resource.Id.koalaEditText);
            var welcome1 = this.FindViewById<EditText>(Resource.Id.welcomeEditText1);

            var cfg = Config.Profile;
            koala.Text = cfg.ServerIp;
            welcome1.Text = cfg.Welcome;

            btnSave.Click += (s, e) =>
            {
                if (IsEmpty(koala))
                {
                    toast("«Î ‰»Î∑˛ŒÒ∆˜IP");
                    return;
                }

                cfg.ServerIp = koala.Text;
                cfg.Welcome = welcome1.Text;
                Config.SaveProfile();

                StartActivity(typeof(MainActivity));
                this.Finish();
            };
        }

        private int getNumber(string s)
        {
            int i = 0;
            Int32.TryParse(s, out i);
            return i;
        }

        private bool IsEmpty(EditText et)
        {
            var val = et.Text.Trim();
            if (string.IsNullOrEmpty(val))
                return true;
            else
                return false;
        }

        private void toast(string msg)
        {
            var t = Toast.MakeText(this, msg, ToastLength.Short);
            t.SetGravity(GravityFlags.CenterVertical, 0, 0);
            t.Show();
        }
    }
}