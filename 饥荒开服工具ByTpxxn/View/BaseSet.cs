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

        #region 游戏风格[Intention]

        /// <summary>
        /// 进入选择游戏风格界面
        /// </summary>
        private void DediBaseSetIntentionButton_Click(object sender, RoutedEventArgs e)
        {
            PanelVisibility("Null");
            DediIntention.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 选择游戏风格
        /// </summary>
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
        
        /// <summary>
        /// 显示游戏风格描述
        /// </summary>
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

        /// <summary>
        /// 隐藏游戏风格描述
        /// </summary>
        private void DediIntention_MouseLeave(object sender, MouseEventArgs e)
        {
            DidiIntentionTextBlock.Text = "";
        }
        #endregion

        #region 基本设置面板[基本设置]

        /// <summary>
        /// 修改ClusterName时左侧SaveSlot按钮同步显示ClusterName
        /// </summary>
        private void DediBaseSetClusterName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //DediMainTopWorldName.Text = DediBaseSetClusterName.Text; TODO 左侧SaveSlot按钮ClusterName
            if (((RadioButton)SaveSlotStackPanel.FindName("SaveSlotRadioButton" + SaveSlot))?.IsChecked == true)
            {
                // ReSharper disable once PossibleNullReferenceException
                ((RadioButton)SaveSlotStackPanel.FindName($"SaveSlotRadioButton{SaveSlot}")).Content = DediBaseSetClusterName.Text;
            }
        }

        /// <summary>
        /// 读取并设定基本设置
        /// </summary>
        private void SetBaseSet()
        {
            var clusterIniFilePath = _dediFilePath.ClusterFilePath;
            if (!File.Exists(clusterIniFilePath))
            {
                Debug.WriteLine("cluster.ini不存在");
                return;
            }
            _baseSet = new BaseSet(clusterIniFilePath);
            // 游戏风格
            DediBaseSetIntentionButton.DataContext = _baseSet;
            // 名称
            DediBaseSetClusterName.DataContext = _baseSet;
            // 描述
            DediBaseSetDescribe.DataContext = _baseSet;
            // 游戏模式
            BaseSetGameModeSelectBox.DataContext = _baseSet;
            // PVP
            BaseSetPvpSelectBox.DataContext = _baseSet;
            // 玩家
            BaseSetMaxPlayerSelectBox.DataContext = _baseSet;
            // 密码
            DediBaseSetSecret.DataContext = _baseSet;
            // 服务器模式
            BaseSetOfflineSelectBox.DataContext = _baseSet;
            // 无人时暂停
            BaseSetIsPauseSelectBox.DataContext = _baseSet;
            // 洞穴
            EditWorldIsCaveSelectBox.DataContext = _baseSet;
            Debug.WriteLine("基本设置-完");
        }

        #endregion

    }
}
