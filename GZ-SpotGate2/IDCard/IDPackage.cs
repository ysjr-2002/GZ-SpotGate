using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZSpotGate.IDCard
{
    class IDPackage
    {
        static readonly byte[] prefix = new byte[] { 0xAA, 0xAA, 0xAA, 0x96, 0x69 };
        static readonly byte[] cmd_find_ID = new byte[] { 0x20, 0x01 };
        static readonly byte[] cmd_select_ID = new byte[] { 0x20, 0x02 };
        static readonly byte[] cmd_read_ID = new byte[] { 0x30, 0x01 };

        static readonly byte[] len = new byte[] { 0x00, 0x03 };

        public static byte[] getFindPackage()
        {
            var data = new List<byte>();
            data.AddRange(len);
            data.AddRange(cmd_find_ID);
            var xor = GetXOR(data.ToArray());
            var package = new List<byte>();
            package.AddRange(prefix);
            package.AddRange(data);
            package.Add(xor);

            return package.ToArray();
        }

        public static byte[] getSelectPackage()
        {
            var data = new List<byte>();
            data.AddRange(len);
            data.AddRange(cmd_select_ID);
            var xor = GetXOR(data.ToArray());
            var package = new List<byte>();
            package.AddRange(prefix);
            package.AddRange(data);
            package.Add(xor);
            return package.ToArray();
        }

        public static byte[] getReadIDPackage()
        {
            var data = new List<byte>();
            data.AddRange(len);
            data.AddRange(cmd_read_ID);
            var xor = GetXOR(data.ToArray());
            var package = new List<byte>();
            package.AddRange(prefix);
            package.AddRange(data);
            package.Add(xor);
            return package.ToArray();
        }

        public static byte GetXOR(byte[] data)
        {
            byte xor = 0;
            for (int i = 0; i < data.Length; i++)
            {
                xor = (byte)(xor ^ data[i]);
            }
            return xor;
        }

        public static void ParseMessage(byte[] bytes, IDModel model)
        {
            byte[] name_bs = new byte[30];
            byte[] sex_bs = new byte[2];
            byte[] nation_bs = new byte[4];
            byte[] time_bs = new byte[16];
            byte[] address_bs = new byte[70];
            byte[] id_bs = new byte[36];
            byte[] office_bs = new byte[30];
            byte[] start_bs = new byte[16];
            byte[] stop_bs = new byte[16];
            byte[] newaddress_bs = new byte[36];

            Array.Copy(bytes, 0, name_bs, 0, 30);
            Array.Copy(bytes, 30, sex_bs, 0, 2);
            Array.Copy(bytes, 32, nation_bs, 0, 4);
            Array.Copy(bytes, 36, time_bs, 0, 16);
            Array.Copy(bytes, 52, address_bs, 0, 70);
            Array.Copy(bytes, 122, id_bs, 0, 36);
            Array.Copy(bytes, 158, office_bs, 0, 30);
            Array.Copy(bytes, 188, start_bs, 0, 16);
            Array.Copy(bytes, 204, stop_bs, 0, 16);
            Array.Copy(bytes, 220, newaddress_bs, 0, 36);

            Name = BufferToString(name_bs);
            Sex = sex_define[BufferToString(sex_bs).ToInt32()];
            //民族
            Nation = getNational(BufferToString(nation_bs));
            ID = BufferToString(id_bs);
            Address = BufferToString(address_bs);
            Office = BufferToString(office_bs);
            Birthday = BufferToString(time_bs);
            ValidateStart = BufferToString(start_bs);
            ValidateEnd = BufferToString(stop_bs);

            model.Name = Name;
            model.Sex = Sex;
            model.Nation = Nation;
            model.IDCard = ID;
            model.Address = Address;
            model.Office = Office;
            model.Birthday = GetIDDateTime(Birthday);
            model.ValidateStart = GetIDDateTime(ValidateStart);
            model.ValidateEnd = GetIDDateTime(ValidateEnd);
        }

        public static byte[] picbuffer;
        public static void SetPicBuffer(byte[] buffer)
        {
            IDPackage.picbuffer = buffer;
        }

        public static string Name;
        public static string Sex;
        public static string Nation;
        public static string ID;
        public static string Address;
        public static string Office;
        public static string Birthday;
        public static string ValidateStart;
        public static string ValidateEnd;

        private static string BufferToString(byte[] bytes)
        {
            var str = Encoding.Unicode.GetString(bytes);
            str = str.Trim();
            return str;
        }

        public static string GetIDDateTime(string datetime)
        {
            if (string.IsNullOrEmpty(datetime))
                return "1970-1-1";

            if (datetime == "长期" || datetime.Length < 8)
                return "2050-1-1";

            var year = datetime.Substring(0, 4).ToInt32();
            var month = datetime.Substring(4, 2).ToInt32();
            var day = datetime.Substring(6, 2).ToInt32();
            var temp = string.Format("{0}-{1}-{2}", year, month, day);
            return temp;
        }

        private static string[] sex_define = new string[] { "未知", "男", "女", "未说明" };

        public static string getNational(string code)
        {
            string result = string.Empty;
            switch (code)
            {
                case "01":
                    result = "汉族";
                    break;
                case "02":
                    result = "蒙古族";
                    break;
                case "03":
                    result = "回族";
                    break;
                case "04":
                    result = "藏族";
                    break;
                case "05":
                    result = "维吾尔族";
                    break;
                case "06":
                    result = "苗族";
                    break;
                case "07":
                    result = "彝族";
                    break;
                case "08":
                    result = "壮族";
                    break;
                case "09":
                    result = "布依族";
                    break;
                case "10":
                    result = "朝鲜族";
                    break;
                case "11":
                    result = "满族";
                    break;
                case "12":
                    result = "侗族";
                    break;
                case "13":
                    result = "瑶族";
                    break;
                case "14":
                    result = "白族";
                    break;
                case "15":
                    result = "土家族";
                    break;
                case "16":
                    result = "哈尼族";
                    break;
                case "17":
                    result = "哈萨克族";
                    break;
                case "18":
                    result = "傣族";
                    break;
                case "19":
                    result = "黎族";
                    break;
                case "20":
                    result = "僳僳族";
                    break;
                case "21":
                    result = "佤族";
                    break;
                case "22":
                    result = "畲族";
                    break;
                case "23":
                    result = "高山族";
                    break;
                case "24":
                    result = "拉祜族";
                    break;
                case "25":
                    result = "水族";
                    break;
                case "26":
                    result = "东乡族";
                    break;
                case "27":
                    result = "纳西族";
                    break;
                case "28":
                    result = "景颇族";
                    break;
                case "29":
                    result = "柯尔克孜族";
                    break;
                case "30":
                    result = "土族";
                    break;
                case "31":
                    result = "达斡尔族";
                    break;
                case "32":
                    result = "仫佬族";
                    break;
                case "33":
                    result = "羌族";
                    break;
                case "34":
                    result = "布朗族";
                    break;
                case "35":
                    result = "撒拉族";
                    break;
                case "36":
                    result = "毛难族";
                    break;
                case "37":
                    result = "仡佬族";
                    break;
                case "38":
                    result = "锡伯族";
                    break;
                case "39":
                    result = "阿昌族";
                    break;
                case "40":
                    result = "普米族";
                    break;
                case "41":
                    result = "塔吉克族";
                    break;
                case "42":
                    result = "怒族";
                    break;
                case "43":
                    result = "乌孜别克族";
                    break;
                case "44":
                    result = "俄罗斯族";
                    break;
                case "45":
                    result = "鄂温克族";
                    break;
                case "46":
                    result = "崩龙族";
                    break;
                case "47":
                    result = "保安族";
                    break;
                case "48":
                    result = "裕固族";
                    break;
                case "49":
                    result = "京族";
                    break;
                case "50":
                    result = "塔塔尔族";
                    break;
                case "51":
                    result = "独龙族";
                    break;
                case "52":
                    result = "鄂伦春族";
                    break;
                case "53":
                    result = "赫哲族";
                    break;
                case "54":
                    result = "门巴族";
                    break;
                case "55":
                    result = "珞巴族";
                    break;
                case "56":
                    result = "基诺族";
                    break;
                default:
                    result = "其他";
                    break;
            }
            return result;
        }
    }
}
