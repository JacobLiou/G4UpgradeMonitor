using System.Text.RegularExpressions;

namespace Sofar.Common.Helper
{
    /// <summary>
    /// 正则帮助类 常用正则表达式
    /// </summary>
    public class RegexHelper
    {
        public static bool IsIPv4Address(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }
            return Regex.IsMatch(ip,
                "^((2((5[0-5])|([0-4]\\d)))|([0-1]?\\d{1,2}))(\\.((2((5[0-5])|([0-4]\\d)))|([0-1]?\\d{1,2}))){3}$");
        }

        /// <summary>
        /// 判定当前URL字符串是否合法
        /// </summary>
        /// <param name="strUrl">URL字符串</param>
        /// <returns></returns>
        public static bool IsUrl(string strUrl)
        {
            return Regex.IsMatch(strUrl,
                @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$",
                RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 判断是否是mac地址
        /// </summary>
        /// <Param name="mac">mac地址字符串</Param>
        /// <returns></returns>
        public static bool IsMacAddress(string mac)
        {
            return Regex.IsMatch(mac, "^([0-9A-F]{2}-){5}[0-9A-F]{2}$") || Regex.IsMatch(mac, "^[0-9A-F]{12}$");
        }

        /// <summary>
        /// 判定是否符合手机号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(string str)
        {
            return Regex.IsMatch(str, @"(^0?1[3|4|5|7|8][0-9]\d{8}$)");
        }

        /// <summary>
        /// 判定是否符合Base64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64String(string str)
        {
            return Regex.IsMatch(str, @"[A-Za-z0-9\+\/\=]");
        }

        /// <summary>
        /// 判定是否符合Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            string pattern = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
            return Regex.IsMatch(email.Trim(), pattern);
        }

        /// <summary>
        /// 判定是否符合Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }

            return Regex.IsMatch(guid, "[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}|[A-F0-9]{32}", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 判定是否符合身份证号码
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public static bool IsIdCard(string idCard)
        {
            if (string.IsNullOrEmpty(idCard))
            {
                return false;
            }
            if (idCard.Length == 15)
            {
                return Regex.IsMatch(idCard, @"^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$");
            }
            return idCard.Length == 0x12 && Regex.IsMatch(idCard, @"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])((\d{4})|\d{3}[A-Z])$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 判定是否符合IpAddress
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIpAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }

            string pattern = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";
            return Regex.IsMatch(ip.Trim(), pattern);
        }

        /// <summary>
        /// 判定是否数字
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsNumber(string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                return false;
            }

            return decimal.TryParse(number, out decimal _);
        }

        public static bool IsAlphabetAndDigits(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            string pattern = @"^[a-zA-Z0-9]+$";
            return Regex.IsMatch(input, pattern);
        }

        private static readonly string[] aryReg ={
            @"<script[^>]*?>.*?</script>",
            @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
            @"([\r\n])[\s]+",
            @"&(quot|#34);",
            @"&(amp|#38);",
            @"&(lt|#60);",
            @"&(gt|#62);",
            @"&(nbsp|#160);",
            @"&(iexcl|#161);",
            @"&(cent|#162);",
            @"&(pound|#163);",
            @"&(copy|#169);",
            @"&#(\d+);",
            @"-->",
            @"<!--.*\n"
        };

        /// <summary>
        /// HTML转行成TEXT
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string HtmlToTxt(string strHtml)
        {
            string newReg = aryReg[0];
            string strOutput = strHtml;
            foreach (var t in aryReg)
            {
                Regex regex = new Regex(t, RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, string.Empty);
            }

            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");
            return strOutput;
        }
    }
}
