using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YandexAuth1
{
    class YandexAuth
    {
        private string Login { get; set; }
        private string Password { get; set; }
        //private string URL { get; set; } // ???

        /// <summary>
        /// Конструктор требует логин/пароль к аккаунту Yandex 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="pass"></param>
        public YandexAuth(string log, string pass)
        {
            Login = log;
            Password = pass;
        }

        /// <summary>
        ///  авторизация в Yandex, и получение строки JSON сервисов по ссылке
        /// </summary>
        /// <param name="url"></param>
        /// <returns>string strJsonYandex</returns>
        public string GetYandex(string url)
        {
            string strJsonYandex = null;

            System.ArgumentException AuthEx = null;

            try
            {
                string sCookies = "";
                HttpWebRequest myHttpWebRequest1 = (HttpWebRequest)HttpWebRequest.Create("https://passport.yandex.ru/passport?mode=auth");
                myHttpWebRequest1.Method = "POST";
                myHttpWebRequest1.Referer = "http://yandex.ru";
                myHttpWebRequest1.UserAgent = "Mozila/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; MyIE2;";
                myHttpWebRequest1.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                myHttpWebRequest1.Headers.Add("Accept-Language", "ru");
                myHttpWebRequest1.ContentType = "application/x-www-form-urlencoded";

                // передаем куки, полученные в предыдущем запросе
                if (!String.IsNullOrEmpty(sCookies))
                {
                    myHttpWebRequest1.Headers.Add(HttpRequestHeader.Cookie, sCookies);
                }

                // ставим False, чтобы при получении кода 302 не делать автоматический редирект
                myHttpWebRequest1.AllowAutoRedirect = false;

                // передаем параметры
                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1);
                string sQueryString = "retpath=http%3A%2F%2Fmail.yandex.ru%2F&timestamp=" +
                Math.Floor(ts.TotalSeconds).ToString() +
                "&login=" + this.Login + "&passwd=" + this.Password;
                byte[] ByteArr = System.Text.Encoding.GetEncoding(1251).GetBytes(sQueryString);
                myHttpWebRequest1.ContentLength = ByteArr.Length;
                myHttpWebRequest1.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);

                HttpWebResponse myHttpWebResponse1 = null;

                // делаем запрось на страницу авторизации "https://passport.yandex.ru/passport?mode=auth"
                // с логин:паролем
                try
                {
                    myHttpWebResponse1 = (HttpWebResponse)myHttpWebRequest1.GetResponse();
                }
                catch(Exception ex)
                {
                    AuthEx = new System.ArgumentException("Ошибка авторизации.", "Неверный логин или пароль!", ex);
                    throw AuthEx;
                }
                
                // получам куки
                string sCookies2 = "";
                if (!String.IsNullOrEmpty(myHttpWebResponse1.Headers["Set-Cookie"]))
                {
                    sCookies2 = myHttpWebResponse1.Headers["Set-Cookie"];
                }
                
                // запрос на получения json по заданной ссылке
                HttpWebRequest requestJSON = (HttpWebRequest)WebRequest.Create(url);
                
                // подставляем ранее полученные куки
                if (!String.IsNullOrEmpty(sCookies2))
                {
                    requestJSON.Headers.Add(HttpRequestHeader.Cookie, sCookies2);
                }

                // объект ответа на запрос
                HttpWebResponse responseJSON = null;
                //исполняем запрос
                try
                {
                    responseJSON = (HttpWebResponse)requestJSON.GetResponse();
                }
                catch (Exception ex)
                {
                    AuthEx = new System.ArgumentException("Ошибка возврата JSON: ", "Не пройдена авторизация", ex);
                    throw AuthEx;
                }

                // получаем строку JSON на возврат
                using (StreamReader sr = new StreamReader(responseJSON.GetResponseStream()))
                {
                    strJsonYandex = sr.ReadToEnd();
                }

                ////////////////////////////////////////////////
                // можно вернуть и json (JObject), далее пример!
                JObject jsonStat = JObject.Parse(strJsonYandex);
                var jsonSpeedVal = jsonStat["totals"];
                ////////////////////////////////////////////////
            }
            catch (Exception ex)
            {
                if(AuthEx != null)
                    MessageBox.Show(AuthEx.Message + "\n LoginYandex(),\n" + ex.Message);
                else
                    MessageBox.Show("Текущее исключение: " + ex.Message);
            }
            return strJsonYandex;
        }
    }
}
