using NSoup;
using NSoup.Nodes;
using NSoup.Select;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace _0gichul_DownLoader
{
    public partial class Form1 : Form
    {
        String main_url = "https://0gichul.com/";
        ArrayList board_url_list = new ArrayList();
        Hashtable file_hash_table = new Hashtable(); //기출지 해시테이블
        Hashtable file_ans_hash_table = new Hashtable(); //정답지 해시테이블
        Hashtable desc_file_hash_table = new Hashtable(); //해설지 해시테이블
        ArrayList file_url_list = new ArrayList();
        ArrayList file_ans_url_list = new ArrayList();
        ArrayList desc_file_url_list = new ArrayList();
        int board_url_cnt = 0;
        public Form1()
        {
            InitializeComponent();
        }
        public void Answer_Downloader(String url, String path)

        {

            try
            {
                Thread.Sleep(1);
                WebClient webClient = new WebClient();
                webClient.DownloadFile(url, path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                Console.ReadLine();
            }

        }
        public void Answer_Downloader2(String url, String path)

        {

            try
            {
                Thread.Sleep(1);
                WebClient webClient = new WebClient();
                webClient.DownloadFile(url, path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                Console.ReadLine();
            }

        }
        public void Answer_Downloader3(String url, String path)

        {

            try
            {
                Thread.Sleep(1);
                WebClient webClient = new WebClient();
                webClient.DownloadFile(url, path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                Console.ReadLine();
            }

        }
        public String Answer_Query(String url)
        {
            int speed = 0;

            Thread thread = new Thread(new ThreadStart(delegate () // thread 생성
            {
                this.Invoke(new Action(delegate ()
                {
                    speed = comboBox1.SelectedIndex;
                }));
            }));
            thread.Start();

            switch (speed)
            {
                case 0:
                    Thread.Sleep(1000);
                    break;
                case 1:
                    Thread.Sleep(500);
                    break;
                case 2:
                    Thread.Sleep(1);
                    break;
                default:
                    break;
            }



            string responseText = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 30000;
            request.KeepAlive = false;
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                Stream respStream = resp.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();
                }
                
            }
            request.Abort();
            var match = Regex.Match(responseText, "\"(https://0gichul.com.*/(.*))\"");
            return match.ToString().Trim().ToString().Replace("\"","").ToString().Trim().ToString();
        }

        public static string GetRequest(String url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 30000;
                request.KeepAlive = false;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int Page_Max_Count(String url) //마지막 페이지 구하기
        {
            var html = GetRequest(url);
            Document doc = NSoupClient.Parse(html);

            Elements content = doc.Select("fieldset > a");

            int i = 0;
            foreach (var item in content)
            {
                i++;
            }
            return i;
           // Console.WriteLine(i.ToString());
        }

        public void get_board_url_list(String url) //부분 페이지에서 게시글 주소 구하기
        {
            var html = GetRequest(url);
            Document doc = NSoupClient.Parse(html);

            Elements content = doc.Select("div.bd_lst_wrp > ol > li > a");
            int i = 0;
            foreach (var item in content)
            {
                i++;
                String linkHref = item.Attr("href");
                //Console.WriteLine(main_url + linkHref.ToString());
                board_url_list.Add(main_url + linkHref.ToString());
            }
            board_url_cnt = i;
        }

        public void get_total_board_url_list(String url,int max_page_cnt) //전체 게시글 주소 구하기
        {
            Thread thread = new Thread(new ThreadStart(delegate () // thread 생성
            {
                this.Invoke(new Action(delegate ()
                {

                    this.Text = "0gichul_Downloader - 1/3";
                }));
            }));
            thread.Start();

            for (int i = 1; i <= max_page_cnt; i++)
            {
                get_board_url_list(url + "&page=" + i.ToString());
            }
            int vals = board_url_cnt * max_page_cnt;

            for(int i=0;i< board_url_list.Count; i++)
           // for (int i = 0; i < 5; i++)
            {
 
                thread = new Thread(new ThreadStart(delegate () // thread 생성
                {
                    this.Invoke(new Action(delegate () 
                    {

                        this.Text = "0gichul_Downloader- 2/3 File Count " + (i+1).ToString() + "/" + board_url_list.Count.ToString();
                    }));
                }));
                thread.Start();


                get_board_file_download(board_url_list[i].ToString()); //각 게시글 접속 
            }
            last_file_download();
            MessageBox.Show("완료");
        }

        public void get_board_file_download(String url) //게시글 파일 다운로드 url 까지 추출완료 / 파일명과 다운로더 모듈 연동하면 됨
        {

            var html = GetRequest(url);
            int i = 0;
            string tmp;

            Document doc = NSoupClient.Parse(html);

            Elements title = doc.Select("h1");

            
            string file_name = get_title_Ext(title.Text, HttpUtility.UrlEncode(Search_Text.Text, Encoding.UTF8));

            Elements file = doc.Select("article > div > p > a");


            if (file.ToString().Contains("해설")) //해설있는것
            {

                foreach (var item in file)
                {
                    String linkHref = item.Attr("href");
                    if (!linkHref.Contains("adobe.php?"))
                    {
                        if (!linkHref.Contains("force_download"))
                        {
                            if (!linkHref.Contains("javascript:;"))
                            {
                                if (item.ToString().Contains("해설"))
                                {
                                    if (!file_hash_table.ContainsKey(main_url + linkHref.ToString()))
                                    {
                                        desc_file_hash_table.Add(main_url + linkHref.ToString(), item.Text());
                                        desc_file_url_list.Add(main_url + linkHref.ToString());

                                        

                                    }

                                }

                            }
                        }
                    }

                    foreach (var item2 in file)
                    {
                        i++;
                        linkHref = item.Attr("href");
                        if (!linkHref.Contains("adobe.php?"))
                        {
                            if (!linkHref.Contains("force_download"))
                            {


                                if (!item.ToString().Contains("google.php"))
                                {
                                    if (!file_hash_table.ContainsKey(main_url + linkHref.ToString()))
                                    {
                                        tmp = file_name;
                                        tmp = tmp + "-" + item.Text();
                                        file_hash_table.Add(main_url + linkHref.ToString(), tmp.ToString());
                                        file_url_list.Add(main_url + linkHref.ToString());
                                    }
                                }
                                else
                                {
                                    string ans = Answer_Query(main_url + linkHref.ToString());

                                    if (!file_ans_hash_table.ContainsKey(ans.ToString()))
                                    {
                                        /*
                                        tmp = file_name;
                                        tmp = tmp + "-" + "정답지";
                                        file_hash_table.Add(linkHref.ToString(), tmp.ToString());
                                        file_url_list.Add(linkHref.ToString());
                                        */
                                        tmp = file_name;
                                        tmp = tmp + "-" + "정답지";
                                        file_ans_hash_table.Add(ans, tmp.ToString());
                                        file_ans_url_list.Add(ans.ToString());
                                       // MessageBox.Show(linkHref.ToString() + "\n\n" + ans);
                                    }
                                }

                            }
                        }
                    }
                }
            }
            else //해설 없는것
            {
               // Console.WriteLine(file.ToString() + " --- 해설없음");
      
                foreach (var item in file)
                {
                    i++;
                    String linkHref = item.Attr("href");
                    if (!linkHref.Contains("adobe.php?"))
                    {
                        if (!linkHref.Contains("force_download"))
                        {


                            if (!item.ToString().Contains("google.php"))
                            {
                                if (!file_hash_table.ContainsKey(main_url + linkHref.ToString()))
                                {
                                    tmp = file_name;
                                    tmp = tmp + "-" + item.Text();
                                    file_hash_table.Add(main_url + linkHref.ToString(), tmp.ToString());
                                    file_url_list.Add(main_url + linkHref.ToString());
                                }
                            }
                            else
                            {
                                string ans = Answer_Query(main_url + linkHref.ToString());
                                if (!file_ans_hash_table.ContainsKey(ans.ToString()))
                                {
                                    tmp = file_name;
                                    tmp = tmp + "-" + "정답지";
                                    file_ans_hash_table.Add(ans, tmp.ToString());
                                    file_ans_url_list.Add(ans.ToString());
                                   // MessageBox.Show(linkHref.ToString() +"\n\n" + ans);
                                }
                            }

                        }
                    }
                } 
            }

        }

        public void last_file_download() //전체 파일 다운로드 
        {
            string file_save_path = "";
            Thread thread = new Thread(new ThreadStart(delegate () // thread 생성
            {
                this.Invoke(new Action(delegate ()
                {

                    this.Text = "0gichul_Downloader - 3/3";
                    file_save_path = FileSavePath_Text.Text;
                }));
            }));
            thread.Start();
            // MessageBox.Show(file_hash_table.Count.ToString() + "==" + file_url_list.Count.ToString());

            DirectoryInfo di = new DirectoryInfo(FileSavePath_Text.Text + "\\기출지");
            if (!di.Exists)
            {
                di.Create();
            }

            di = new DirectoryInfo(FileSavePath_Text.Text + "\\정답지");
            if (!di.Exists)
            {
                di.Create();
            }
            di = new DirectoryInfo(FileSavePath_Text.Text + "\\해설지");
            if (!di.Exists)
            {
                di.Create();
            }

            thread = new Thread(new ThreadStart(delegate () // thread 생성
            {
                this.Invoke(new Action(delegate ()
                {

                    for (int i = 0; i < file_url_list.Count; i++) //기출지 다운로드
                    {
                        string file_url = file_url_list[i].ToString();
                        string file_name = file_hash_table[file_url].ToString();

                        Answer_Downloader(file_url, file_save_path + "\\기출지\\" + file_name + ".pdf");
                        // MessageBox.Show(file_url + "\n\n" + file_save_path + "\\기출지\\" + file_name + ".pdf");
                    }
                }));
            }));




            thread.Start();

            thread = new Thread(new ThreadStart(delegate () // thread 생성
            {
                this.Invoke(new Action(delegate ()
                {

                    for (int i = 0; i < file_ans_url_list.Count; i++) // 정답지 다운로드
                    {
                        string file_ans_url = file_ans_url_list[i].ToString();
                        string file_ans_name = file_ans_hash_table[file_ans_url].ToString();
                        Answer_Downloader2(file_ans_url, file_save_path + "\\정답지\\" + file_ans_name + ".pdf");
                        // MessageBox.Show(file_ans_url + "\n\n" + file_save_path + "\\기출지\\" + file_ans_name + ".pdf");
                        //MessageBox.Show(tmp);
                    }
                }));
            }));
            thread.Start();

            thread = new Thread(new ThreadStart(delegate () // thread 생성
            {
                this.Invoke(new Action(delegate ()
                {

                    for (int i = 0; i < desc_file_url_list.Count; i++) //해설지 다운로드
                    {
                        string desc_file_url = desc_file_url_list[i].ToString();
                        string desc_file_name = desc_file_hash_table[desc_file_url].ToString();
                        Answer_Downloader3(desc_file_url, file_save_path + "\\해설지\\" + desc_file_name + ".pdf");
                        //  MessageBox.Show(desc_file_url + "\n\n" + file_save_path + "\\기출지\\" + desc_file_name + ".pdf");
                        //MessageBox.Show(tmp);
                    }
                    
                }));
            }));
            thread.Start();



        }


        public string get_title_Ext(String html,string search_text)
        {
            string originalName = HttpUtility.UrlDecode(search_text, Encoding.UTF8);

            string title = html.Substring(0, html.IndexOf(originalName) + originalName.Length);

            return title.Substring(0, title.IndexOf(originalName) + originalName.Length);
//
            //Console.WriteLine(title.Substring(0, title.IndexOf(originalName) + originalName.Length));
        }


        private void button1_Click(object sender, EventArgs e)
        {
            board_url_list.Clear();
            file_url_list.Clear();
            file_ans_url_list.Clear();
            desc_file_url_list.Clear();
            file_hash_table.Clear();
            file_ans_hash_table.Clear();
            desc_file_hash_table.Clear();

            Thread thread0 = new Thread(() => Run(Search_Text.Text));

            thread0.Start();

        }
        public void Run(string Search_Text)
        {
            string search_text = HttpUtility.UrlEncode(Search_Text, Encoding.UTF8);
            int max_page = Page_Max_Count("https://0gichul.com/index.php?mid=recent&search_keyword=" + search_text + "&search_target=title&page=1");
            get_total_board_url_list("https://0gichul.com/index.php?mid=recent&search_keyword=" + search_text + "&search_target=title", max_page);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            string select_path = dialog.SelectedPath;
            FileSavePath_Text.Text = select_path;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }
    }
}
