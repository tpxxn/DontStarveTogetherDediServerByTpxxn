using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using 饥荒开服工具ByTpxxn.Class;
using 饥荒开服工具ByTpxxn.Class.JsonDeserialize;

namespace 饥荒开服工具ByTpxxn
{
    public class App : Application
    {
        /// <summary>
        /// Interaction logic for App.xaml
        /// </summary>
        private class AppRun : Application
        {
            public AppRun()
            {
                // TODO
                //Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
                //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Startup += App_Startup;
            }

            private static void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
            {
                UnhandledExceptionFileLog(e.Exception.ToString());
                e.Handled = true;//使用这一行代码告诉运行时，该异常被处理了，不再作为UnhandledException抛出了。
            }

            private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                UnhandledExceptionFileLog(e.ExceptionObject.ToString());
            }

            private static void UnhandledExceptionFileLog(string log)
            {
                try
                {
                    var logFilePath = Environment.CurrentDirectory + @"\Log\"; //设置文件夹位置
                    if (Directory.Exists(logFilePath) == false) //若文件夹不存在
                    {
                        Directory.CreateDirectory(logFilePath);
                    }
                    var logFilename = "dedicatedserverslog_" + DateTime.Now.ToString("yyyyMMdd") + "_" +
                        DateTime.Now.ToString("hhmmss") + ".txt"; //设置文件名
                    var logPath = logFilePath + logFilename;
                    if (!File.Exists(logPath))
                    {
                        var fs = File.Create(logPath);
                        fs.Close();
                    }
                    var fileStream = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    var streamWriter = new StreamWriter(fileStream);
                    streamWriter.WriteLine("错误信息：\r\n" + log);
                    streamWriter.Flush();
                    streamWriter.Close();
                    fileStream.Close();
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            private static void App_Startup(object sender, StartupEventArgs e)
            {
                #region 设置全局字体
                var mainWindowFont = IniFileIo.IniFileReadString("DedicatedServerConfigure", "Font", "FontFamily");
                Global.FontFamily = !string.IsNullOrEmpty(mainWindowFont) ? new FontFamily(mainWindowFont) : new FontFamily("微软雅黑");
                #endregion

                #region 淡紫色透明光标
                var mainWindowLavenderCursor = IniFileIo.IniFileReadString("DedicatedServerConfigure", "Others", "LavenderCursor");
                if (string.IsNullOrEmpty(mainWindowLavenderCursor))
                {
                    mainWindowLavenderCursor = "True";
                    IniFileIo.IniFileWrite("DedicatedServerConfigure", "Others", "LavenderCursor", "True");
                }
                ResourceDictionary CursorDictionary;
                if (mainWindowLavenderCursor == "True")
                {
                    CursorDictionary = new ResourceDictionary
                    {
                        Source = new Uri(
                            "pack://application:,,,/饥荒开服工具ByTpxxn;component/Dictionary/Cursor/CursorDictionary.xaml",
                            UriKind.Absolute)
                    };
                }
                else
                {
                    CursorDictionary = new ResourceDictionary
                    {
                        Source = new Uri(
                            "pack://application:,,,/饥荒开服工具ByTpxxn;component/Dictionary/Cursor/DefaultCursorDictionary.xaml",
                            UriKind.Absolute)
                    };
                    Current.Resources.MergedDictionaries.Add(CursorDictionary);
                }
                foreach (var key in CursorDictionary.Keys)
                {
                    var cursor = ((TextBlock)CursorDictionary[key]).Cursor;
                    CursorDictionary.Remove(key);
                    CursorDictionary.Add(key, cursor);
                }
                Current.Resources.MergedDictionaries.Add(CursorDictionary);
                #endregion

                #region 读取资源字典
                var resourceDictionaries = new Collection<ResourceDictionary>
                {
                    new ResourceDictionary
                    {
                        Source = new Uri(
                            "pack://application:,,,/饥荒开服工具ByTpxxn;component/Dictionary/ComboBoxDictionary.xaml",
                            UriKind.Absolute)
                    },
                    new ResourceDictionary
                    {
                        Source = new Uri(
                            "pack://application:,,,/饥荒开服工具ByTpxxn;component/Dictionary/WindowDictionary.xaml",
                            UriKind.Absolute)
                    },
                    new ResourceDictionary
                    {
                        Source = new Uri(
                            "pack://application:,,,/饥荒开服工具ByTpxxn;component/Dictionary/DediLeftPanelRadioButtonDictionary.xaml",
                            UriKind.Absolute)
                    },
                    new ResourceDictionary
                    {
                        Source = new Uri(
                            "pack://application:,,,/饥荒开服工具ByTpxxn;component/Dictionary/DediModBoxDictionary.xaml",
                            UriKind.Absolute)
                    },
                    new ResourceDictionary
                    {
                        Source = new Uri(
                            "pack://application:,,,/饥荒开服工具ByTpxxn;component/Dictionary/DediRightPanelButtonDictionary.xaml",
                            UriKind.Absolute)
                    },
                    new ResourceDictionary
                    {
                        Source = new Uri(
                            "pack://application:,,,/饥荒开服工具ByTpxxn;component/Dictionary/DediRightPanelRadioButtonDictionary.xaml",
                            UriKind.Absolute)
                    },
                    new ResourceDictionary
                    {
                        Source = new Uri(
                            "pack://application:,,,/饥荒开服工具ByTpxxn;component/Dictionary/DediRightPanelTextBoxDictionary.xaml",
                            UriKind.Absolute)
                    },
                    new ResourceDictionary
                    {
                        Source = new Uri(
                            "pack://application:,,,/饥荒开服工具ByTpxxn;component/Dictionary/DediScrollViewerDictionary.xaml",
                            UriKind.Absolute)
                    },
                    new ResourceDictionary
                    {
                        Source = new Uri(
                            "pack://application:,,,/饥荒开服工具ByTpxxn;component/Dictionary/DediSelectBoxDictionary.xaml",
                            UriKind.Absolute)
                    }
                };
                foreach (var resourceDictionary in resourceDictionaries)
                {
                    Current.Resources.MergedDictionaries.Add(resourceDictionary);
                }
                #endregion

                var mainWindowShow = new MainWindow();
                mainWindowShow.InitializeComponent();
                mainWindowShow.Show();
                mainWindowShow.Activate();
            }
        }

        /// <summary>
        /// Entry point class to handle single instance of the application
        /// </summary>
        public static Semaphore SingleInstanceWatcher { get; private set; }

        private static bool _createdNew;

        public static class EntryPoint
        {
            [STAThread]
            public static void Main(string[] args)
            {
                if (args.Length == 0)
                {
                    // 工程名("饥荒开服工具ByTpxxn")
                    var projectName = Assembly.GetExecutingAssembly().GetName().Name;
                    // 单实例监视
                    SingleInstanceWatcher = new Semaphore(0, 1, projectName, out _createdNew);
                    if (_createdNew)
                    {
                        //启动
                        var app = new AppRun();
                        app.Run();
                    }
                    else
                    {
                        MessageBox.Show("请不要重复运行(ノ｀Д)ノ");
                        Environment.Exit(-2);
                    }
                }
                //else
                //{
                //    if (args[0].ToLower() == "-clear")
                //    {
                //        if (MessageBox.Show("警告：您将会删除所有注册表设置，点“确定”立即清除，点“取消”取消清除！", "Σ(っ°Д°;)っ", MessageBoxButton.OKCancel) ==
                //            MessageBoxResult.OK)
                //        {
                //            RegeditRw.ClearReg();
                //            MessageBox.Show("清除完毕！", "ヾ(๑╹◡╹)ﾉ");
                //        }
                //        else
                //        {
                //            MessageBox.Show("取消清除！", "(～￣▽￣)～");
                //        }
                //        Environment.Exit(0);
                //    }
                //}
            }
        }
    }
}
