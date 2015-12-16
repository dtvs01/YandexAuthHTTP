using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YandexAuth1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime dateNow = DateTime.Now;
            string[] date = dateNow.ToString().Split(' ');
            string[] date1 = date[0].Split('.');
            string[] time1 = date[1].Split(':');
            if (time1[0].Length == 1)
            {
                time1[0] = "0" + time1[0];
            }

            string timeHit = time1[0] + ":" + time1[1];
            // время
            string allTimeHit = date1[2] + "-" + date1[1] + "-" + date1[0] + " " + timeHit;
            // день получения статистики
            string dateCounter = date1[2] + date1[1] + date1[0];

            // создаем объект авторизации в Yandex!
            YandexAuth ya = new YandexAuth(
                    textBoxLogin.Text,
                    textBoxPass.Text
                );

            // ссылка на получение json страницы "мониторинг: время загрузки страниц"
            string UrlYandexTimeLoad = "https://old.metrika.yandex.ru/api/stat/faced/v2/stat/data?preset=site_speed&id=6936841&lang=ru&reverse=1&per_page=1000&limit=1000&offset=1&group=all&quantile=50&date1=" + dateCounter + "&date2=" + dateCounter + "&table_mode=plain&selected_country=world";

            // любая другая ссылка textBoxLink.Text
            richTextBox1.Text += ya.GetYandex(textBoxLink.Text);

            richTextBox1.Text += "\n\n//////////////////////////////////////////\n//////////////////////////////////////////\n\n";

            // Вызываем метод класса с запросом на "мониторинг: время загрузки страниц"
            richTextBox1.Text += ya.GetYandex(UrlYandexTimeLoad);

            richTextBox1.Text += "\n\n//////////////////////////////////////////\n//////////////////////////////////////////\n\n";

            // пример вызова функции для получения статистики сайта по yandex-метрике (без класса)
            // "мониторинг: время загрузки страниц"  && "трафик по минутам"
            LoginYandex();
            richTextBox1.Text += "отрисовка: " + otrisovka + "\n";
            richTextBox1.Text += "ответ сервера: " + otvet_servera + "\n";
            richTextBox1.Text += "время отклика: " + vremya_otklika + "\n";
            richTextBox1.Text += "хиты: " + hits + "\n";
        }

        // пример использования кода без класса:
        string otrisovka = null;
        string otvet_servera = null;
        string vremya_otklika = null;
        string hits = null;

        // метод получающий тоже, что и по ссылке UrlYandexTimeLoad
        // с разбором возвращенного json
        private void LoginYandex()
        {
            try
            {
                DateTime dateNow = DateTime.Now;
                string[] date = dateNow.ToString().Split(' ');
                string[] date1 = date[0].Split('.');
                string[] time1 = date[1].Split(':');
                if (time1[0].Length == 1)
                {
                    time1[0] = "0" + time1[0];
                }

                string timeHit = time1[0] + ":" + time1[1];
                // время
                string allTimeHit = date1[2] + "-" + date1[1] + "-" + date1[0] + " " + timeHit;
                // день получения статистики
                string dateCounter = date1[2] + date1[1] + date1[0];

                string sCookies = "";
                HttpWebRequest myHttpWebRequest2 = (HttpWebRequest)HttpWebRequest.Create("https://passport.yandex.ru/passport?mode=auth");
                myHttpWebRequest2.Method = "POST";
                myHttpWebRequest2.Referer = "http://yandex.ru";
                myHttpWebRequest2.UserAgent = "Mozila/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; MyIE2;";
                myHttpWebRequest2.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                myHttpWebRequest2.Headers.Add("Accept-Language", "ru");
                myHttpWebRequest2.ContentType = "application/x-www-form-urlencoded";

                // передаем куки, полученные в предыдущем запросе
                if (!String.IsNullOrEmpty(sCookies))
                {
                    myHttpWebRequest2.Headers.Add(HttpRequestHeader.Cookie, sCookies);
                }

                // ставим False, чтобы при получении кода 302 не делать автоматический редирект
                myHttpWebRequest2.AllowAutoRedirect = false;

                string sLogin = textBoxLogin.Text;
                string sPassword = textBoxPass.Text;

                // передаем параметры
                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1);
                string sQueryString = "retpath=http%3A%2F%2Fmail.yandex.ru%2F&timestamp=" +
                Math.Floor(ts.TotalSeconds).ToString() +
                "&login=" + sLogin + "&passwd=" + sPassword;
                byte[] ByteArr = System.Text.Encoding.GetEncoding(1251).GetBytes(sQueryString);
                myHttpWebRequest2.ContentLength = ByteArr.Length;
                myHttpWebRequest2.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);

                // делаем запрос
                HttpWebResponse myHttpWebResponse2 = (HttpWebResponse)myHttpWebRequest2.GetResponse();
                string sLocation = myHttpWebResponse2.Headers["Location"];

                // получам куки
                string sCookies2 = "";
                if (!String.IsNullOrEmpty(myHttpWebResponse2.Headers["Set-Cookie"]))
                {
                    sCookies2 = myHttpWebResponse2.Headers["Set-Cookie"];
                }

                // url берется из браузера в окне просмотра запросов (в Firefox Исследовать элемент -> Сеть)
                string url_stat = "https://old.metrika.yandex.ru/api/stat/faced/v2/stat/data?preset=site_speed&id=6936841&lang=ru&reverse=1&per_page=1000&limit=1000&offset=1&group=all&quantile=50&date1=" + dateCounter + "&date2=" + dateCounter + "&table_mode=plain&selected_country=world"; // &mticket=" + u_id + ":" + token;

                HttpWebRequest requestJSON = (HttpWebRequest)WebRequest.Create(url_stat);
                if (!String.IsNullOrEmpty(sCookies2))
                {
                    requestJSON.Headers.Add(HttpRequestHeader.Cookie, sCookies2);
                }

                HttpWebResponse responseJSON = (HttpWebResponse)requestJSON.GetResponse();

                string strJsonMetricaStat = null;
                using (StreamReader sr = new StreamReader(responseJSON.GetResponseStream()))
                {
                    strJsonMetricaStat = sr.ReadToEnd();
                }

                JObject jsonStat = JObject.Parse(strJsonMetricaStat);

                var jsonSpeedVal = jsonStat["totals"];

                //////////////////////////////////////////////////////////////////////////
                // сразу и хиты получаем!
                string url_hits = "https://old.metrika.yandex.ru/api/stat/load_minutely_all/?offset=1&lang=ru&date1=" + dateCounter + "&date2=" + dateCounter + "&group=day&id=6936841&table_mode=tree&selected_country=world"; //&mticket=" + u_id + ":" + token;
                

                HttpWebRequest requestJSON_hits = (HttpWebRequest)WebRequest.Create(url_hits);
                if (!String.IsNullOrEmpty(sCookies2))
                {
                    requestJSON_hits.Headers.Add(HttpRequestHeader.Cookie, sCookies2);
                }

                HttpWebResponse responseJSON_hits = (HttpWebResponse)requestJSON_hits.GetResponse();

                string strJsonMetricaHits = null;
                using (StreamReader sr = new StreamReader(responseJSON_hits.GetResponseStream()))
                {
                    strJsonMetricaHits = sr.ReadToEnd();
                }
                JObject jsonHits = JObject.Parse(strJsonMetricaHits);

                var jsonHitsVal = jsonHits["data"];

                // разбор хитов
                if (jsonHitsVal != null)
                {
                    var jsonHitsTime = jsonHits.Last;

                    List<string> hit_str = new List<string>();
                    List<string> hit_time = new List<string>();

                    foreach (var result_hit in jsonHitsVal)
                    {
                        JToken hit = result_hit["hits"];
                        foreach (var hit_val in hit)
                        {
                            hit_str.Add(hit_val.ToString());
                        }
                    }

                    // 2015-06-09 00:14 формат даты
                    foreach (var result_hit in jsonHitsTime)
                    {
                        foreach (var hit_val in result_hit)
                        {
                            hit_time.Add(hit_val[1].ToString());
                        }
                    }

                    if (hit_time.Count > 0 && hit_str.Count > 0)
                    {
                        for (int i = 0; i < hit_time.Count; i++)
                        {
                            if (allTimeHit == hit_time[i])
                            {
                                // выставляем отмотку на 10 мин. (яндекс часто опаздывает!)
                                hits = hit_str[i - 10];
                                break;
                            }
                        }
                    }
                }
                else
                {
                    hits = "исчерпана квота";
                }

                ////////////////////////////////////////////////////////////////////////////
                // пишем статистику от yandex
                if (jsonSpeedVal != null)
                {
                    otrisovka = jsonSpeedVal[1].ToString();
                    otvet_servera = jsonSpeedVal[6].ToString();
                    vremya_otklika = jsonSpeedVal[5].ToString();
                }
                else
                {
                    otrisovka = "исчерпана квота";
                    otvet_servera = "исчерпана квота";
                    vremya_otklika = "исчерпана квота";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n LoginYandex()");
            }
        }
    }
}
