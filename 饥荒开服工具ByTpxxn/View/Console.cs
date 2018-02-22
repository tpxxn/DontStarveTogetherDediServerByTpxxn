using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using 饥荒开服工具ByTpxxn.Class;
using 饥荒开服工具ByTpxxn.Class.JsonDeserialize;
using 饥荒开服工具ByTpxxn.Class.Tools;

namespace 饥荒开服工具ByTpxxn.View
{
    public partial class DedicatedServerPage : Page
    {
        /// <summary>
        /// 根据分类生产RadioButton
        /// </summary>
        private void CreateConsoleClassificationButton()
        {
            ConsoleClassificationStackPanel.Children.Clear();
            // otherRadioButton
            var otherRadioButton = new RadioButton
            {
                Content = "其他",
                Width = 140,
                Height = 40,
                Foreground = new SolidColorBrush(Colors.Black),
                FontWeight = FontWeights.Bold,
                Style = (Style)FindResource("RadioButtonStyle")
            };
            otherRadioButton.Checked += ConsoleRadioButton_Click;
            otherRadioButton.IsChecked = true;
            ConsoleClassificationStackPanel.Children.Add(otherRadioButton);
            // foodRadioButton
            var foodRadioButton = new RadioButton
            {
                Content = "食物",
                Width = 140,
                Height = 40,
                Foreground = new SolidColorBrush(Colors.Black),
                FontWeight = FontWeights.Bold,
                Style = (Style)FindResource("RadioButtonStyle")
            };
            foodRadioButton.Checked += ConsoleRadioButton_Click;
            foodRadioButton.IsChecked = false;
            ConsoleClassificationStackPanel.Children.Add(foodRadioButton);
            // resourcesRadioButton
            var resourcesRadioButton = new RadioButton
            {
                Content = "资源",
                Width = 140,
                Height = 40,
                Foreground = new SolidColorBrush(Colors.Black),
                FontWeight = FontWeights.Bold,
                Style = (Style)FindResource("RadioButtonStyle")
            };
            resourcesRadioButton.Checked += ConsoleRadioButton_Click;
            resourcesRadioButton.IsChecked = false;
            ConsoleClassificationStackPanel.Children.Add(resourcesRadioButton);
            // toolsRadioButton
            var toolsRadioButton = new RadioButton
            {
                Content = "工具",
                Width = 140,
                Height = 40,
                Foreground = new SolidColorBrush(Colors.Black),
                FontWeight = FontWeights.Bold,
                Style = (Style)FindResource("RadioButtonStyle")
            };
            toolsRadioButton.Checked += ConsoleRadioButton_Click;
            toolsRadioButton.IsChecked = false;
            ConsoleClassificationStackPanel.Children.Add(toolsRadioButton);
            // weaponsRadioButton
            var weaponsRadioButton = new RadioButton
            {
                Content = "武器",
                Width = 140,
                Height = 40,
                Foreground = new SolidColorBrush(Colors.Black),
                FontWeight = FontWeights.Bold,
                Style = (Style)FindResource("RadioButtonStyle")
            };
            weaponsRadioButton.Checked += ConsoleRadioButton_Click;
            weaponsRadioButton.IsChecked = false;
            ConsoleClassificationStackPanel.Children.Add(weaponsRadioButton);
            // giftsRadioButton
            var giftsRadioButton = new RadioButton
            {
                Content = "礼物",
                Width = 140,
                Height = 40,
                Foreground = new SolidColorBrush(Colors.Black),
                FontWeight = FontWeights.Bold,
                Style = (Style)FindResource("RadioButtonStyle")
            };
            giftsRadioButton.Checked += ConsoleRadioButton_Click;
            giftsRadioButton.IsChecked = false;
            ConsoleClassificationStackPanel.Children.Add(giftsRadioButton);
            // clothesRadioButton
            var clothesRadioButton = new RadioButton
            {
                Content = "衣物",
                Width = 140,
                Height = 40,
                Foreground = new SolidColorBrush(Colors.Black),
                FontWeight = FontWeights.Bold,
                Style = (Style)FindResource("RadioButtonStyle")
            };
            clothesRadioButton.Checked += ConsoleRadioButton_Click;
            clothesRadioButton.IsChecked = false;
            ConsoleClassificationStackPanel.Children.Add(clothesRadioButton);
        }

        /// <summary>
        /// 显示具体分类信息
        /// </summary>
        private void ConsoleRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ConsoleDetailsWrapPanel.Children.Clear();
            // 读取分类信息
            var itemList = JsonConvert.DeserializeObject<ItemListRootObject>(StringProcess.GetJsonString("ItemList.json"));
            // 把当前选择的值放到这里了
            ConsoleClassificationStackPanel.Tag = ((RadioButton)sender).Content;
            // 显示具体分类信息
            switch (ConsoleClassificationStackPanel.Tag)
            {
                case "其他":
                    foreach (var detail in itemList.Items.Other.Details)
                    {
                        if (string.IsNullOrEmpty(detail.Chinese))
                        {
                            continue;
                        }
                        CreateConsoleButton(detail);
                    }
                    break;
                case "食物":
                    foreach (var detail in itemList.Items.Food.Details)
                    {
                        if (string.IsNullOrEmpty(detail.Chinese))
                        {
                            continue;
                        }
                        CreateConsoleButton(detail);
                    }
                    break;
                case "资源":
                    foreach (var detail in itemList.Items.Resources.Details)
                    {
                        if (string.IsNullOrEmpty(detail.Chinese))
                        {
                            continue;
                        }
                        CreateConsoleButton(detail);
                    }
                    break;
                case "工具":
                    foreach (var detail in itemList.Items.Tools.Details)
                    {
                        if (string.IsNullOrEmpty(detail.Chinese))
                        {
                            continue;
                        }
                        CreateConsoleButton(detail);
                    }
                    break;
                case "武器":
                    foreach (var detail in itemList.Items.Weapons.Details)
                    {
                        if (string.IsNullOrEmpty(detail.Chinese))
                        {
                            continue;
                        }
                        CreateConsoleButton(detail);
                    }
                    break;
                case "礼物":
                    foreach (var detail in itemList.Items.Gifts.Details)
                    {
                        if (string.IsNullOrEmpty(detail.Chinese))
                        {
                            continue;
                        }
                        CreateConsoleButton(detail);
                    }
                    break;
                case "衣物":
                    foreach (var detail in itemList.Items.Clothes.Details)
                    {
                        if (string.IsNullOrEmpty(detail.Chinese))
                        {
                            continue;
                        }
                        CreateConsoleButton(detail);
                    }
                    break;
            }
        }

        /// <summary>
        /// 创建Console按钮
        /// </summary>
        /// <param name="detail"></param>
        private void CreateConsoleButton(Detail3 detail)
        {
            var codeString = detail.Code;
            var chineseString = detail.Chinese;
            // 按钮
            var button = new Button
            {
                Content = chineseString,
                Width = 115,
                Height = 35,
                Tag = codeString,
                FontWeight = FontWeights.Bold,
                Style = (Style)FindResource("DediBottomButtonStyle")
            };
            button.Click += ConsoleButton_Click;
            ConsoleDetailsWrapPanel.Children.Add(button);
        }

        /// <summary>
        /// Console按钮Click事件
        /// </summary>
        private void ConsoleButton_Click(object sender, RoutedEventArgs e)
        {
            var code = ((Button)sender).Tag.ToString();
            // 如果是其他分类,则直接运行code
            if (ConsoleClassificationStackPanel.Tag.ToString() == "其他")
            {
                SsendMessage(code);
                System.Windows.Forms.Clipboard.SetDataObject(code);
            }
            // 如果不是其他
            else
            {
                SsendMessage("c_give(\"" + code + "\", 1)");
                System.Windows.Forms.Clipboard.SetDataObject("c_give(\"" + code + "\", 1)");
            }
        }

        /// <summary>
        /// 发送“消息”
        /// </summary>
        /// <param name="messageStr">消息字符串</param>
        private static void SsendMessage(string messageStr)
        {
            var mySendMessage = new MySendMessage();
            // 得到句柄
            var pstr = Process.GetProcessesByName("dontstarve_dedicated_server_nullrenderer");
            // 根据句柄,发送消息
            foreach (var t in pstr)
            {
                mySendMessage.InputStr(t.MainWindowHandle, messageStr);
                mySendMessage.SendEnter(t.MainWindowHandle);
            }
        }
    }
}
