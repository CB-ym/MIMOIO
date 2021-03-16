using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MIMOIO.Tool;
using Newtonsoft.Json;

namespace MIMOIO
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 是否打开组包好的MP4文件
        /// </summary>
        bool Isss = false;
        bool IsUp = false, IsStop = false;
        int num001 = 0, ioi = 0, UID = 7, num_00001 = 0, num_0000002 = 0;
        string str, str_002 = "a0_000", v_i = "1.1.2";
        Conf json;
        StringReader sr;
        
        /// <summary>
        /// api相关接口
        /// </summary>
        Funtion_Api api = new Funtion_Api();

        //定时处理
        System.Timers.Timer timer_1 = new System.Timers.Timer();

        public MainWindow()
        {
            string strProcessName = Process.GetCurrentProcess().ProcessName;
            //检查进程是否已经启动，已经启动则退出程序。
            if (Process.GetProcessesByName(strProcessName).Length > 1)
            {
                Environment.Exit(0);
            }

            InitializeComponent();
            //程序居中
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //程序加载初始化
            api.IsMuLu("影视资源");
            api.IsMuLu("资源列表");
            //判断是否存在配置文件
            if (!api.IsWenJian("Conf.ini"))
            {
                api.ChuangJianWJ("Conf.ini");
            }

            timer_1.Interval = 10000;
            timer_1.Elapsed += new ElapsedEventHandler(Timer_1);
            timer_1.Enabled = false;
            
            ThreadPool.QueueUserWorkItem(new WaitCallback(Yu_JiaZai));
        }

        #region 自动检测影视资源加载进度
        private void Timer_1(object sender, ElapsedEventArgs e)
        {
            if (num_1 == num_00001)
            {
                num_0000002++;
                if (num_0000002 == 6)
                {
                    Process.Start("ChongZhi.exe");
                    Environment.Exit(0);
                }
            }
            else
            {
                num_0000002 = 0;
                num_00001 = num_1;
            }
        }
        #endregion

        #region 更新检测以及自动加载之前未加载完毕的影视资源
        private void Yu_JiaZai(object state)
        {
            //检测版本号更新
            string sss1 = api.Get("https://www.ym-o.cn/api/Conf.php");
            json = JsonConvert.DeserializeObject<Conf>(sss1);
            if (json.UID != UID)
            {
                IsUp = true;
                VC_001.Visibility = Visibility.Visible;
                GR_001.Visibility = Visibility.Visible;
                tilte_001.Content = "更新(Update)";
                TX_001.Text = "当前版本 - " + v_i + "\n发现新版本 - " + json.V_I + "\n\n更新内容：\n" + json.MSg;
            }
            //判断是的已经看过公告弹窗，看过则不做处理
            else if (api.Read("Conf.ini") != "已读")
            {
                api.Writer("Conf.ini", "已读");
                VC_001.Visibility = Visibility.Visible;
                GR_001.Visibility = Visibility.Visible;
                tilte_001.Content = "公告";
                TX_001.Text = "欢迎使用MIMOIO，里面的影视图片链接大部分已失效，但影视资源并未失效。请放心使用。\n此项目属于开源项目，可前往GitHub下载源代码和最新版程序(不过当前还未发布到GitHub上)\n\n\n\n\n\t开发者 - 小梦(编辑)";
            }

            if (api.IsWenJian("影视资源/mian.m3u8"))
            {
                string line;
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    VC_001.Visibility = Visibility.Visible;
                    //显示资源处理提示窗
                    bb_00.Visibility = Visibility.Visible;
                });

                line = api.Read("影视资源/uri.txt");
                string[] arr = Regex.Split(line, @"\$@@\$");
                line1 = arr[0];
                line2 = arr[1];
                line3 = arr[2];
                str = api.Get(arr[1]);
                sr = new StringReader(str);

                using (StringReader sr = new StringReader(str))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (Regex.IsMatch(line, @"\.ts"))
                        {
                            num001++;
                        }
                    }
                }

                num001 *= 2;
                timer_1.Enabled = true;
                /*线程池处理，实例化5个加载线程（想加快加载速度的，可以多添加几个加载线程）
                 * 理论上添加没限制，但不建议添加太多。毕竟访问的人不是一个两个，同一时间的时访问量大了，资源提供端那边的服务器也是有点压力的
                 */
                ThreadPool.QueueUserWorkItem(new WaitCallback(SS_001));
                ThreadPool.QueueUserWorkItem(new WaitCallback(SS_002));
                ThreadPool.QueueUserWorkItem(new WaitCallback(SS_003));
                ThreadPool.QueueUserWorkItem(new WaitCallback(SS_004));
                ThreadPool.QueueUserWorkItem(new WaitCallback(SS_005));
            }
        }
        #endregion

        #region 影视列表加载
        /// <summary>
        /// 添加影视列表
        /// </summary>
        /// <param name="uri">图片链接</param>
        /// <param name="name">影视名</param>
        /// <param name="id">影视ID</param>
        private void Lise(string uri, string name, int id)
        {
            Grid grid = new Grid();
            Border border = new Border();
            StackPanel stack = new StackPanel();
            Image image = new Image();
            Label label = new Label();
            Label label1 = new Label();
            TextBlock text = new TextBlock();
            Image image1 = new Image();
            Image image2 = new Image();

            border.BorderThickness = new Thickness(0, 0, 0, 1);
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(30, 124, 236));
            border.Margin = new Thickness(8, 0, 8, 0);

            stack.Orientation = Orientation.Horizontal;
            stack.Margin = new Thickness(10, 10, 0, 5);

            image.Width = 135;
            image.Height = 192.5;
            image.ToolTip = name;
            image.Stretch = Stretch.UniformToFill;
            image.HorizontalAlignment = HorizontalAlignment.Left;
            image.VerticalAlignment = VerticalAlignment.Top;
            image.Source = new BitmapImage(new Uri("Images/eorror.jpg", UriKind.Relative));
            image.Clip = new RectangleGeometry(new Rect(0, 0, 135, 192.5), 5, 5);

            label.Content = name;
            label.VerticalAlignment = VerticalAlignment.Top;
            label.FontSize = 18;
            label.FontFamily = new FontFamily("MS Reference Sans Serif");
            label.Margin = new Thickness(5, 0, 0, 0);
            label.FontWeight = FontWeights.Bold;

            label1.HorizontalAlignment = HorizontalAlignment.Left;
            label1.VerticalAlignment = VerticalAlignment.Top;
            label1.Margin = new Thickness(152, 40, 12, 0);

            text.Text = "简介：简介是什么？看图就对了。图片没加载出来？那就看影视名吧，这总加载出来了的吧~";
            text.FontSize = 16;
            text.Padding = new Thickness(0);
            text.Height = 100;
            text.Foreground = new SolidColorBrush(Color.FromRgb(95, 95, 95));
            text.TextWrapping = TextWrapping.Wrap;

            image1.Name = "DOW_" + id;
            image1.Source = new BitmapImage(new Uri("Images/M_003.png", UriKind.Relative));
            image1.Height = 32;
            image1.VerticalAlignment = VerticalAlignment.Bottom;
            image1.HorizontalAlignment = HorizontalAlignment.Right;
            image1.Margin = new Thickness(0, 0, 75, 6);
            image1.ToolTip = "超喵播放";
            image1.MouseLeftButtonUp += Player_Button;

            image2.Name = "DOW_" + id;
            image2.Source = new BitmapImage(new Uri("Images/Dowen.png", UriKind.Relative));
            image2.Height =28;
            image2.VerticalAlignment = VerticalAlignment.Bottom;
            image2.HorizontalAlignment = HorizontalAlignment.Right;
            image2.Margin = new Thickness(0, 0, 30, 8);
            image2.ToolTip = "极速下载";
            image2.MouseLeftButtonUp += Download_Button;

            lb.Children.Add(grid);
            grid.Children.Add(border);
            grid.Children.Add(stack);
            grid.Children.Add(label1);
            grid.Children.Add(image1);
            grid.Children.Add(image2);
            stack.Children.Add(image);
            stack.Children.Add(label);
            label1.Content = text;
        }
        #endregion

        #region 集数加载与点击播放
        private void JiShuJiaZai(string name, string name1, string uri, string uri1)
        {
            ioi++;
            Grid grid = new Grid();
            Grid grid1 = new Grid();
            Grid grid2 = new Grid();
            Label label = new Label();
            Label label1 = new Label();

            grid.Margin = new Thickness(0, 15, 0, 0);

            grid1.Height = 30;
            grid1.Width = 130;
            grid1.ToolTip = uri;
            grid1.Margin = new Thickness(18, 0, 0, 0);
            grid1.MouseLeftButtonUp += BF_JiaZai;
            grid1.Name = "a0_" + ioi.ToString();
            RegisterName("a0_" + ioi.ToString(), grid1);
            grid1.HorizontalAlignment = HorizontalAlignment.Left;
            grid1.Background = new SolidColorBrush(Color.FromRgb(22, 211, 230));
            grid1.Clip = new RectangleGeometry(new Rect(0, 0, 130, 30), 4, 4);

            RegisterName("label_" + ioi.ToString(), label);
            label.Content = name;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            grid2.Height = 30;
            grid2.Width = 130;
            grid2.ToolTip = uri1;
            grid2.Margin = new Thickness(0, 0, 18, 0);

            //如果第二个为空，则不做名称注册和方法注册处理
            if (uri1 != "空的")
            {
                ioi++;
                grid2.Name = "a0_" + ioi.ToString();
                grid2.MouseLeftButtonUp += BF_JiaZai;
                RegisterName("a0_" + ioi.ToString(), grid2);
                RegisterName("label_" + ioi.ToString(), label1);
            }
            grid2.HorizontalAlignment = HorizontalAlignment.Right;
            grid2.Background = new SolidColorBrush(Color.FromRgb(22, 211, 230));
            grid2.Clip = new RectangleGeometry(new Rect(0, 0, 130, 30), 4, 4);

            label1.Content = name1;
            label1.HorizontalAlignment = HorizontalAlignment.Center;
            label1.VerticalAlignment = VerticalAlignment.Center;
            label1.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            lb1.Children.Add(grid);
            grid.Children.Add(grid1);
            grid1.Children.Add(label);
            grid.Children.Add(grid2);
            grid2.Children.Add(label1);
        }

        private void BF_JiaZai(object sender, MouseButtonEventArgs e)
        {
            Isss = false;
            IsStop = false;
            Grid grid = sender as Grid;
            //把当前加载的集数放在公共变量str_002上（后面要用到）
            str_002 = grid.Name;
            string[] s = Regex.Split(str_002, "_");
            Label label = FindName("label_" + s[1].ToString()) as Label;
            if (label != null)
            {
                line3 = label.Content.ToString();
            }
            //播放资源加载
            ThreadPool.QueueUserWorkItem(new WaitCallback(Player_jiazai), grid.ToolTip.ToString());
            //隐藏集数显示框
            scrolls1.Visibility = Visibility.Collapsed;
            //显示资源处理提示窗
            bb_00.Visibility = Visibility.Visible;
            //删除所有集数列表
            lb1.Children.Clear();
        }

        int num_1 = 0;
        string line1 = "", line2, line3 = "";
        bool b1 = false, b2 = false, b3 = false, b4 = false, b5 = false;
        private void Player_jiazai(object state)
        {
            string line;
            str = api.Get(state.ToString());
            using (StringReader sr = new StringReader(str))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (Regex.IsMatch(line, @"\.m3u8"))
                    {
                        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                        {
                            BB_01.Content = "二次解析资源中...";
                        });
                        if (Regex.IsMatch(line, @"^(http|https)"))
                        {
                            str = api.Get(line);
                            if (str != "error")
                            {
                                string[] arr1 = Regex.Split(line, "/");
                                for (int i = 0; i < arr1.Length - 1; i++)
                                {
                                    line1 += arr1[i] + "/";
                                }
                                api.Writer("影视资源/mian.m3u8", str);
                            }
                        }
                        else
                        {
                            string[] arr1 = Regex.Split(state.ToString(), "/");
                            for (int i = 0; i < arr1.Length - 1; i++)
                            {
                                line1 += arr1[i] + "/";
                            }

                            arr1 = Regex.Split(line, "/");
                            if (Regex.IsMatch(line, "^/"))
                            {
                                for (int i = 1; i < arr1.Length - 1; i++)
                                {
                                    line1 += arr1[i] + "/";
                                }
                            }
                            else
                            {
                                for (int i = 0; i < arr1.Length - 1; i++)
                                {
                                    line1 += arr1[i] + "/";
                                }
                            }

                            line2 = line1 + arr1[arr1.Length - 1];
                            str = api.Get(line2);
                            if (str != "error")
                            {
                                api.Writer("影视资源/mian.m3u8", str);
                            }
                        }
                    }
                    else if (Regex.IsMatch(line, @"\.ts"))
                    {
                        api.Writer("影视资源/mian.m3u8", str);
                        line2 = state.ToString();
                        string[] arr2 = Regex.Split(state.ToString(), "/");
                        for (int i = 0; i < arr2.Length - 1; i++)
                        {
                            line1 += arr2[i] + "/";
                        }
                        break;
                    }
                }
            }

            if (str != "error")
            {
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    BB_01.Content = "开始读取M3U8文件...";
                });

                num001 = 0;
                using (StringReader sr = new StringReader(str))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (Regex.IsMatch(line, @"\.ts"))
                        {
                            num001++;
                        }
                    }
                }

                if (num001 == 0)
                {
                    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        BB_01.Content = "资源加载失败，请更换播放资源";
                    });
                    Thread.Sleep(3000);
                    line1 = "";
                    line2 = "";
                    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        BB_01.Content = "加载资源中...";
                        VC_001.Visibility = Visibility.Collapsed;
                        bb_00.Visibility = Visibility.Collapsed;
                    });
                }
                else
                {
                    b1 = false; b2 = false; b3 = false; b4 = false; b5 = false;
                    num_1 = 0;
                    num001 *= 2;
                    timer_1.Enabled = true;
                    api.Writer("影视资源/uri.txt", line1 + "$@@$" + line2 + "$@@$" + line3);
                    sr = new StringReader(str);
                    /*线程池处理，实例化5个加载线程（想加快加载速度的，可以多添加几个加载线程）
                     * 理论上添加没限制，但不建议添加太多。毕竟访问的人不是一个两个，同一时间的时访问量大了，资源提供端那边的服务器也是有点压力的
                     */
                    ThreadPool.QueueUserWorkItem(new WaitCallback(SS_001));
                    ThreadPool.QueueUserWorkItem(new WaitCallback(SS_002));
                    ThreadPool.QueueUserWorkItem(new WaitCallback(SS_003));
                    ThreadPool.QueueUserWorkItem(new WaitCallback(SS_004));
                    ThreadPool.QueueUserWorkItem(new WaitCallback(SS_005));
                }
            }
            else{
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    BB_01.Content = "资源加载失败，请更换播放资源";
                });
                Thread.Sleep(3000);
                line1 = "";
                line2 = "";
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    BB_01.Content = "加载资源中...";
                    VC_001.Visibility = Visibility.Collapsed;
                    bb_00.Visibility = Visibility.Collapsed;
                });
            }
        }
        #endregion

        #region 下载池
        private void SS_005(object state)
        {
            string line = "";

            Ts_00(line);
            b5 = true;
            IS_005(line);
        }

        private void SS_004(object state)
        {
            string line = "";

            Ts_00(line);
            b4 = true;
            IS_005(line);
        }

        private void SS_003(object state)
        {
            string line = "";

            Ts_00(line);
            b3 = true;
            IS_005(line);
        }

        private void SS_002(object state)
        {
            string line = "";

            Ts_00(line);
            b2 = true;
            IS_005(line);
        }

        private void SS_001(object state)
        {
            string line = "";

            Ts_00(line);
            b1 = true;
            IS_005(line);
        }
        #endregion

        #region 组包ts
        private void Ts_00(string line)
        {
            string line44 = "error";
            while ((line = sr.ReadLine()) != null)
            {
                if (IsStop)
                {
                    break;
                }

                num_1++;
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    BB_01.Content = "加载第" + num_1 + "个ts中，预计有" + num001 + "个ts";
                });
                if (Regex.IsMatch(line, @"\.ts") && !api.IsWenJian("影视资源/" + line))
                {
                    while (line44 == "error")
                    {
                        if (Regex.IsMatch(line, @"(http|https)"))
                        {
                            string[] arr2 = Regex.Split(line, @"/");
                            line44 = api.DownloadFile(line, "影视资源/" + arr2[arr2.Length - 1]);
                        }
                        else
                        {
                            line44 = api.DownloadFile(line1 + line, "影视资源/" + line);
                        }
                    }
                    line44 = "error";
                }
            }
        }
        #endregion

        #region 线程池处理完毕等相关操作
        private void IS_005(string line)
        {
            //当5条线程都完成时才执行下方的代码
            if (b1 && b2 && b3 && b4 && b5)
            {
                if (IsStop)
                {
                    timer_1.Enabled = false;

                    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        BB_01.Content = "加载资源中...";
                        VC_001.Visibility = Visibility.Collapsed;
                        bb_00.Visibility = Visibility.Collapsed;
                    });
                    line1 = "";
                    line2 = "";

                    FileInfo[] flie1 = api.File_All("资源列表");
                    //判断资源列表文件夹里是否存在相同的文件名
                    foreach (FileInfo file in flie1)
                    {
                        api.DeleteFile("影视资源/" + file.Name);
                    }
                }
                else
                {
                    M8ZM4("影视资源/mian.m3u8", "资源列表/" + line3 + ".mp4");
                }
            }
        }
        #endregion

        #region 播放按钮相关
        private void Player_Button(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            string[] arr1 = Regex.Split(image.Name, "_");
            //线程池加载集数处理
            ThreadPool.QueueUserWorkItem(new WaitCallback(Player_BF), arr1[1]);
        }

        private void Player_BF(object state)
        {
            //获取当前播放资源相关的集数
            str = api.Get("https://www.ym-o.cn/api/MIMOIO_ZY.php?name=" + state.ToString());

            if (str == "error" || str == "失败")
            {
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    MIM_001.Content = "服务器端出现问题，稍后再试";
                });
            }
            else if (str != "不存在")
            {
                string[] arr1 = Regex.Split(str, @"\$\$\$");
                foreach (string s in arr1)
                {
                    string[] arr2 = Regex.Split(s, @"\$\$");
                    if (Regex.IsMatch(arr2[0], "m3u8"))
                    {
                        //当来自不同的播放源的时候，ioi中间隔2位数
                        ioi += 3;
                        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                        {
                            //播放来源标题加载
                            Title_Name(arr2[0]);
                        });
                        string[] arr3 = Regex.Split(arr2[1], "#");
                        for (int i = 0; i < arr3.Length; i = i + 2)
                        {
                            string[] arr4 = Regex.Split(arr3[i], @"\$");
                            if (i + 1 >= arr3.Length)
                            {
                                str = "没有了~$空的";
                            }
                            else
                            {
                                str = arr3[i + 1];
                            }
                            string[] arr5 = Regex.Split(str, @"\$");
                            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                            {
                                scrolls1.Visibility = Visibility.Visible;
                                JiShuJiaZai(arr4[0], arr5[0], arr4[1], arr5[1]);
                            });
                        }
                    }
                }
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    //打开屏蔽层
                    VC_001.Visibility = Visibility.Visible;
                });
            }
        }
        #endregion

        #region 公告了解按钮
        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GR_001.Visibility = Visibility.Collapsed;
            VC_001.Visibility = Visibility.Collapsed;

            if (IsUp)
            {
                Process.Start("https://github.com/CB-ym/MIMOIO/releases");
                if (json.UID - 3 >= UID)
                {
                    MessageBox.Show("版本过低，请更新后再使用");
                    Environment.Exit(0);
                }
            }
        }
        #endregion

        #region 关闭加载资源
        private void Image_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            IsStop = true;
        }
        #endregion

        #region 播放来源加载
        private void Title_Name(string s)
        {
            Label label = new Label();
            label.Content = s;
            label.HorizontalContentAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Top;
            label.Margin = new Thickness(0, 15, 0, 0);
            label.Padding = new Thickness(0, 5, 0, 2);
            label.FontSize = 18;
            label.FontFamily = new FontFamily("Microsoft YaHei UI");
            label.Width = 120;
            label.BorderThickness = new Thickness(0, 0, 0, 2);
            label.BorderBrush = new SolidColorBrush(Color.FromRgb(32, 170, 222));
            lb1.Children.Add(label);
        }
        #endregion

        private void Download_Button(object sender, MouseButtonEventArgs e)
        {
            //待加入。
        }

        #region m3u8转mp4
        private void M8ZM4(string m8, string m4)
        {
            FileInfo[] flie1 = api.File_All("资源列表");
            //判断资源列表文件夹里是否存在相同的文件名
            foreach (FileInfo file in flie1)
            {
                if (file.Name == m4)
                {
                    //存在则删除
                    api.DeleteFile("资源列表/" + m4);
                }
            }

            //这里我启用了外部的ffmpeg程序进行了M3U8转MP4
            ProcessStartInfo ps = new ProcessStartInfo();
            ps.CreateNoWindow = true;
            ps.WindowStyle = ProcessWindowStyle.Hidden;
            ps.FileName = "ffmpeg.exe";
            ps.Arguments = "-i " + m8 + " -c copy " + m4;

            Process proc = new Process();
            proc.StartInfo = ps;
            proc.Start();
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
            {
                BB_01.Content = "组包MP4中...";
            });
            //等待转换完成再执行下面的代码
            proc.WaitForExit();
            proc.Close();
            proc.Dispose();

            FileInfo[] flie3 = api.File_All("影视资源");
            //判断资源列表文件夹里是否存在相同的文件名
            foreach (FileInfo file4 in flie3)
            {
                api.DeleteFile("影视资源/" + file4.Name);
            }

            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
            {
                BB_01.Content = "加载资源中...";
                VC_001.Visibility = Visibility.Collapsed;
                bb_00.Visibility = Visibility.Collapsed;
            });
            line1 = "";
            line2 = "";

            string[] s = Regex.Split(str_002, "_");
            int n = int.Parse(s[1]) + 1;
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
            {
                Grid grid = FindName("a0_" + n.ToString()) as Grid;
                //判断下一集是否存在，存在则继续加载下一集的资源
                if (grid != null)
                {
                    str_002 = grid.Name;
                    //线程池处理，插件初始加载
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Player_jiazai), grid.ToolTip.ToString());
                    bb_00.Visibility = Visibility.Visible;
                    VC_001.Visibility = Visibility.Visible;
                }
                else
                {
                    timer_1.Enabled = false;
                }
            });

            //判断是否打开组包mp4后的文件位置
            if (!Isss)
            {
                Isss = true;
                Process p = new Process();
                p.StartInfo.FileName = "explorer.exe";
                p.StartInfo.Arguments = @"资源列表\";
                p.Start();
            }
        }
        #endregion

        #region 点击关闭集数列表
        private void VC_001_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //只有当不是在资源加载中和公告弹窗的时候才处理
            if (bb_00.Visibility == Visibility.Collapsed && GR_001.Visibility == Visibility.Collapsed)
            {
                lb1.Children.Clear();
                scrolls1.Visibility = Visibility.Collapsed;
                VC_001.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region 搜索框变化相关
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Msg_New.Text == "")
            {
                Msg_Str.Content = "搜索点什么？";
                Msg_Str.Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            }
            else
            {
                Msg_Str.Content = "";
                Msg_New.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }
        #endregion

        #region 搜索资源
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //影视资源列表加载线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(Liebiao));
        }

        private void Liebiao(object state)
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
            {
                MIM_001.Visibility = Visibility.Visible;
                str = Msg_New.Text;
                MIM_001.Content = "搜索资源中...";
                lb.Children.Clear();
            });

            if (str.Length < 2)
            {
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    MIM_001.Content = "什么玩意，小心程序崩溃";
                });
            }
            else
            {
                //搜索相关资源
                str = api.Get("https://www.ym-o.cn/api/MIMOIO.php?name=" + str);
                if (str != "不存在")
                {
                    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        if (str == "失败" || str == "error")
                        {
                            MIM_001.Content = "服务器端出现问题，稍后再试";
                        }
                        else
                        {
                            string[] arr1 = Regex.Split(str, "@@");
                            foreach (string s in arr1)
                            {
                                if (s != "不存在")
                                {
                                    string[] arr2 = Regex.Split(s, @"\)\)");
                                    Lise(arr2[0], arr2[1], Convert.ToInt32(arr2[2]));
                                }
                            }
                        }
                        MIM_001.Visibility = Visibility.Collapsed;
                    });
                }
                else
                {
                    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        Msg_New.Text = "";
                        MIM_001.Content = "没有找到相关的资源";
                        Msg_Str.Content = "找不到？试试关键字";
                    });
                }
            }
        }

        private void Msg_New_KeyDown(object sender, KeyEventArgs e)
        {
            //按下回车键后处理消息
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                //线程池处理，插件初始加载
                ThreadPool.QueueUserWorkItem(new WaitCallback(Liebiao));
            }
        }
        #endregion

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            scrolls1.Visibility = Visibility.Collapsed;
        }
    }
}
