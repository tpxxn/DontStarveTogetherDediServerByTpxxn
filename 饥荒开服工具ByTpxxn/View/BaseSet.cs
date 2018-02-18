using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 饥荒开服工具ByTpxxn.Class.DedicateServer;

namespace 饥荒开服工具ByTpxxn.View
{
    public partial class DedicatedServerPage : Page
    {

        #region "游戏风格"

        private void DediIntention_Click(object sender, RoutedEventArgs e)
        {
            PanelVisibility("Null");
            DediBaseSet.Visibility = Visibility.Visible;
            switch (((Button)sender).Name)
            {
                case "IntentionSocialButton":
                    DediBaseSetIntentionButton.Content = "交际";
                    break;
                case "IntentionCooperativeButton":
                    DediBaseSetIntentionButton.Content = "合作";
                    break;
                case "IntentionCompetitiveButton":
                    DediBaseSetIntentionButton.Content = "竞争";
                    break;
                case "IntentionMadnessButton":
                    DediBaseSetIntentionButton.Content = "疯狂";
                    break;
            }
        }

        private void DediIntention_MouseEnter(object sender, MouseEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "IntentionSocialButton":
                    DidiIntentionTextBlock.Text = "这是一个闲聊&扯蛋的地方。\r\n轻松的游戏风格，只是为了互相沟通&扯蛋。\r\n还等什么，快进来一起扯蛋吧~~";
                    break;
                case "IntentionCooperativeButton":
                    DidiIntentionTextBlock.Text = "一个团队生存的世界。在这个世界，我们要一起合作，尽我们可能来驯服这个充满敌意的世界。";
                    break;
                case "IntentionCompetitiveButton":
                    DidiIntentionTextBlock.Text = "这是一个完美的舞台。\r\n展示你的生存能力，战斗能力、建设能力...吧！";
                    break;
                case "IntentionMadnessButton":
                    DidiIntentionTextBlock.Text = "在这里，你将过着茹毛饮血的生活！\r\n是你吃掉粮食还是被粮食吃掉呢？\r\n让我们拭目以待吧！";
                    break;
            }
        }

        private void DediIntention_MouseLeave(object sender, MouseEventArgs e)
        {
            DidiIntentionTextBlock.Text = "";
        }
        #endregion

        #region "基本设置面板"
        /// <summary>
        /// 修改房间名时顶部显示房间名和左侧显示房间名同步修改
        /// </summary>
        private void DediBaseSetHouseName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //DediMainTopWorldName.Text = DediBaseSetHouseName.Text; TODO
            if (((RadioButton)SaveSlotStackPanel.FindName("SaveSlotRadioButton" + SaveSlot))?.IsChecked == true)
            {
                // ReSharper disable once PossibleNullReferenceException
                ((RadioButton)SaveSlotStackPanel.FindName($"SaveSlotRadioButton{SaveSlot}")).Content = DediBaseSetHouseName.Text;
            }
        }

        /// <summary>
        /// 选择游戏风格
        /// </summary>
        private void DediBaseSetIntentionButton_Click(object sender, RoutedEventArgs e)
        {
            PanelVisibility("Null");
            DediIntention.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 打开游戏
        /// </summary>
        private void OpenGameButton_Click(object sender, RoutedEventArgs e)
        {
            RunClient();
        }

        /// <summary>
        /// 创建世界按钮
        /// </summary>
        private void CtrateWorldButton_Click(object sender, RoutedEventArgs e)
        {
            RunServer();
        }

        /// <summary>
        /// 读取并设定基本设置
        /// </summary>
        private void SetBaseSet()
        {
            var clusterIniFilePath = _pathFile.ServerDirPath + @"\cluster.ini";
            if (!File.Exists(clusterIniFilePath))
            {
                //MessageBox.Show("cluster.ini不存在");
                return;
            }
            _baseSet = new BaseSet(clusterIniFilePath);

            BaseSetGameModeSelectBox.DataContext = _baseSet;
            BaseSetPvpSelectBox.DataContext = _baseSet;
            BaseSetMaxPlayerSelectBox.DataContext = _baseSet;
            BaseSetOfflineSelectBox.DataContext = _baseSet;
            DediBaseSetHouseName.DataContext = _baseSet;
            DediBaseSetDescribe.DataContext = _baseSet;
            DediBaseSetSecret.DataContext = _baseSet;
            BaseSetIsPauseSelectBox.DataContext = _baseSet;
            DediBaseSetIntentionButton.DataContext = _baseSet;
            EditWorldIsCaveSelectBox.DataContext = _baseSet;
            Debug.WriteLine("基本设置-完");
        }

        #endregion

    }
}
